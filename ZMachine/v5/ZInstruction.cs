using System;
using System.Collections.Generic;

namespace ZMachine.v5
{
    [Serializable]
    public class ZInstruction : v3.ZInstruction
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mem">Reference to the active ZMemory</param>
        public ZInstruction(ZMemory mem) : base(mem)
        {}

        protected override bool DecodeEXT(byte instruct)
        {
            // skip over the 0xbe byte
            PC++;

            m_opcodeType = ZMachine.Common.OpcodeType.OP_EXT;


            // Make the single byte into a full word so that coding
            // is easier.  The LSB of the word needs to be 0xFF
            m_vartype = (m_memory.GetByte(PC++) << 8) | 0xff;
     

            // Mask the opcode to strip off operand type bits
            m_opcode = (byte)(instruct & 0x1F);


            // Decode each bitpair of the vartype word to determine the
            // operand types.  If a 11b is encountered, then there are
            // no more operands.
            for (int t = 0; t < 8; t++)
            {
                bool EOF = false;

                switch ((m_vartype >> ((7 - t) * 2)) & 0x03)
                {
                    case 0x00:
                        // Long constant
                        FetchLongConstantOperand();
                        break;
                    case 0x01:
                        // Small constant
                        FetchSmallConstantOperand();
                        break;
                    case 0x02:
                        // Variable
                        FetchVariableOperand();
                        break;
                    default:  // i.e., case 0x03 and anything weird
                        // End of variables;
                        EOF = true;
                        break;
                }

                if (EOF)
                    break;

                m_operandCount++;
            }

            return true;
        }



        /// <summary>
        /// Initialize the operation metadata.  Deriving classes (for other
        /// ZMachine versions) should override this method.
        /// </summary>
        protected override void InitializeOps()
        {
            Ops[(int)ZMachine.Common.OpcodeType.OP_2] = new ZMachine.Common.ZOperationDef[]
            {   
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 00
                new ZMachine.Common.ZOperationDef("JE",            false, true),  // 01
                new ZMachine.Common.ZOperationDef("JL",            false, true),  // 02
                new ZMachine.Common.ZOperationDef("JG",            false, true),  // 03
                new ZMachine.Common.ZOperationDef("DEC_CHK",       false, true),  // 04
                new ZMachine.Common.ZOperationDef("INC_CHK",       false, true),  // 05
                new ZMachine.Common.ZOperationDef("JIN",           false, true),  // 06
                new ZMachine.Common.ZOperationDef("TEST",          false, true),  // 07
                new ZMachine.Common.ZOperationDef("OR",            true, false),  // 08
                new ZMachine.Common.ZOperationDef("AND",           true, false),  // 09
                new ZMachine.Common.ZOperationDef("TEST_ATTR",     false, true),  // 0a
                new ZMachine.Common.ZOperationDef("SET_ATTR",      false, false), // 0b
                new ZMachine.Common.ZOperationDef("CLEAR_ATTR",    false, false), // 0c
                new ZMachine.Common.ZOperationDef("STORE",         false, false), // 0d
                new ZMachine.Common.ZOperationDef("INSERT_OBJ",    false, false), // 0e
                new ZMachine.Common.ZOperationDef("LOADW",         true, false),  // 0f
                new ZMachine.Common.ZOperationDef("LOADB",         true, false),  // 10
                new ZMachine.Common.ZOperationDef("GET_PROP",      true, false),  // 11
                new ZMachine.Common.ZOperationDef("GET_PROP_ADDR", true, false),  // 12
                new ZMachine.Common.ZOperationDef("GET_NEXT_PROP", true, false),  // 13
                new ZMachine.Common.ZOperationDef("ADD",           true, false),  // 14
                new ZMachine.Common.ZOperationDef("SUB",           true, false),  // 15
                new ZMachine.Common.ZOperationDef("MUL",           true, false),  // 16
                new ZMachine.Common.ZOperationDef("DIV",           true, false),  // 17
                new ZMachine.Common.ZOperationDef("MOD",           true, false),  // 18
                new ZMachine.Common.ZOperationDef("CALL_2S",       true, false),  // 19
                new ZMachine.Common.ZOperationDef("CALL_2N",       false, false), // 1a
                new ZMachine.Common.ZOperationDef("SET_COLOR",     false, false), // 1b
                new ZMachine.Common.ZOperationDef("THROW",         false, false), // 1c
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 1d
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 1e
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false)  // 1f
            };

