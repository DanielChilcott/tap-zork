using System;
using System.Collections.Generic;

namespace ZMachine
{
    public interface IZHeader
    {
        byte VersionNumber { get; }
        byte Flags1 { get; set; }
        int HighMemoryBase { get; }
        int InitialPC { get; }
        int DictionaryLocation { get; }
        int ObjectTableLocation { get; }
        int GlobalVarTableLocation { get; }
        int StaticMemoryBase { get; }
        byte Flags2 { get; set; }
        int AbbreviationTableLocation { get; }
        int StandardRevisionNumber { get; }
    }
}
