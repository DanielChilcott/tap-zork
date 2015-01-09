// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com)
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.v5
{
    [Serializable]
    public class ZHeader : v3.ZHeader
    {
        public ZHeader(ZMemory mem)  : base(mem)
        {
            
        }

        public virtual byte InterpreterNumber           // Version 4
        {
            get { return m_memory.GetByte(0x1e); }
            set { m_memory.PutByte(0x1e, value); }
        }

        public virtual byte InterpreterVersion          // Version 4
        {
            get { return m_memory.GetByte(0x1f); }
            set { m_memory.PutByte(0x1f, value); }
        }

        public virtual byte ScreenHeightLines           // Version 4
        {
            get { return m_memory.GetByte(0x20); }
            set { m_memory.PutByte(0x20, value); }
        }

        public virtual byte ScreenWidthChars            // Version 4
        {
            get { return m_memory.GetByte(0x21); }
            set { m_memory.PutByte(0x21, value); }
        }

        public virtual int ScreenWidthInUnits
        {
            get { return m_memory.GetWord(0x22); }
            set { m_memory.PutWord(0x22, (short)value); }
        }

        public virtual int ScreenHeightInUnits
        {
            get { return m_memory.GetWord(0x24); }
            set { m_memory.PutWord(0x24, (short)value); }
        }

        public virtual byte FontWidthInUnits
        {
            get { return m_memory.GetByte(0x26); }
            set { m_memory.PutByte(0x26, value); }
        }

        public virtual byte FontHeightInUnits
        {
            get { return m_memory.GetByte(0x27); }
            set { m_memory.PutByte(0x27, value); }
        }

        public virtual byte DefaultBackgroundColor
        {
            get { return m_memory.GetByte(0x2c); }
            set { m_memory.PutByte(0x2c, value); }
        }

        public virtual byte DefaultForegroundColor
        {
            get { return m_memory.GetByte(0x2d); }
            set { m_memory.PutByte(0x2d, value); }
        }

        public virtual int TerminatingCharsTableAddr
        {
            get { return m_memory.GetWord(0x2e); }
        }

        public virtual int AlphabetCharsTableAddr
        {
            get { return m_memory.GetWord(0x34); }
        }

        public virtual int HdrExtensionTableAddr
        {
            get { return m_memory.GetWord(0x36); }
        }
    }
}
