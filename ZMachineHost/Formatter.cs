using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachineHost
{
    [Serializable]
    class MyFormatter : ZMachine.VM.Formatter
    {
        public override void Print(string text)
        {
            base.Print(text);
        }
    }
}