            Ops[(int)ZMachine.Common.OpcodeType.OP_1] = new ZMachine.Common.ZOperationDef[]
            {                   
                new ZMachine.Common.ZOperationDef("JZ",            false, true),  // 00
                new ZMachine.Common.ZOperationDef("GET_SIBLING",   true, true),   // 01
                new ZMachine.Common.ZOperationDef("GET_CHILD",     true, true),   // 02
                new ZMachine.Common.ZOperationDef("GET_PARENT",    true, false),  // 03
                new ZMachine.Common.ZOperationDef("GET_PROP_LEN",  true, false),  // 04
                new ZMachine.Common.ZOperationDef("INC",           false, false), // 05
                new ZMachine.Common.ZOperationDef("DEC",           false, false), // 06
                new ZMachine.Common.ZOperationDef("PRINT_ADDR",    false, false), // 07
                new ZMachine.Common.ZOperationDef("CALL_1S",       true, false),  // 08
                new ZMachine.Common.ZOperationDef("REMOVE_OBJ",    false, false), // 09
                new ZMachine.Common.ZOperationDef("PRINT_OBJ",     false, false), // 0a
                new ZMachine.Common.ZOperationDef("RET",           false, false), // 0b
                new ZMachine.Common.ZOperationDef("JUMP",          false, false), // 0c
                new ZMachine.Common.ZOperationDef("PRINT_PADDR",   false, false), // 0d
                new ZMachine.Common.ZOperationDef("LOAD",          true, false),  // 0e
                new ZMachine.Common.ZOperationDef("CALL_1N",       false, false)  // 0f
            };

            Ops[(int)ZMachine.Common.OpcodeType.OP_0] = new ZMachine.Common.ZOperationDef[]
            {                   
                new ZMachine.Common.ZOperationDef("RTRUE",         false, false), // 00
                new ZMachine.Common.ZOperationDef("RFALSE",        false, false), // 01
                new ZMachine.Common.ZOperationDef("PRINT",         false, false), // 02
                new ZMachine.Common.ZOperationDef("PRINT_RET",     false, false), // 03
                new ZMachine.Common.ZOperationDef("NOP",           false, false), // 04
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 05
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 06
                new ZMachine.Common.ZOperationDef("RESTART",       false, false), // 07
                new ZMachine.Common.ZOperationDef("RET_POPPED",    false, false), // 08
                new ZMachine.Common.ZOperationDef("CATCH",         true, false),  // 09
                new ZMachine.Common.ZOperationDef("QUIT",          false, false), // 0a
                new ZMachine.Common.ZOperationDef("NEW_LINE",      false, false), // 0b
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 0c
                new ZMachine.Common.ZOperationDef("VERIFY",        false, true),  // 0d
                new ZMachine.Common.ZOperationDef("EXTENDED",      false, false), // 0e
                new ZMachine.Common.ZOperationDef("PIRACY",        false, true)   // 0f
            };

