// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.v3
{
    [Serializable]
    public class ZObjectTable : Common.ZObjectTableCore
    {
        public ZObjectTable(IZMemory mem)
        {
            // V3 constants

            DEFAULTSIZE = 62; // 31 words = 62 bytes
            OBJATTRSIZE = 4;
            OBJHANDLESIZE = 1;

            objEntrySize = (OBJATTRSIZE + (3 * OBJHANDLESIZE) + 2);

            m_memory = mem;
        }
    }

}


