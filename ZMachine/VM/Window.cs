// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM
{
    [Serializable]
    public abstract class Window
    {
        protected ZMachine.VM.Formatter formatter = null;

        public Window(ZMachine.VM.Formatter formatter)
        {
            this.formatter = formatter;
            formatter.Window = this;
        }

        public virtual void EraseWindow()
        {
            if (formatter == null)
                throw new NullReferenceException("Virtual Machine Window Formatter does not exist");

            formatter.EraseAll();
        }

        public virtual void BufferMode(int flag)
        {
            if (formatter == null)
                throw new NullReferenceException("Virtual Machine Window Formatter does not exist");

            formatter.BufferMode = ((flag & 0x01) == 0x01);
        }

        public virtual void EraseLine()
        {
            if (formatter == null)
                throw new NullReferenceException("Virtual Machine Window Formatter does not exist"); 
            
            formatter.EraseLine();
        }

        public virtual void SetColor(int foreground, int background)
        {
            CurrentForegroundColor = (VM.Color)foreground;
            CurrentBackgroundColor = (VM.Color)background;
        }

        public virtual void SetCursor(short Y, short X)
        {
            CurrentX = X;
            CurrentY = Y;
        }

        public virtual void SetFont(int zFont)
        {
            CurrentFont = (VM.Font)zFont;
        }

        public virtual void SetStyle(int zStyle)
        {
            CurrentStyle = (VM.Style)zStyle;
        }

        public virtual void Print(string text)
        {
            if (formatter == null)
                throw new NullReferenceException("Virtual Machine Window Formatter does not exist");

            formatter.Print(text);            
        }

        public virtual void ForceFlush()
        {
            if (formatter == null)
                throw new NullReferenceException("Virtual Machine Window Formatter does not exist");

            formatter.Flush();
        }

        public VM.Style CurrentStyle { get; set; }
        public VM.Font CurrentFont { get; set; }
        public VM.Color CurrentForegroundColor { get; set; }
        public Color CurrentBackgroundColor { get; set; }
        public short CurrentX { get; set; }
        public short CurrentY { get; set; }
        public short Height { get; set; }
        public short Width { get; set; }
    }
}
