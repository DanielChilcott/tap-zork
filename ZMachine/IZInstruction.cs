using System;
using System.Collections.Generic;

namespace ZMachine
{
    public interface IZInstruction
    {
        byte Result { get; set; }
        byte Branch { get; set; }
        int Offset { get; set; }
        byte OpCode { get; }
        ZMachine.Common.OpcodeType OpcodeType { get; }
        int OperandCount { get; }
        short[] Operands { get; }
        ZMachine.Common.OperandType[] OperandTypes { get; }
        int Length { get; }
        void Decode(int zPC);
        string DBG_GET_OPERANDNAME(int o);
        string ToString();
        string DBG_GET_OPCODE();
    }
}
