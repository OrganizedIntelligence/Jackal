// 2013-04-27
//
// Cleaned up code and added comments. Using regions at the moment since I am just copying
// and pasting this into the GitHub editor. I'll create the files as they are organized
// on my machine soon. I'll also keep a condensed file for easier implementation so only
// one file needs to be added to a project to implement the interpreter.
//
// New implementation methods added:
//
// Static implementation examples:
//
//     Process file:
//         BrainFuck.Run("path/to/code.bs");
//
//     Process string:
//         BrainFuck.Run("++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>.");
//
// Read buffer on instantiation:
//     byte[] buffer = Encoding.GetEncoding(1252).GetBytes("++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>.");
//     BrainFuck bf = new BrainFuck(buffer);
//     bf.Run();
// Read file on instantiation:
//     BrainFuck bf = new BrainFuck("path/to/code.bs");
//     bf.Run();
//
// Read string on instantiation:
//     BrainFuck bf = new BrainFuck("++++++++++[>+++++++>++++++++++>+++>+<<<<-]>++.>+.+++++++..+++.>++.<<+++++++++++++++.>.+++.------.--------.>+.>.");
//     bf.Run();
//
// 2013-04-26
//
// This is not a pretty implementation. I'll clean up the file and add better commenting
// later.
//
// I'll be adding methods to load from a string, or load from a file and tidy up the novel
// crammed into one line in the example.
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
    using System.IO;
    using System.Linq;
    using System.Text;

    public partial class BrainFuck
    {
        #region BrainFuck.Reader.cs (Fields)
    	
		/// <summary>
        /// Buffer containing interpreter instructions
        /// </summary>
        private byte[] bufferInstructions;

        /// <summary>
        /// Current read position of buffer array
        /// </summary>
        private int bufferPosition = 0;

        /// <summary>
        /// Length of buffer array
        /// </summary>
        private int bufferLength = 0;

        /// <summary>
        /// Pointer data register
        /// </summary>
        private char[] pointerData = new char[30000];

        /// <summary>
        /// Pointer data bit counter
        /// </summary>
        private int pointerCounter = 0;

        /// <summary>
        /// Current register index of pointer data
        /// </summary>
        private int pointerIndex = 0;	
	
		#endregion
		
		#region BrainFuck.cs
	
        /// <summary>
        /// Creates new instance of BrainFuck object.
        /// </summary>
        public BrainFuck()
        {
        }

        /// <summary>
        /// Creates new instance of BrainFuck object and sets instruction data.
        /// </summary>
        /// <param name="buffer">Buffer containing interpreter instructions</param>
        public BrainFuck(byte[] buffer)
        {
            this.Initialize(buffer);
        }

        /// <summary>
        /// Creates new instance of BrainFuck object
        /// </summary>
        /// <param name="pathOrData">Path to file or string containing interpreter instructions</param>
        public BrainFuck(string pathOrData)
        {
            if (this.ValidatePath(pathOrData))
            {
                this.ReadFile(pathOrData);
            }
            else
            {
                this.ReadString(pathOrData);
            }
        }
		
		#endregion
		
		#region BrainFuck.Properties.cs (Properties)
		
        /// <summary>
        /// Gets or sets value indicating current pointers ascii character
        /// </summary>
        private char AsciiCharacter
        {
            get
            {
                return this.pointerData[this.pointerIndex];
            }

            set
            {
                this.pointerData[this.pointerIndex] = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating current pointers ascii code
        /// </summary>
        private int AsciiCode
        {
            get
            {
                return (int)this.pointerData[this.pointerIndex];
            }

            set
            {
                this.pointerData[this.pointerIndex] = (char)value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating position of current interpreter instruction buffer
        /// </summary>
        private int Position
        {
            get
            {
                return this.bufferPosition;
            }

            set
            {
                this.bufferPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating current interpreter instruction token
        /// </summary>
        private BrainFuckToken Token
        {
            get
            {
                return (BrainFuckToken)this.bufferInstructions[this.bufferPosition];
            }
        }		
		
		#endregion
		
		#region BrainFuck.Methods.cs
		
		/// <summary>
        /// Reads buffer using existing buffer.
        /// </summary>
        /// <returns>Returns updated BrainFuck object</returns>
        public BrainFuck Run()
        {
            return this.Start();
        }

        /// <summary>
        /// Sets and reads buffer containing interpreter instructions.
        /// </summary>
        /// <param name="buffer">Buffer containing interpreter instructions</param>
        /// <returns>Returns updated BrainFuck object</returns>
        public BrainFuck Run(byte[] buffer)
        {
            this.ReadBuffer(buffer);
            return this;
        }

        /// <summary>
        /// Static method to process file or string containing interpreter instructions.
        /// </summary>
        /// <param name="pathOrData">File path or string containing interpreter instructions</param>
        /// <returns>Returns new BrainFuck object</returns>
        public static BrainFuck Run(string pathOrData)
        {
            return new BrainFuck(pathOrData).Run();
        }

        /// <summary>
        /// Sets and reads new buffer containing interpreter instructions.
        /// </summary>
        /// <param name="buffer">Buffer containing interpreter instructions</param>
        public void ReadBuffer(byte[] buffer)
        {
            this.Initialize(buffer);
        }

        /// <summary>
        /// Reads from file path provided and sets interpreter instructions from file contents.
        /// </summary>
        /// <param name="path">File path to open file containing interpreter instructions</param>
        public void ReadFile(string path)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                this.Error("BrainFuck.ReadFile(string path) failed, path argument is empty.");
            }            
            else if (!File.Exists(path))
            {
                this.Error("BrainFuck.ReadFile(string path) failed, path provided does not exist.");
            }
            else
            {
                try
                {
                    this.ReadBuffer(File.ReadAllBytes(path));
                }
                catch(Exception ex)
                {
                    this.Error("BrainFuck.ReadString(string str) failed. {0}", ex.Message);
                }
            }
        }

        /// <summary>
        /// Sets string containing interpreter instructions.
        /// </summary>
        /// <param name="str">String containing interpreter instructions</param>
        public void ReadString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                this.Error("BrainFuck.ReadString(string str) failed, str argument is empty.");               
            }
            else
            {
                this.Initialize(Encoding.GetEncoding(1252).GetBytes(str));
            }
        }

        /// <summary>
        /// Checks if path contains any invalid characters, then checks if
        /// file path exists.
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Returns true if file exists</returns>
        private bool ValidatePath(string path)
        {
            bool result = true;

            try
            {
                result = (path.IndexOfAny(Path.GetInvalidPathChars()) == -1 && File.Exists(path));
            }
            catch(Exception ex)
            {
                result = false;
                this.Error("BrainFuck.ValidatePath(string path) failed. {0}", ex.Message);
            }

            return result;
        }
		
		#endregion
		
		#region BrainFuck.Reader.cs
		
        /// <summary>
        /// Start processing interpreter instructions.
        /// </summary>
        /// <returns>Returns updated BrainFuck object</returns>
        private BrainFuck Start()
        {
            return this.Start(0);
        }

        /// <summary>
        /// Start processing interpreter instructions from specified position.
        /// </summary>
        /// <param name="start">Starting position of interpreter instructions</param>
        /// <returns>Returns updated BrainFuck object</returns>
        private BrainFuck Start(int start)
        {
            this.bufferPosition = (start - 1);

            while (this.ReadNext())
            {
                switch (this.Token)
                {
                    case BrainFuckToken.Next:
                        this.Next();
                        break;
                    case BrainFuckToken.Prev:
                        this.Previous();
                        break;
                    case BrainFuckToken.Increment:
                        this.Increment();
                        break;
                    case BrainFuckToken.Decrement:
                        this.Decrement();
                        break;
                    case BrainFuckToken.Input:
                        this.Input();
                        break;
                    case BrainFuckToken.Print:
                        this.Print();
                        break;
                    case BrainFuckToken.StartLoop:
                        this.StartLoop();
                        break;
                    case BrainFuckToken.EndLoop:
                        this.EndLoop();
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// Moves to next position in instruction buffer.
        /// </summary>
        /// <returns>Returns false if position exceeds instruction buffer length</returns>
        private bool ReadNext()
        {   
            this.bufferPosition++;

            return (this.bufferPosition < this.bufferLength);
        }

        /// <summary>
        /// Moves to previous position in instruction buffer.
        /// </summary>
        /// <returns>Returns false if position is already at beginning of instruction buffer</returns>
        private bool ReadPrev()
        {
            this.bufferPosition--;

            return (this.bufferPosition > -1);
        }
		
		#endregion
		
		#region BrainFuck.Instructions.cs
		
		        /// <summary>
        /// Initializes or resets interpreter instruction buffer.
        /// </summary>
        /// <param name="buffer">Buffer containing interpreter instructions</param>
        /// <returns>Returns updated BrainFuck object</returns>
        private BrainFuck Initialize(byte[] buffer)
        {
            this.pointerIndex = 0;
            this.bufferPosition = -1;
            this.bufferLength = buffer.Count();
            this.bufferInstructions = buffer;
            this.pointerData = new char[30000];

            return this;
        }

        /// <summary>
        /// Steps into conditional loop if pointer value is zero. If pointer value
        /// is not zero, then skip instructions up to matching end loop token
        /// </summary>
        private void StartLoop()
        {
            int depth = 0;

            if ((int)this.pointerData[this.pointerIndex] == 0)
            {
                while (this.ReadNext())
                {
                    if (this.Token == BrainFuckToken.EndLoop && depth == 0)
                    {
                        break;
                    }
                    else if (this.Token == BrainFuckToken.EndLoop && depth > 0)
                    {
                        depth--;
                    }
                    else if (this.Token == BrainFuckToken.StartLoop)
                    {
                        depth++;
                    }
                }
            }
        }
        
        /// <summary>
        /// Decrement pointer counter
        /// </summary>
        private void Decrement()
        {
            this.pointerCounter--;
            this.pointerData[this.pointerIndex] = (char)this.pointerCounter;
        }

        /// <summary>
        /// Increment pointer counter
        /// </summary>
        private void Increment()
        {
            this.pointerCounter++;
            this.pointerData[this.pointerIndex] = (char)this.pointerCounter;
        }

        /// <summary>
        /// Read console key provided by user and set value to pointer
        /// </summary>
        private void Input()
        {
            this.pointerData[this.pointerIndex] = Console.ReadKey(false).KeyChar;
        }

        /// <summary>
        /// Steps out of conditional loop if pointer value is not zero. If pointer
        /// value is zero, jump back to matching start loop token.
        /// </summary>
        private void EndLoop()
        {
            int depth = 0;

            if ((int)this.pointerData[this.pointerIndex] != 0)
            {
                while (this.ReadPrev())
                {
                    if (this.Token == BrainFuckToken.StartLoop && depth == 0)
                    {
                        break;
                    }
                    else if (this.Token == BrainFuckToken.StartLoop && depth > 0)
                    {
                        depth--;
                    }
                    else if (this.Token == BrainFuckToken.EndLoop)
                    {
                        depth++;
                    }
                }

                this.ReadPrev();
            }
        }

        /// <summary>
        /// Writes error message to terminal.
        /// </summary>
        /// <param name="err">Error message</param>
        private void Error(string err)
        {
            Console.WriteLine("Error: {0}", err);
        }

        /// <summary>
        /// Writes error message and arguments to terminal.
        /// </summary>
        /// <param name="format">Error message format</param>
        /// <param name="args">Error message arguments</param>
        private void Error(string format, params object[] args)
        {
            if (args != null && args.Count() > 0)
            {
                this.Error(string.Format(format, args));
            }
            else
            {
                this.Error(format);
            }
        }

        /// <summary>
        /// Moves to next pointer and updates pointer counter.
        /// </summary>
        private void Next()
        {
            this.pointerIndex++;
            this.pointerCounter = (int)this.pointerData[this.pointerIndex];
        }

        /// <summary>
        /// Moves to previous pointer and updates pointer counter.
        /// </summary>
        private void Previous()
        {
            this.pointerIndex--;
            this.pointerCounter = (int)this.pointerData[this.pointerIndex];
        }

        /// <summary>
        /// Writes pointer ascii character to terminal.
        /// </summary>
        private void Print()
        {
            Console.Write(this.AsciiCharacter);
        }
		
		#endregion
    }
	
	#region BrainFuckToken.cs
	
	/// <summary>
    /// Interpreter instruction tokens
    /// </summary>
	public enum BrainFuckToken : byte
    {
        /// <summary>
        /// No instruction provided
        /// </summary>
        None,
        /// <summary>
        /// Beginning pf program
        /// </summary>
        Start,
        /// <summary>
        /// End of program
        /// </summary>
        End,
        /// <summary>
        /// Evaluate next instruction token
        /// </summary>
        Next = 0x3e,
        /// <summary>
        /// Evaluate previous instruction token
        /// </summary>
        Prev = 0x3c,
        /// <summary>
        /// Increment current pointer value
        /// </summary>
        Increment = 0x2b,
        /// <summary>
        /// Decrement current pointer value
        /// </summary>
        Decrement = 0x2d,
        /// <summary>
        /// Print current pointer value to terminal
        /// </summary>
        Print = 0x2e,
        /// <summary>
        /// Get response from user and set current pointer value to result
        /// </summary>
        Input = 0x2c,
        /// <summary>
        /// Start conditional loop
        /// </summary>
        StartLoop = 0x5b,
        /// <summary>
        /// End conditional loop
        /// </summary>
        EndLoop = 0x5d
    }
	
	#endregion
}
