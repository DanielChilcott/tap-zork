// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM.Console.Formatters
{
    [Serializable]
    public class SimpleFormatter : VM.Formatter
    {
        public override void Print(string text)
        {
            if (EndOutput != null)
                EndOutput(text);
        }
    }
}