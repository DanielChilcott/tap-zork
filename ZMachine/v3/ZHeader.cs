// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace ZMachine.v3
{
    [Serializable]
    public class ZHeader : ZMachine.Common.ZHeaderBase
    {
        public ZHeader(ZMemory mem) : base(mem) { }

        public virtual int FileLength
        {
            get { return m_memory.GetWord(0x1a) << 1; }
        }

        public virtual int FileChecksum
        {
            get { return m_memory.GetWord(0x1c); }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Z-code version: {0}", VersionNumber.ToString());
            sb.AppendFormat("\nSize of dynamic memory: {0}", StaticMemoryBase.ToString("x"));
            sb.AppendFormat("\nSize of resident memory: {0}", HighMemoryBase.ToString("x"));
            sb.AppendFormat("\nStart PC: {0}", InitialPC.ToString("x"));
            sb.AppendFormat("\nDictionary Address: {0}", DictionaryLocation.ToString("x"));
            sb.AppendFormat("\nObject Table Address: {0}", ObjectTableLocation.ToString("x"));
            sb.AppendFormat("\nAbbreviation address: {0}", AbbreviationTableLocation.ToString("x"));
            sb.AppendFormat("\nFile Size: {0}", FileLength.ToString("x"));
            sb.AppendFormat("\nChecksum: {0}", FileChecksum.ToString("x"));
            sb.AppendFormat("\nStandardRevision: {0}", StandardRevisionNumber.ToString("x"));
            sb.AppendFormat("\nGlobals: {0}", GlobalVarTableLocation.ToString("x"));

            return sb.ToString();
        }
    }
}
