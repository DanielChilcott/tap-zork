// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM
{
    [Serializable]
    public abstract class Screen
    {
        protected Window[] Windows;
        public int CurrentWindowIndex { get; set; }

        public virtual VM.Window LowerWindow
        {
            get { return Windows[0]; }
            set { Windows[0] = value; }
        }

        public virtual VM.Window UpperWindow
        {
            get { return Windows[1]; }
            set { Windows[1] = value; }
        }


        public virtual void BufferMode(int flag)
        {
            if (Windows[0] == null)
                throw new NullReferenceException("Virtual Machine Screen Lower Window does not exist");

            Windows[0].BufferMode(flag);
        }

        public virtual void SetWindow(int zWindow)
        {
            CurrentWindowIndex = zWindow;
        }

        public virtual void ShowStatus()
        {

        }

        public virtual void SplitWindow(int height)
        {
            UpperWindow.Height = (short)height;
            UpperWindow.EraseWindow();
        }

        public virtual void EraseWindow(int zWindow)
        {
            if (Windows[zWindow] == null)
                throw new NullReferenceException(String.Format("Virtual Machine Window {0} does not exist", zWindow));

            Windows[zWindow].EraseWindow();
        }

        public virtual void EraseLine()
        {
            if (Windows[CurrentWindowIndex] == null)
                throw new NullReferenceException(String.Format("Virtual Machine Window {0} does not exist", CurrentWindowIndex));

            Windows[CurrentWindowIndex].EraseLine();
        }

        public virtual void SetColor(int foreground, int background)
        {
            if (Windows[CurrentWindowIndex] == null)
                throw new NullReferenceException(String.Format("Virtual Machine Window {0} does not exist", CurrentWindowIndex));

            Windows[CurrentWindowIndex].SetColor(foreground, background);
        }

        public virtual void SetCursor(short Y, short X)
        {
            if (Windows[CurrentWindowIndex] == null)
                throw new NullReferenceException(String.Format("Virtual Machine Window {0} does not exist", CurrentWindowIndex));

            Windows[CurrentWindowIndex].SetCursor(Y, X);
        }

        public virtual short[] GetCursor()
        {
            if (Windows[CurrentWindowIndex] == null)
                throw new NullReferenceException(String.Format("Virtual Machine Window {0} does not exist", CurrentWindowIndex));

            return new short[] { Windows[CurrentWindowIndex].CurrentY, Windows[CurrentWindowIndex].CurrentX };
        }

        public virtual void SetFont(int zFont)
        {
            if (Windows[CurrentWindowIndex] == null)
                throw new NullReferenceException(String.Format("Virtual Machine Window {0} does not exist", CurrentWindowIndex));

            Windows[CurrentWindowIndex].SetFont(zFont);
        }

        public virtual void SetStyle(int zStyle)
        {
            if (Windows[CurrentWindowIndex] == null)
                throw new NullReferenceException(String.Format("Virtual Machine Window {0} does not exist", CurrentWindowIndex));

            Windows[CurrentWindowIndex].SetStyle(zStyle);
        }

        public virtual void Print(string text)
        {
            if (Windows[CurrentWindowIndex] == null)
                throw new NullReferenceException(String.Format("Virtual Machine Window {0} does not exist", CurrentWindowIndex));

            Windows[CurrentWindowIndex].Print(text);
        }

        public virtual void Reset()
        {

        }
    }
}
