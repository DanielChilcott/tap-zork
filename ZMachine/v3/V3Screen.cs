using System;
using System.Collections.Generic;

namespace ZMachine.v3
{
    [Serializable]
    public class V3Screen : ZMachine.VM.Screen
    {
        protected v3.ZProcessor cpu;

        public V3Screen(v3.ZProcessor zproc)
        {
            cpu = zproc;
            Windows = new VM.Window[2];
        }

        public override VM.Window LowerWindow
        {
            get { return Windows[0]; }
            set { Windows[0] = value; }
        }

        public override VM.Window UpperWindow
        {
            get { return Windows[1]; }
            set { Windows[1] = value; }
        }

        public override void Reset()
        {
            // Set defaults
            cpu.Header.SetFlag1Bit(5, false); // Disable top window
            ColorSupported = true;


            EraseWindow(-1);
        }

        public void EraseWindow(int index)
        {
            if (index == -1)
            {
                Windows[1].Height = 0;
                Windows[1].EraseWindow();
                Windows[0].EraseWindow();
                CurrentWindowIndex = 0;
            }
        }

        private bool _ColorSupported;
        public bool ColorSupported
        {
            get { return _ColorSupported; }
            set
            {
                _ColorSupported = value;
                cpu.Header.SetFlag1Bit(0, value);
            }
        }


    }
}