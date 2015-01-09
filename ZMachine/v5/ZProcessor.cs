// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.v5
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

        public v5.ZHeader Header
        {
            get { return z_memory.Header as v5.ZHeader; }
        }

        public override Common.ZIO LoadStory(byte[] storyBytes)
        {
            z_memory = new ZMemory();
            z_memory.LoadStory(storyBytes);

            if (z_memory.Header.VersionNumber != 5)
                throw new Exception(String.Format("Not a Version 5 Story File.  Version byte indicates: {0}", z_memory.Header.VersionNumber));

            if (!Initialized)
                Initialize();

            // Handle Version 5 Stuff
            byte b = z_memory.Header.Flags1;

            //b |= 0x80;  // Timed Keyboard input available (v4+)
            //b |= 0x40;  // ??
            //b |= 0x20;  // Sound effects available (v6+)
            //b |= 0x10;  // Monospace available (v4+)
            //b |= 0x08;  // Italic available (v4+)
            //b |= 0x04;  // Boldface available (v4+)
            //b |= 0x02;  // Picture displaying available (v6+)
            //b |= 0x01;  // Color Available (v5+)

            // Default: 0x010000
            b &= 0x40;
            b |= 0x10;
            z_memory.PutByte(0x01, b);

            return z_io;
        }

        protected override void Initialize()
        {
            base.Initialize();
            z_io = new Common.ZIO(this);
            z_frame = new ZMachine.Common.ZCallFrame(z_memory.Header.InitialPC);
            z_objtable = new v5.ZObjectTable(z_memory);
            z_dict = new v5.ZDictionary(z_memory as v5.ZMemory);
            z_instruct = new v5.ZInstruction(z_memory as v5.ZMemory);

            Initialized = true;
        }


        /// <summary>
        /// Sets up the function pointer arrays.  Deriving classes (i.e., later versions) should override this method
        /// in order to set pointers to new opcode handlers.
        /// </summary>
        protected override void InitializeOpcodeHandlerArrays()
        {
            OP_2 = new Operation[] {
                ILLEGAL,                    // 00
                OP_JE,                      // 01
                OP_JL,                      // 02
                OP_JG,                      // 03
                OP_DEC_CHK,                 // 04
                OP_INC_CHK,                 // 05
                OP_JIN,                     // 06
                OP_TEST,                    // 07
                OP_OR,                      // 08
                OP_AND,                     // 09
                OP_TEST_ATTR,               // 0A
                OP_SET_ATTR,                // 0B
                OP_CLEAR_ATTR,              // 0C
                OP_STORE,                   // 0D
                OP_INSERT_OBJ,              // 0E
                OP_LOADW,                   // 0F
                OP_LOADB,                   // 10
                OP_GET_PROP,                // 11
                OP_GET_PROP_ADDR,           // 12
                OP_GET_NEXT_PROP,           // 13
                OP_ADD,                     // 14
                OP_SUB,                     // 15
                OP_MUL,                     // 16
                OP_DIV,                     // 17
                OP_MOD,                     // 18
                OP_CALL_2S,                 // 19
                OP_CALL_2N,                 // 1A
                OP_SET_COLOR,               // 1B
                OP_THROW,                   // 1C
                ILLEGAL,                    // 1D
                ILLEGAL,                    // 1E
                ILLEGAL                     // 1F
            };

            OP_1 = new Operation[] {
                OP_JZ,                      // 00
                OP_GET_SIBLING,             // 01
                OP_GET_CHILD,               // 02
                OP_GET_PARENT,              // 03
                OP_GET_PROP_LEN,            // 04
                OP_INC,                     // 05
                OP_DEC,                     // 06
                OP_PRINT_ADDR,              // 07
                OP_CALL_1S,                 // 08
                OP_REMOVE_OBJ,              // 09
                OP_PRINT_OBJ,               // 0A
                OP_RET,                     // 0B
                OP_JUMP,                    // 0C
                OP_PRINT_PADDR,             // 0D
                OP_LOAD,                    // 0E
                OP_CALL_1N                  // 0F
            };


            OP_0 = new Operation[] {
                OP_RTRUE,                   // 00
                OP_RFALSE,                  // 01
                OP_PRINT,                   // 02
                OP_PRINT_RET,               // 03
                OP_NOP,                     // 04
                ILLEGAL,                    // 05
                ILLEGAL,                    // 06
                OP_RESTART,                 // 07
                OP_RET_POPPED,              // 08
                OP_CATCH,                   // 09
                OP_QUIT,                    // 0A
                OP_NEW_LINE,                // 0B
                ILLEGAL,                    // 0C
                OP_VERIFY,                  // 0D
                EXTENDED,                   // 0E
                OP_PIRACY                   // 0F
            };

            OP_VAR = new Operation[] {
                OP_CALL_VS,                 // 00
                OP_STOREW,                  // 01
                OP_STOREB,                  // 02
                OP_PUT_PROP,                // 03
                OP_AREAD,                   // 04
                OP_PRINT_CHAR,              // 05
                OP_PRINT_NUM,               // 06
                OP_RANDOM,                  // 07
                OP_PUSH,                    // 08
                OP_PULL,                    // 09
                OP_SPLIT_WINDOW,            // 0A
                OP_SET_WINDOW,              // 0B
                OP_CALL_VS2,                // 0C
                OP_ERASE_WINDOW,            // 0D
                OP_ERASE_LINE,              // 0E
                OP_SET_CURSOR,              // 0F
                OP_GET_CURSOR,              // 10
                OP_SET_TEXT_STYLE,          // 11
                OP_BUFFER_MODE,             // 12
                OP_OUTPUT_STREAM,           // 13
                OP_INPUT_STREAM,            // 14
                OP_SOUND_EFFECT,            // 15
                OP_READ_CHAR,               // 16
                OP_SCAN_TABLE,              // 17
                OP_NOT,                     // 18
                OP_CALL_VN,                 // 19
                OP_CALL_VN2,                // 1A
                OP_TOKENIZE,                // 1B
                OP_ENCODE_TEXT,             // 1C
                OP_COPY_TABLE,              // 1D
                OP_PRINT_TABLE,             // 1E
                OP_CHECK_ARG_COUNT          // 1F            
            };

            OP_EXT = new Operation[] {
                OP_SAVE,                    // 00
                OP_RESTORE,                 // 01
                OP_LOG_SHIFT,               // 02
                OP_ART_SHIFT,               // 03
                OP_SET_FONT,                // 04
                ILLEGAL,                    // 05
                ILLEGAL,                    // 06
                ILLEGAL,                    // 07
                ILLEGAL,                    // 08
                OP_SAVE_UNDO,               // 09
                OP_RESTORE_UNDO,            // 0A
                OP_PRINT_UNICODE,           // 0B
                OP_CHECK_UNICODE,           // 0C
                ILLEGAL,                    // 0D
                ILLEGAL,                    // 0E
                ILLEGAL,                    // 0F
                ILLEGAL,                    // 10
                ILLEGAL,                    // 11
                ILLEGAL,                    // 12
                ILLEGAL,                    // 13
                ILLEGAL,                    // 14
                ILLEGAL,                    // 15
                ILLEGAL,                    // 16
                ILLEGAL,                    // 17
                ILLEGAL,                    // 18
                ILLEGAL,                    // 19
                ILLEGAL,                    // 1A
                ILLEGAL,                    // 1B
                ILLEGAL                     // 1C
           };
        }

        protected override int UnpackAddress(int PADDR)
        {
            // Version 4 & 5: Multiply by 4
            return PADDR << 2;
        }

    }
}