            Ops[(int)ZMachine.Common.OpcodeType.OP_VAR] = new ZMachine.Common.ZOperationDef[]
            {   new ZMachine.Common.ZOperationDef("CALL_VS",       true, false),  // 00
                new ZMachine.Common.ZOperationDef("STOREW",        false, false), // 01
                new ZMachine.Common.ZOperationDef("STOREB",        false, false), // 02
                new ZMachine.Common.ZOperationDef("PUT_PROP",      false, false), // 03
                new ZMachine.Common.ZOperationDef("AREAD",         true, false),  // 04
                new ZMachine.Common.ZOperationDef("PRINT_CHAR",    false, false), // 05
                new ZMachine.Common.ZOperationDef("PRINT_NUM",     false, false), // 06
                new ZMachine.Common.ZOperationDef("RANDOM",        true, false),  // 07
                new ZMachine.Common.ZOperationDef("PUSH",          false, false), // 08
                new ZMachine.Common.ZOperationDef("PULL",          false, false), // 09
                new ZMachine.Common.ZOperationDef("SPLIT_WINDOW",  false, false), // 0A
                new ZMachine.Common.ZOperationDef("SET_WINDOW",    false, false), // 0B
                new ZMachine.Common.ZOperationDef("CALL_VS2",      true, false),  // 0C
                new ZMachine.Common.ZOperationDef("ERASE_WINDOW",  false, false), // 0D
                new ZMachine.Common.ZOperationDef("ERASE_LINE",    false, false), // 0E
                new ZMachine.Common.ZOperationDef("SET_CURSOR",    false, false), // 0F
                new ZMachine.Common.ZOperationDef("GET_CURSOR",    false, false), // 10
                new ZMachine.Common.ZOperationDef("SET_TEXT_STYLE",false, false), // 11
                new ZMachine.Common.ZOperationDef("BUFFER_MODE",   false, false), // 12
                new ZMachine.Common.ZOperationDef("OUTPUT_STREAM", false, false), // 13
                new ZMachine.Common.ZOperationDef("INPUT_STREAM",  false, false), // 14
                new ZMachine.Common.ZOperationDef("SOUND_EFFECT",  false, false), // 15
                new ZMachine.Common.ZOperationDef("READ_CHAR",     true, false),  // 16
                new ZMachine.Common.ZOperationDef("SCAN_TABLE",    true, true),   // 17
                new ZMachine.Common.ZOperationDef("NOT",           true, false),  // 18
                new ZMachine.Common.ZOperationDef("CALL_VN",       false, false), // 19
                new ZMachine.Common.ZOperationDef("CALL_VN2",      false, false), // 1A
                new ZMachine.Common.ZOperationDef("TOKENIZE",      false, false), // 1B
                new ZMachine.Common.ZOperationDef("ENCODE_TEXST",  false, false), // 1C
                new ZMachine.Common.ZOperationDef("COPY_TABLE",    false, false), // 1D
                new ZMachine.Common.ZOperationDef("PRINT_TABLE",   false, false), // 1E
                new ZMachine.Common.ZOperationDef("CHECK_ARG_COUNT", false, true) // 1F
            };

            Ops[(int)ZMachine.Common.OpcodeType.OP_EXT] = new ZMachine.Common.ZOperationDef[]
            {
                new ZMachine.Common.ZOperationDef("SAVE",          true, false),  // 00
                new ZMachine.Common.ZOperationDef("RESTORE",       true, false),  // 01
                new ZMachine.Common.ZOperationDef("LOG_SHIFT",     true, false),  // 02
                new ZMachine.Common.ZOperationDef("ART_SHIFT",     true, false),  // 03
                new ZMachine.Common.ZOperationDef("SET_FONT",      true, false),  // 04
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 05
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 06
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 07
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 08
                new ZMachine.Common.ZOperationDef("SAVE_UNDO",     true, false),  // 09
                new ZMachine.Common.ZOperationDef("RESTORE_UNDO",  true, false),  // 0A
                new ZMachine.Common.ZOperationDef("PRINT_UNICODE", false, false), // 0B
                new ZMachine.Common.ZOperationDef("CHECK_UNICODE", false, false), // 0C
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 0D
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 0E
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 0F
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 10
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 11
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 12
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 13
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 14
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 15
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 16
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 17
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 18
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 19
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 1A
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 1B
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false)  // 1C

            };
        }
    }
}
