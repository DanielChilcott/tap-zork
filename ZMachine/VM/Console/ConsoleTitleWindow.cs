// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM.Console
{
    [Serializable]
    public class ConsoleTitleWindow : Window
    {
        public ConsoleTitleWindow(Formatter formatter)
            : base(formatter)
        {
            formatter.EndOutput = t => System.Console.Title = t;
        }
    }
}
