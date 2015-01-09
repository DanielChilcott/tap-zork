using System;
using System.Collections.Generic;

namespace ZMachine.Common
{
    /// <summary>
    /// Enumeration of the different Operand types
    /// </summary>
    public enum OperandType : int
    {
        UNKNOWN = 0,
        SmallConstant,
        LongConstant,
        Variable
    }
}
