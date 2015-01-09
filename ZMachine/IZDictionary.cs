using System;
using System.Collections.Generic;

namespace ZMachine
{
    public interface IZDictionary
    {
        int GetAddressForEntry(string entry);
        string GetWordSeparatorsAsString();
    }
}
