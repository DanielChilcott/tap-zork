using System;
using System.Collections.Generic;

namespace ZMachine.Common
{
    /// <summary>
    /// Enumeration of the different Opcode types
    /// </summary>
    public enum OpcodeType : int
    {
        UNKNOWN = -1,
        OP_0 = 2,
        OP_1 = 1,
        OP_2 = 0,
        OP_VAR = 3,
        OP_EXT = 4
    }
}
