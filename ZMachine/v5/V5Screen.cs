using System;
using System.Collections.Generic;

namespace ZMachine.v5
{
    [Serializable]
    public class V5Screen : ZMachine.VM.Screen
    {
        protected v5.ZProcessor cpu;

        public V5Screen(v5.ZProcessor zproc)
        {
            cpu = zproc;
            Windows = new VM.Window[2];
        }

        public VM.Window LowerWindow
        {
            get { return Windows[0]; }
            set { Windows[0] = value; }
        }

        public VM.Window UpperWindow
        {
            get { return Windows[1]; }
            set { Windows[1] = value; }
        }

        public override void Reset()
        {
            // Set defaults
            cpu.Header.SetFlag2Bit(3, false); // Character Graphic font not supported
            ColorSupported = true;
            DefaultBackgroundColor = VM.Color.Black;
            DefaultForegroundColor = VM.Color.White;
            ScreenHeightLines = 255; // infinite (scrolling handled by UI)
            ScreenWidthChars = 80;
            ScreenHeightInUnits = 255;
            ScreenWidthInUnits = 80;
            FontHeightInUnits = 1;
            FontWidthInUnits = 1;
            
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
        public VM.Color DefaultBackgroundColor
        {
            get { return (VM.Color)cpu.Header.DefaultBackgroundColor; }
            set { cpu.Header.DefaultBackgroundColor = (byte)value; }
        }

        public VM.Color DefaultForegroundColor
        {
            get { return (VM.Color)cpu.Header.DefaultForegroundColor; }
            set { cpu.Header.DefaultForegroundColor = (byte)value; }
        }

        public byte ScreenWidthChars
        {
            get { return cpu.Header.ScreenWidthChars; }
            set { cpu.Header.ScreenWidthChars = value; }
        }

        public byte ScreenHeightLines
        {
            get { return cpu.Header.ScreenHeightLines; }
            set { cpu.Header.ScreenHeightLines = value; }
        }

        public int ScreenWidthInUnits
        {
            get { return cpu.Header.ScreenWidthInUnits; }
            set { cpu.Header.ScreenWidthInUnits = value; }
        }

        public int ScreenHeightInUnits
        {
            get { return cpu.Header.ScreenHeightInUnits; }
            set { cpu.Header.ScreenHeightInUnits = value; }
        }

        public byte FontWidthInUnits
        {
            get { return cpu.Header.FontWidthInUnits; }
            set { cpu.Header.FontWidthInUnits = value; }
        }

        public byte FontHeightInUnits
        {
            get { return cpu.Header.FontHeightInUnits; }
            set { cpu.Header.FontHeightInUnits = value; }
        }
    }
}