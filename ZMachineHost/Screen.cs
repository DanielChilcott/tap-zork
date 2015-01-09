using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachineHost
{
    [Serializable]
    class Screen : ZMachine.VM.Screen
    {
        public delegate void ScreenPrintDelegate(object sender, string text);
        public event ScreenPrintDelegate ScreenPrint;

        ZMachine.VM.Window _lowerWindow, _upperWindow;

        public override ZMachine.VM.Window LowerWindow
        {
            get
            {
                return _lowerWindow;
            }
            set
            {
                _lowerWindow = value;
            }
        }

        public override ZMachine.VM.Window UpperWindow
        {
            get
            {
                return _upperWindow;
            }
            set
            {
                _upperWindow = value;
            }
        }

        public Screen()
        {
            _upperWindow = new Window(new MyFormatter());
            _lowerWindow = new Window(new MyFormatter());
            //((MyWindow)_upperWindow).WindowPrint += (s, t) => { CommonPrint(s, t); };
            //((MyWindow)_lowerWindow).WindowPrint += (s, t) => { CommonPrint(s, t); };
        }


        public override void Print(string text)
        {
            if (text.Contains("don't"))
            {
            }
            CommonPrint(this, text);
        }

        private void CommonPrint(object source, string text)
        {
            if (ScreenPrint != null)
                ScreenPrint(source, text);



        }
    }
}
