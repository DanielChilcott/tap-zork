// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM
{
    [Serializable]
    public abstract class Formatter
    {
        public bool BufferMode { get; set; }

        public virtual void EraseLine()
        {

        }

        public virtual void EraseAll()
        {

        }

        public virtual void Print(string text)
        {

        }

        public virtual void Flush()
        {
            
        }

        public Window Window { get; set; }

        public delegate void DisplayOutputHandler(string text);
        public DisplayOutputHandler EndOutput;

    }
}
