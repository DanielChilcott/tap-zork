using System;
using System.Collections.Generic;

namespace ZMachine.v5
{
    [Serializable]
    public class ZObjectTable : Common.ZObjectTableCore
    {
        public ZObjectTable(IZMemory mem)
        {
            // V4+ constants

            DEFAULTSIZE = 126; // 63 words = 126 bytes
            OBJATTRSIZE = 6;
            OBJHANDLESIZE = 2;

            objEntrySize = (OBJATTRSIZE + (3 * OBJHANDLESIZE) + 2);

            m_memory = mem;
        }
    }
}


