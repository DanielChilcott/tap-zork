// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.v3
{
    /// <summary>
    /// ZProcessor is the CPU of the ZMachine.
    /// </summary>
    [Serializable]
    public class ZProcessor : Common.ZProcessorCore
    {

        public ZProcessor()
        {
            z_random = new Random();
            InitializeOpcodeHandlerArrays();
        }

        public v3.ZHeader Header
        {
            get { return z_memory.Header as v3.ZHeader; }
        }

        /// <summary>
        /// Sets up the function pointer arrays. 
        /// </summary>
        protected override void InitializeOpcodeHandlerArrays()
        {
            base.InitializeOpcodeHandlerArrays();

            OP_2 = new Operation[] {
                ILLEGAL,         // 00
                OP_JE,           // 01
                OP_JL,           // 02
                OP_JG,           // 03
                OP_DEC_CHK,      // 04
                OP_INC_CHK,      // 05
                OP_JIN,          // 06
                OP_TEST,         // 07
                OP_OR,           // 08
                OP_AND,          // 09
                OP_TEST_ATTR,    // 0A
                OP_SET_ATTR,     // 0B
                OP_CLEAR_ATTR,   // 0C
                OP_STORE,        // 0D
                OP_INSERT_OBJ,   // 0E
                OP_LOADW,        // 0F
                OP_LOADB,        // 10
                OP_GET_PROP,     // 11
                OP_GET_PROP_ADDR,// 12
                OP_GET_NEXT_PROP,// 13
                OP_ADD,          // 14
                OP_SUB,          // 15
                OP_MUL,          // 16
                OP_DIV,          // 17
                OP_MOD,          // 18
                ILLEGAL,         // 19
                ILLEGAL,         // 1A
                ILLEGAL,         // 1B
                ILLEGAL,         // 1C
                ILLEGAL,         // 1D
                ILLEGAL,         // 1E
                ILLEGAL          // 1F
            };

            OP_1 = new Operation[] {
                OP_JZ,           // 00
                OP_GET_SIBLING,  // 01
                OP_GET_CHILD,    // 02
                OP_GET_PARENT,   // 03
                OP_GET_PROP_LEN, // 04
                OP_INC,          // 05
                OP_DEC,          // 06
                OP_PRINT_ADDR,   // 07
                ILLEGAL,         // 08
                OP_REMOVE_OBJ,   // 09
                OP_PRINT_OBJ,    // 0A
                OP_RET,          // 0B
                OP_JUMP,         // 0C
                OP_PRINT_PADDR,  // 0D
                OP_LOAD,         // 0E
                OP_NOT           // 0F
            };

            OP_0 = new Operation[] {
                OP_RTRUE,        // 00
                OP_RFALSE,       // 01
                OP_PRINT,        // 02
                OP_PRINT_RET,    // 03
                OP_NOP,          // 04
                OP_SAVE,         // 05
                OP_RESTORE,      // 06
                OP_RESTART,      // 07
                OP_RET_POPPED,   // 08
                OP_POP,          // 09
                OP_QUIT,         // 0A
                OP_NEW_LINE,     // 0B
                OP_SHOW_STATUS,  // 0C
                OP_VERIFY,       // 0D
                ILLEGAL,         // 0E
                ILLEGAL          // 0F
            };

            OP_VAR = new Operation[] {
                OP_CALL,         // 00
                OP_STOREW,       // 01
                OP_STOREB,       // 02
                OP_PUT_PROP,     // 03
                OP_SREAD,        // 04
                OP_PRINT_CHAR,   // 05
                OP_PRINT_NUM,    // 06
                OP_RANDOM,       // 07
                OP_PUSH,         // 08
                OP_PULL,         // 09
                OP_SPLIT_WINDOW, // 0A
                OP_SET_WINDOW,   // 0B
                ILLEGAL,         // 0C
                ILLEGAL,         // 0D
                ILLEGAL,         // 0E
                ILLEGAL,         // 0F
                ILLEGAL,         // 10
                ILLEGAL,         // 11
                ILLEGAL,         // 12
                OP_OUTPUT_STREAM,// 13
                OP_INPUT_STREAM, // 14
                ILLEGAL,         // 15
                ILLEGAL,         // 16
                ILLEGAL,         // 17
                ILLEGAL,         // 18
                ILLEGAL,         // 19
                ILLEGAL,         // 1A
                ILLEGAL,         // 1B
                ILLEGAL,         // 1C
                ILLEGAL,         // 1D
                ILLEGAL,         // 1E
                ILLEGAL          // 1F            
            };
        }


        public override Common.ZIO LoadStory(byte[] storyBytes)
        {        
            z_memory = new ZMemory();
            z_memory.LoadStory(storyBytes);

            if (z_memory.Header.VersionNumber != 3)
                throw new Exception(String.Format("Not a Version 3 Story File.  Version byte indicates: {0}", z_memory.Header.VersionNumber));
            
            if (!Initialized)
                Initialize();



            // Handle Version 3 Stuff
            byte b = z_memory.Header.Flags1;

            b &= 0x87;  // Tandy bit off, Status Line Available, Upper window not available, Fixed Width font

            z_memory.PutByte(0x01, b);

            return z_io;
        }

        protected override int UnpackAddress(int PADDR)
        {
            // Version 3: Multiply by 2
            return PADDR << 1;
        }

        protected override void Initialize()
        {
            base.Initialize();
            z_io = new Common.ZIO(this);
            z_frame = new ZMachine.Common.ZCallFrame(z_memory.Header.InitialPC);
            z_objtable = new v3.ZObjectTable(z_memory);
            z_dict = new v3.ZDictionary(z_memory as v3.ZMemory);
            z_instruct = new v3.ZInstruction(z_memory as v3.ZMemory);

            Initialized = true;
        }
    }
}
