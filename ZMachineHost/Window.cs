using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachineHost
{
    [Serializable]
    class Window : ZMachine.VM.Window
    {

        public delegate void WindowPrintDelegate(object sender, string text);
        public event WindowPrintDelegate WindowPrint;

        public Window(ZMachine.VM.Formatter formatter)
            : base(formatter)
        {
        }

        public override void Print(string text)
        {
            if (WindowPrint != null)
                WindowPrint(this, text);
        }
    }
}
