// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.Common
{
    /// <summary>
    /// Base class used for decoding header data
    /// </summary>
    [Serializable]
    public abstract class ZHeaderBase : IZHeader
    {
        protected IZMemory m_memory;
        protected ZHeaderBase(IZMemory mem)
        {
            m_memory = mem;
        }

        public virtual byte VersionNumber
        {
            get { return m_memory.GetByte(0x0); }
        }

        public virtual byte Flags1
        {
            get { return m_memory.GetByte(0x1); }
            set { m_memory.PutByte(0x1, value); }
        }

        public virtual int HighMemoryBase
        {
            get { return m_memory.GetWord(0x4); }
        }

        public virtual int InitialPC
        {
            get { return m_memory.GetWord(0x6); }
        }

        public virtual int DictionaryLocation
        {
            get { return m_memory.GetWord(0x8); }
        }

        public virtual int ObjectTableLocation
        {
            get { return m_memory.GetWord(0xa); }
        }

        public virtual int GlobalVarTableLocation
        {
            get { return m_memory.GetWord(0xc); }
        }

        public virtual int StaticMemoryBase
        {
            get { return m_memory.GetWord(0xe); }
        }

        public virtual byte Flags2
        {
            get { return m_memory.GetByte(0x10); }
            set { m_memory.PutByte(0x10, value); }
        }

        public virtual int AbbreviationTableLocation
        {
            get { return m_memory.GetWord(0x18); }
        }

        public virtual int StandardRevisionNumber
        {
            get { return m_memory.GetWord(0x32); }
        }


        public void SetFlag1Bit(int bitNumber, bool value)
        {
            if (bitNumber < 0 || bitNumber > 7)
                throw new ArgumentOutOfRangeException("bitNumber", bitNumber, "Valid bit numbers range from 0 to 7");

            var v = Flags1;
            if (value)
                v |= (byte)((1 << bitNumber) & 0xff);
            Flags1 = v;
        }

        public bool GetFlag1Bit(int bitNumber)
        {
            if (bitNumber < 0 || bitNumber > 7)
                throw new ArgumentOutOfRangeException("bitNumber", bitNumber, "Valid bit numbers range from 0 to 7");

            var test = (byte)((1 << bitNumber) & 0xff);
            return (Flags1 & test) == test;
        }

        public void SetFlag2Bit(int bitNumber, bool value)
        {
            if (bitNumber < 0 || bitNumber > 7)
                throw new ArgumentOutOfRangeException("bitNumber", bitNumber, "Valid bit numbers range from 0 to 7");

            var v = Flags2;
            if (value)
                v |= (byte)((1 << bitNumber) & 0xff);
            Flags2 = v;
        }

        public bool GetFlag2Bit(int bitNumber)
        {
            if (bitNumber < 0 || bitNumber > 7)
                throw new ArgumentOutOfRangeException("bitNumber", bitNumber, "Valid bit numbers range from 0 to 7");

            var test = (byte)((1 << bitNumber) & 0xff);
            return (Flags2 & test) == test;
        }


    }
}
