using System;
using System.Collections.Generic;

namespace ZMachine.v5
{
    [Serializable]
    public class ZMemory : v3.ZMemory
    {

        public override void LoadStory(byte[] storyBytes)
        {
            m_bytes = storyBytes;
            m_header = new v5.ZHeader(this);
        }
    }
}
