// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.Common
{
    [Serializable]
    public partial class ZProcessorCore : IZProcessor
    {
        // Version Specific
        protected IZMemory z_memory = null;
        protected IZDictionary z_dict = null;
        protected IZInstruction z_instruct = null;
        protected ZObjectTableCore z_objtable = null;

        // Common
        protected ZMachine.Common.ZStatus z_status;
        protected ZMachine.Common.ZCallFrame z_frame = null;
        protected ZIO z_io = null;
        protected Random z_random = null;

        protected bool isWaitingForInput = false;

        /// <summary>
        /// Operation Delegate: All opcode handlers use this signature to enable
        /// the use of function pointers
        /// </summary>
        /// <param name="z_instr">IZInstruction variable containing the decoded instruction information</param>
        protected delegate void Operation(IZInstruction z_instr);


        // Function pointer arrays by type
        protected Operation[] OP_2;
        protected Operation[] OP_1;
        protected Operation[] OP_0;
        protected Operation[] OP_VAR;
        protected Operation[] OP_EXT;


        // Architecture change to support SQLCLR: Got rid of events in favor of single delegates
        public delegate void SaveHandler();
        public SaveHandler OnSave = null;

        public delegate void RestoreHandler();
        public RestoreHandler OnRestore = null;

        public delegate void RestartHandler();
        public RestartHandler OnRestart = null;

        public virtual void Stop()
        {
            Running = false;
        }

        public virtual void Start()
        {
            if (z_memory == null)
                return;

            Running = true;
            Execute();
        }

        public bool Running { get; set; }
        public bool Initialized { get; protected set; }


        public virtual Common.ZIO LoadStory(byte[] storyBytes)
        {
            throw new NotImplementedException("Not Implemented.");
        }

        protected virtual int UnpackAddress(int PADDR)
        {
            throw new NotImplementedException("Not Implemented.");
        }

        protected virtual void Initialize()
        {
        }
        


        public virtual ZMachine.Common.ZStatus GetStatus()
        {
            z_status.IsTimedGame = ((z_memory.Header.Flags1 & 0x02) == 0x02);
            z_status.Location = ZMachine.Common.ZText.PrintZString(z_memory, z_objtable.getObjectName(z_memory.GetVariable(16)));
            z_status.Num1 = z_memory.GetVariable(17);
            z_status.Num2 = z_memory.GetVariable(18);

            return z_status;
        }

        //public virtual string GetOutput(string input)
        //{
        //    if (isWaitingForInput && z_instruct != null)
        //    {
        //        isWaitingForInput = false;
        //        ParseInput(input, z_instruct);
        //    }

        //    Start();

        //    string ret = output.ToString();
        //    output = new StringBuilder();

        //    return ret;
        //}


        protected virtual void Execute()
        {
            while (Running)
            {
                z_instruct.Decode(z_frame.PC);

#if (VERBOSEDEBUG)
                System.Diagnostics.Debug.WriteLine(z_instruct.ToString());
#endif

                z_frame.PC += z_instruct.Length;

                switch (z_instruct.OpcodeType)
                {
                    case ZMachine.Common.OpcodeType.OP_0:
                        OP_0[z_instruct.OpCode](z_instruct);
                        break;
                    case ZMachine.Common.OpcodeType.OP_1:
                        OP_1[z_instruct.OpCode](z_instruct);
                        break;
                    case ZMachine.Common.OpcodeType.OP_2:
                        OP_2[z_instruct.OpCode](z_instruct);
                        break;
                    case ZMachine.Common.OpcodeType.OP_VAR:
                        OP_VAR[z_instruct.OpCode](z_instruct);
                        break;
                    case ZMachine.Common.OpcodeType.OP_EXT:
                        OP_EXT[z_instruct.OpCode](z_instruct);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }








        /// <summary>
        /// Sets up the function pointer arrays.  Deriving classes (i.e., later versions) should override this method
        /// in order to set pointers to new opcode handlers.
        /// </summary>
        protected virtual void InitializeOpcodeHandlerArrays()
        {
            OP_2 = new Operation[] {
                ILLEGAL,        // 00
                ILLEGAL,        // 01
                ILLEGAL,        // 02
                ILLEGAL,        // 03
                ILLEGAL,        // 04
                ILLEGAL,        // 05
                ILLEGAL,        // 06
                ILLEGAL,        // 07
                ILLEGAL,        // 08
                ILLEGAL,        // 09
                ILLEGAL,        // 0A
                ILLEGAL,        // 0B
                ILLEGAL,        // 0C
                ILLEGAL,        // 0D
                ILLEGAL,        // 0E
                ILLEGAL,        // 0F
                ILLEGAL,        // 10
                ILLEGAL,        // 11
                ILLEGAL,        // 12
                ILLEGAL,        // 13
                ILLEGAL,        // 14
                ILLEGAL,        // 15
                ILLEGAL,        // 16
                ILLEGAL,        // 17
                ILLEGAL,        // 18
                ILLEGAL,        // 19
                ILLEGAL,        // 1A
                ILLEGAL,        // 1B
                ILLEGAL,        // 1C
                ILLEGAL,        // 1D
                ILLEGAL,        // 1E
                ILLEGAL         // 1F
            };

            OP_1 = new Operation[] {
                ILLEGAL,        // 00
                ILLEGAL,        // 01
                ILLEGAL,        // 02
                ILLEGAL,        // 03
                ILLEGAL,        // 04
                ILLEGAL,        // 05
                ILLEGAL,        // 06
                ILLEGAL,        // 07
                ILLEGAL,        // 08
                ILLEGAL,        // 09
                ILLEGAL,        // 0A
                ILLEGAL,        // 0B
                ILLEGAL,        // 0C
                ILLEGAL,        // 0D
                ILLEGAL,        // 0E
                ILLEGAL         // 0F
            };

            OP_0 = new Operation[] {
                ILLEGAL,        // 00
                ILLEGAL,        // 01
                ILLEGAL,        // 02
                ILLEGAL,        // 03
                ILLEGAL,        // 04
                ILLEGAL,        // 05
                ILLEGAL,        // 06
                ILLEGAL,        // 07
                ILLEGAL,        // 08
                ILLEGAL,        // 09
                ILLEGAL,        // 0A
                ILLEGAL,        // 0B
                ILLEGAL,        // 0C
                ILLEGAL,        // 0D
                ILLEGAL,        // 0E
                ILLEGAL         // 0F
            };

            OP_VAR = new Operation[] {
                ILLEGAL,        // 00
                ILLEGAL,        // 01
                ILLEGAL,        // 02
                ILLEGAL,        // 03
                ILLEGAL,        // 04
                ILLEGAL,        // 05
                ILLEGAL,        // 06
                ILLEGAL,        // 07
                ILLEGAL,        // 08
                ILLEGAL,        // 09
                ILLEGAL,        // 0A
                ILLEGAL,        // 0B
                ILLEGAL,        // 0C
                ILLEGAL,        // 0D
                ILLEGAL,        // 0E
                ILLEGAL,        // 0F
                ILLEGAL,        // 10
                ILLEGAL,        // 11
                ILLEGAL,        // 12
                ILLEGAL,        // 13
                ILLEGAL,        // 14
                ILLEGAL,        // 15
                ILLEGAL,        // 16
                ILLEGAL,        // 17
                ILLEGAL,        // 18
                ILLEGAL,        // 19
                ILLEGAL,        // 1A
                ILLEGAL,        // 1B
                ILLEGAL,        // 1C
                ILLEGAL,        // 1D
                ILLEGAL,        // 1E
                ILLEGAL         // 1F            
            };

            OP_EXT = new Operation[] {
                ILLEGAL,        // 00
                ILLEGAL,        // 01
                ILLEGAL,        // 02
                ILLEGAL,        // 03
                ILLEGAL,        // 04
                ILLEGAL,        // 05
                ILLEGAL,        // 06
                ILLEGAL,        // 07
                ILLEGAL,        // 08
                ILLEGAL,        // 09
                ILLEGAL,        // 0A
                ILLEGAL,        // 0B
                ILLEGAL,        // 0C
                ILLEGAL,        // 0D
                ILLEGAL,        // 0E
                ILLEGAL,        // 0F
                ILLEGAL,        // 10
                ILLEGAL,        // 11
                ILLEGAL,        // 12
                ILLEGAL,        // 13
                ILLEGAL,        // 14
                ILLEGAL,        // 15
                ILLEGAL,        // 16
                ILLEGAL,        // 17
                ILLEGAL,        // 18
                ILLEGAL,        // 19
                ILLEGAL,        // 1A
                ILLEGAL,        // 1B
                ILLEGAL,        // 1C
                ILLEGAL,        // 1D
                ILLEGAL,        // 1E
                ILLEGAL         // 1F            
            };
        }
    }
}
