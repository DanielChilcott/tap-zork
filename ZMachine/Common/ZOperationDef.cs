// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.Common
{
    [Serializable]
    public class ZOperationDef
    {
        public ZOperationDef(string name, bool store, bool branch)
        {
            Name = name;
            Store = store;
            Branch = branch;
        }

        public string Name { get; set; }
        public bool Store { get; set; }
        public bool Branch { get; set; }
    }
}
