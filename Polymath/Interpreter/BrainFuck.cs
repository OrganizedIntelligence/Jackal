// This is not a pretty implementation. I'll clean up the file and add better commenting
// later. Decided to write the interpreter since I didn't see any c# implementations that
// were easy to find and use.
//
// This is simple enough to implement.
// Example:
// 
//    using Polymath.Interpreter;
//
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            BrainFuck bf = new BrainFuck().Run(Encoding.GetEncoding(1252).GetBytes("++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>."));
//            Console.ReadLine();
//        }
//    }
//
// Again, many better ways to go about it. Simple edits to make this easier to read and easier
// to maintain. Next time I edit this, it will be much more fun to play with than it is now.
//
// If you find this useful in its current state, hooray!
//
namespace Polymath.Interpreter
{
    using System;
    using System.Linq;

    public class BrainFuck
    {
        private byte[] buffer;
        private char[] register = new char[30000];
        private int counter = 0;
        private int index = 0;
        private int position = 0;
        private int length = 0;

        public BrainFuck()
        {

        }

        public BrainFuck(byte[] data)
        {
            this.buffer = data;
            this.length = data.Count();
        }
        
        public BrainFuck Run(byte[] data)
        {
            this.index = 0;
            this.position = -1;
            this.length = data.Count();
            this.buffer = data;
            this.register = new char[30000];
            this.Start(-1);
            return this;
        }

        private void Start(int start)
        {
            this.position = start;

            while (this.Next())
            {
                switch (this.Token())
                {
                    case BrainFuckToken.IncrementDataPointer:
                        this.index++;
                        this.counter = (int)this.register[this.index];
                        break;
                    case BrainFuckToken.DecrementDataPointer:
                        this.index--;
                        this.counter = (int)this.register[this.index];
                        break;
                    case BrainFuckToken.IncrementCounter:
                        this.counter++;
                        this.register[this.index] = (char)this.counter;
                        break;
                    case BrainFuckToken.DecrementCounter:
                        this.counter--;
                        this.register[this.index] = (char)this.counter;
                        break;
                    case BrainFuckToken.Input:
                        break;
                    case BrainFuckToken.Output:
                        Console.Write(this.register[this.index]);
                        break;
                    case BrainFuckToken.Compare:
                        if ((int)this.register[this.index] == 0)
                        {
                            while (this.Next())
                            {
                                if (this.Token() == BrainFuckToken.Jump)
                                {
                                    break;
                                }
                            }
                        }
                        break;
                    case BrainFuckToken.Jump:
                        if ((int)this.register[this.index] != 0)
                        {
                            while (this.Prev())
                            {
                                if (this.Token() == BrainFuckToken.Compare)
                                {
                                    this.Prev();
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private BrainFuckToken Token()
        {
            return (BrainFuckToken)this.buffer[this.position];
        }

        private bool Next()
        {   
            this.position++;
            return (this.position < this.length);
        }

        private void Output()
        {
            Console.Write((char)this.Value());
        }

        private bool Prev()
        {
            this.position--;
            return (this.position > -1);
        }

        private int Value()
        {
            return (int)this.register[this.index];
        }
    }

    public enum BrainFuckToken : byte
    {
        None,
        Start,
        End,
        IncrementDataPointer = 0x3e,
        DecrementDataPointer = 0x3c,
        IncrementCounter = 0x2b,
        DecrementCounter = 0x2d,
        Output = 0x2e,
        Input = 0x2c,
        Compare = 0x5b,
        Jump = 0x5d
    }
}
