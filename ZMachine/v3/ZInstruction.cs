// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace ZMachine.v3
{
    /// <summary>
    /// ZInstruction is responsible for decoding instructions
    /// </summary>
    [Serializable]
    public class ZInstruction : IZInstruction
    {
        // Reference to the active ZMemory object
        protected ZMemory m_memory = null;

        // Holds the list of Operands as decoded.  If an operand is a variable, then the contents of the 
        // variable will be held in this array
        protected short[] m_operands = new short[8];

        // Holds the list of Operands as decoded.  If an operand is a variable, then the variable number
        // will be held in this array
        protected short[] m_rawoperands = new short[8];

        // Holds the Operand Type for each Operand as decoded
        protected ZMachine.Common.OperandType[] m_operandTypes = new ZMachine.Common.OperandType[8];

        // Tracks the number of Operands (i.e., even though the operand array has 8 elements, maybe only 2 will be used)
        protected int m_operandCount = 0;

        // Holds the opcode number.  This combined with the Opcode Type determines which instruction to execute
        protected byte m_opcode = 0;

        // Holds the opcode type.  This combined with the Opcode Number determines which instruction to execute
        protected ZMachine.Common.OpcodeType m_opcodeType;

        // Working Program Counter that is used during decoding (is not the "live" program counter from the call frame)
        protected int PC = 0;

        // Holds the instruction length after decoding.  
        protected int m_length = 0;

        // Holds the "vartype" byte(s) to determine the type (and number) of operands
        protected int m_vartype = -1;

        private byte m_branch = 0;
        private int m_offset = 0;

        private byte m_result = 0;

        public byte Result
        {
            get { return m_result; }
            set { m_result = value; }
        }

        public byte Branch
        {
            get { return m_branch; }
            set { m_branch = value; }
        }

        public int Offset
        {
            get { return m_offset; }
            set { m_offset = value; }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="mem">Reference to the active ZMemory</param>
        public ZInstruction(ZMemory mem)
        {
            InitializeOps();
            m_memory = mem;
        }

        /// <summary>
        /// The start of each decode process resets all of the member variables
        /// to default values
        /// </summary>
        protected virtual void Reset()
        {
            m_opcodeType = ZMachine.Common.OpcodeType.UNKNOWN;
            m_operandCount = 0;
            m_operands = new short[8];
            m_rawoperands = new short[8];
            m_operandTypes = new ZMachine.Common.OperandType[8];
            m_length = 0;
            m_vartype = -1;
            m_branch = 0;
            m_offset = 0;
        }

        /// <summary>
        /// Masked opcode of the current instruction.  Used in conjunction with the
        /// OpcodeType property in order to determine which operation to perform.
        /// </summary>
        public byte OpCode
        {
            get { return m_opcode; }
        }

        /// <summary>
        /// Type of the current Opcode.  Used in conjunction with the OpCode property 
        /// in order to determine which operation to perform.
        /// </summary>
        public ZMachine.Common.OpcodeType OpcodeType
        {
            get { return m_opcodeType; }
        }

        /// <summary>
        /// Number of operands in the current instruction
        /// </summary>
        public int OperandCount
        {
            get { return m_operandCount; }
        }

        /// <summary>
        /// The operands array.  Note: Array will likely have more elements than are
        /// actually populated.  Use in conjunction with the OperandCount property.
        /// </summary>
        public short[] Operands
        {
            get { return m_operands; }
        }

        /// <summary>
        /// The operands type array.  Note: Array will likely have more elements than
        /// are actually populated.  Use in conjunction with the OperandCount property.
        /// </summary>
        public ZMachine.Common.OperandType[] OperandTypes
        {
            get { return m_operandTypes; }
        }

        /// <summary>
        /// Length of the current instruction
        /// </summary>
        public int Length
        {
            get { return m_length; }
        }

        /// <summary>
        /// Here's where all of the magic resides.  Decodes the program bytes, and
        /// populates the member variables of the object.
        /// </summary>
        /// <param name="zPC">Program counter value passed in from the ZProcessor</param>
        public virtual void Decode(int zPC)
        {
            // Set the working PC to the value of the actual PC
            PC = zPC;

            // Initialize everything to default values
            Reset();

            // Get the first byte...
            byte instruct = m_memory.GetByte(PC++);

            // ... and start decoding!
            if (instruct < 0x80)
            {
                Decode2OP(instruct);
            }
            else if (instruct < 0xB0)
            {
                Decode1OP(instruct);
            }
            else if (instruct == 0xBE)
            {
                DecodeEXT(m_memory.GetByte(PC));
            }
            else if (instruct < 0xC0)
            {
                Decode0OP(instruct);
            }
            else if (instruct < 0xE0 && instruct != 0xC1)
            {
                DecodeVAR2OP(instruct);
            }
            else
            {
                DecodeVARVAR(instruct);
            }

            // Get Store
            if (OpIsStore())
            {
                m_result = m_memory.GetByte(PC++);
            }
            // Get Branch
            if (OpIsBranch())
            {
                m_branch = m_memory.GetByte(PC++);
                m_offset = (m_branch & 0x3f);

                // if bit 6 of branch byte is 1, offset is 6 bits (0-63) else 14 bits
                if ((m_branch & 0x40) == 0)
                {
                    m_offset = (m_offset << 8) + m_memory.GetByte(PC++);
                }
            }

            // Determine instruction length by subtracting original PC from 
            // current working PC
            m_length = PC - zPC;
        }

        protected virtual void Decode2OP(byte instruct)
        {
            // This is a 2OP
            m_opcodeType = ZMachine.Common.OpcodeType.OP_2;

            if ((instruct & 0x40) == 0x40)
            {
                // Operand 1 is a variable
                FetchVariableOperand();
            }
            else
            {
                // Operand 1 is a constant
                FetchSmallConstantOperand();
            }
            m_operandCount++;

            if ((instruct & 0x20) == 0x20)
            {
                // Operand 2 is a variable
                FetchVariableOperand();
            }
            else
            {
                // Operand 2 is a constant
                FetchSmallConstantOperand();
            }
            m_operandCount++;

            // Mask the opcode to strip off operand type bits
            m_opcode = (byte)(instruct & 0x1F);
        }
        protected virtual void Decode1OP(byte instruct)
        {
            // 1OP
            m_opcodeType = ZMachine.Common.OpcodeType.OP_1;

            if ((instruct & 0x30) == 0x00)
            {
                // Operand 1 is a long constant
                FetchLongConstantOperand();
            }
            else if ((instruct & 0x30) == 0x10)
            {
                // Operand 1 is a small constant
                FetchSmallConstantOperand();
            }
            else if ((instruct & 0x30) == 0x20)
            {
                // Operand 1 is a variable
                FetchVariableOperand();
            }
            m_operandCount++;

            // Mask the opcode to strip off operand type bits
            m_opcode = (byte)(instruct & 0x0F);
        }
        protected virtual void Decode0OP(byte instruct)
        {
            // 0OP (no operands)
            m_opcodeType = ZMachine.Common.OpcodeType.OP_0;

            // Mask the opcode to strip off operand type bits
            m_opcode = (byte)(instruct & 0x0F);
        }
        protected virtual bool DecodeEXT(byte instruct)
        {
            // Extended opcode - not used by V3 stories
            return false;
        }

        protected virtual void DecodeVAR2OP(byte instruct)
        {
            // VAR 2OP
            m_opcodeType = ZMachine.Common.OpcodeType.OP_2;

            m_vartype = m_memory.GetByte(PC++);

            if ((m_vartype & 0xC0) == 0x00)
            {
                // Operand 1 is a long constant
                FetchLongConstantOperand();
            }
            else if ((m_vartype & 0xC0) == 0x40)
            {
                // Operand 1 is a small constant
                FetchSmallConstantOperand();
            }
            else if ((m_vartype & 0xC0) == 0x80)
            {
                // Operand 1 is a variable
                FetchVariableOperand();
            }
            m_operandCount++;

            if ((m_vartype & 0x30) == 0x00)
            {
                // Operand 2 is a long constant
                FetchLongConstantOperand();
            }
            else if ((m_vartype & 0x30) == 0x10)
            {
                // Operand 2 is a small constant
                FetchSmallConstantOperand();
            }
            else if ((m_vartype & 0x30) == 0x20)
            {
                // Operand 2 is a variable
                FetchVariableOperand();
            }
            m_operandCount++;

            // Mask the opcode to strip off operand type bits
            m_opcode = (byte)(instruct & 0x1F);
        }

        protected virtual void DecodeVARVAR(byte instruct)
        {
            // VAR VAR
            m_opcodeType = ZMachine.Common.OpcodeType.OP_VAR;

            if (instruct == 0xec || instruct == 0xfa)
            {
                m_vartype = m_memory.GetWord(PC);
                PC += 2;
            }
            else
            {
                // Make the single byte into a full word so that coding
                // is easier.  The LSB of the word needs to be 0xFF
                m_vartype = (m_memory.GetByte(PC++) << 8) | 0xff;
            }

            // Mask the opcode to strip off operand type bits
            // Plus, opcode C1 is actually a OP2, not a OPVAR
            if (instruct == 0xC1)
            {
                m_opcodeType = ZMachine.Common.OpcodeType.OP_2;
                m_opcode = 1;
            }
            else
            {
                m_opcode = (byte)(instruct & 0x1F);
            }

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
                    case 0x03:
                        // End of variables;
                        EOF = true;
                        break;
                }

                if (EOF)
                    break;

                m_operandCount++;
            }
        }
        protected virtual void FetchLongConstantOperand()
        {
            m_operandTypes[m_operandCount] = ZMachine.Common.OperandType.LongConstant;
            m_rawoperands[m_operandCount] = m_memory.GetWord(PC);
            m_operands[m_operandCount] = m_rawoperands[m_operandCount];
            PC += 2;
        }

        protected virtual void FetchSmallConstantOperand()
        {
            m_operandTypes[m_operandCount] = ZMachine.Common.OperandType.SmallConstant;
            m_rawoperands[m_operandCount] = m_memory.GetByte(PC++);
            m_operands[m_operandCount] = m_rawoperands[m_operandCount];
        }

        protected virtual void FetchVariableOperand()
        {
            m_operandTypes[m_operandCount] = ZMachine.Common.OperandType.Variable;
            m_rawoperands[m_operandCount] = m_memory.GetByte(PC++);
            m_operands[m_operandCount] = m_memory.GetVariable(m_rawoperands[m_operandCount]);
        }


        /// <summary>
        /// DEBUGGING/DISASSEMBLER FUNCTION: Gets an appropriate Inform-style name
        /// for the given operand number
        /// </summary>
        /// <param name="o">Operand index number</param>
        /// <returns>String containing Inform-style name (i.e., SP, GC3, #F2, etc)</returns>
        public string DBG_GET_OPERANDNAME(int o)
        {
            if (o >= OperandCount || o < 0)
                return "???";

            switch (m_operandTypes[o])
            {
                case ZMachine.Common.OperandType.LongConstant:
                    return "#" + m_rawoperands[o].ToString("X4");
                case ZMachine.Common.OperandType.SmallConstant:
                    return "#" + m_rawoperands[o].ToString("X2");
                case ZMachine.Common.OperandType.Variable:
                    int op = m_rawoperands[o];
                    if (op == 0)
                        return "SP=" + m_operands[o].ToString("X4");
                    if (op < 0x10)
                        return String.Format("L{0:X2}={1:X4}", op - 1, m_operands[o]);
                    if (op < 0x100)
                        return String.Format("G{0:X2}={1:X4}", op - 16, m_operands[o]);

                    return "V???";
                default:
                    return "???";
            }

        }

        public override string ToString()
        {
            int bytecount = m_length;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:X5}: ", PC - m_length);

            for (int i = 0; i < m_length; i++)
            {
                sb.AppendFormat("{0:X2} ", m_memory.GetByte(PC - m_length + i));
            }

            for (int i = bytecount; i < 12; i++)
            {
                sb.Append("   ");
            }

            sb.Append(" | ");

            sb.Append(DBG_GET_OPCODE());

            sb.Append(" ");

            for (int i = 0; i < m_operandCount; i++)
            {
                sb.Append(DBG_GET_OPERANDNAME(i));
                sb.Append(" ");
            }

            if (OpIsStore())
            {
                sb.Append("-> ");


                if (m_result == 0)
                    sb.Append("SP");
                else if (m_result < 0x10)
                    sb.Append("L" + (m_result - 1).ToString("X2"));
                else
                    sb.Append("G" + (m_result - 16).ToString("X2"));

            }

            if (OpIsBranch())
            {
                if ((m_branch & 0x80) == 0x80)
                    sb.Append("[TRUE] ");
                else
                    sb.Append("[FALSE] ");

                int addr = PC - 2 + m_offset;

                sb.Append(addr.ToString("X5"));
            }
            return sb.ToString();
        }


        private bool OpIsStore()
        {
            int idx = (int)m_opcodeType;

            if (idx < 0 || idx >= Ops.Length)
                throw new ArgumentOutOfRangeException("The current OpcodeType is out of range for the Ops array.");

            return Ops[idx][m_opcode].Store;
        }


        private bool OpIsBranch()
        {
            int idx = (int)m_opcodeType;

            if (idx < 0 || idx >= Ops.Length)
                throw new ArgumentOutOfRangeException("The current OpcodeType is out of range for the Ops array.");

            return Ops[idx][m_opcode].Branch;
        }

        public string DBG_GET_OPCODE()
        {
            int idx = (int)m_opcodeType;

            if (idx < 0 || idx >= Ops.Length)
                throw new ArgumentOutOfRangeException("The current OpcodeType is out of range for the Ops array.");

            return Ops[idx][m_opcode].Name;
        }

        // Jagged array to hold the operation metadata (name, store, branch)
        public ZMachine.Common.ZOperationDef[][] Ops = new ZMachine.Common.ZOperationDef[5][];

        /// <summary>
        /// Initialize the operation metadata.  Deriving classes (for other
        /// ZMachine versions) should override this method.
        /// </summary>
        protected virtual void InitializeOps()
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
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 19
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 1a
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 1b
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 1c
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
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 08
                new ZMachine.Common.ZOperationDef("REMOVE_OBJ",    false, false), // 09
                new ZMachine.Common.ZOperationDef("PRINT_OBJ",     false, false), // 0a
                new ZMachine.Common.ZOperationDef("RET",           false, false), // 0b
                new ZMachine.Common.ZOperationDef("JUMP",          false, false), // 0c
                new ZMachine.Common.ZOperationDef("PRINT_PADDR",   false, false), // 0d
                new ZMachine.Common.ZOperationDef("LOAD",          true, false),  // 0e
                new ZMachine.Common.ZOperationDef("NOT",           true, false)   // 0f
            };

            Ops[(int)ZMachine.Common.OpcodeType.OP_0] = new ZMachine.Common.ZOperationDef[]
            {                   
                new ZMachine.Common.ZOperationDef("RTRUE",         false, false), // 00
                new ZMachine.Common.ZOperationDef("RFALSE",        false, false), // 01
                new ZMachine.Common.ZOperationDef("PRINT",         false, false), // 02
                new ZMachine.Common.ZOperationDef("PRINT_RET",     false, false), // 03
                new ZMachine.Common.ZOperationDef("NOP",           false, false), // 04
                new ZMachine.Common.ZOperationDef("SAVE",          false, true),  // 05
                new ZMachine.Common.ZOperationDef("RESTORE",       false, true),  // 06
                new ZMachine.Common.ZOperationDef("RESTART",       false, false), // 07
                new ZMachine.Common.ZOperationDef("RET_POPPED",    false, false), // 08
                new ZMachine.Common.ZOperationDef("POP",           false, false), // 09
                new ZMachine.Common.ZOperationDef("QUIT",          false, false), // 0a
                new ZMachine.Common.ZOperationDef("NEW_LINE",      false, false), // 0b
                new ZMachine.Common.ZOperationDef("SHOW_STATUS",   false, false), // 0c
                new ZMachine.Common.ZOperationDef("VERIFY",        false, true),  // 0d
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false), // 0e
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, true)   // 0f
            };

            Ops[(int)ZMachine.Common.OpcodeType.OP_VAR] = new ZMachine.Common.ZOperationDef[]
            {   new ZMachine.Common.ZOperationDef("CALL",          true, false),
                new ZMachine.Common.ZOperationDef("STOREW",        false, false),
                new ZMachine.Common.ZOperationDef("STOREB",        false, false),
                new ZMachine.Common.ZOperationDef("PUT_PROP",      false, false),
                new ZMachine.Common.ZOperationDef("SREAD",         false, false),
                new ZMachine.Common.ZOperationDef("PRINT_CHAR",    false, false),
                new ZMachine.Common.ZOperationDef("PRINT_NUM",     false, false),
                new ZMachine.Common.ZOperationDef("RANDOM",        true, false),
                new ZMachine.Common.ZOperationDef("PUSH",          false, false),
                new ZMachine.Common.ZOperationDef("PULL",          false, false),
                new ZMachine.Common.ZOperationDef("SPLIT_WINDOW",  false, false),
                new ZMachine.Common.ZOperationDef("SET_WINDOW",    false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("OUTPUT_STREAM", false, false),
                new ZMachine.Common.ZOperationDef("INPUT_STREAM",  false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false),
                new ZMachine.Common.ZOperationDef("ILLEGAL",       false, false)
            };
            Ops[(int)ZMachine.Common.OpcodeType.OP_EXT] = new ZMachine.Common.ZOperationDef[]
            {
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 00
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 01
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 02
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 03
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 04
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 05
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 06
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 07
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 08
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 09
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 0A
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 0B
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 0C
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 0D
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 0E
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 0F
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 10
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 11
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 12
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 13
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 14
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 15
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 16
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 17
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 18
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 19
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 1A
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false), // 1B
                new ZMachine.Common.ZOperationDef("ILLEGAL",   false, false)  // 1C

            };

        }

    }
}

