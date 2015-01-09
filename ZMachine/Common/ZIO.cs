// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.Common
{
    [Serializable]
	public class ZIO
	{
        protected IZProcessor cpu;
        protected ZMachine.VM.Screen screen;
        protected ZMachine.VM.Keyboard keyboard;
        protected ZMachine.VM.Sound sound;

        public bool Stream1Active { get; set; }
        public bool Stream2Active { get; set; }
        public bool Stream3Active { get; set; }
        public bool Stream4Active { get; set; }

        public ZIO(IZProcessor cpu)
        {
            this.cpu = cpu;
        }

        public ZMachine.VM.Screen Screen
        {
            get { return screen; }
            set
            {
                if (screen == value)
                    return;
                screen = value;
            }
        }
        public ZMachine.VM.Keyboard Keyboard
        {
            get { return keyboard; }
            set
            {
                if (keyboard == value)
                    return;
                keyboard = value;
            }
        }

        public ZMachine.VM.Sound Sound
        {
            get { return sound; }
            set
            {
                if (sound == value)
                    return;
                sound = value;
            }
        }

        public void SetOutputStream(int zStream)
        {
            switch (Math.Abs(zStream))
            {
                case 1:
                    Stream1Active = (Math.Sign(zStream) == 1);
                    break;
                case 2:
                    Stream1Active = (Math.Sign(zStream) == 1);
                    break;
                case 3:
                    Stream1Active = (Math.Sign(zStream) == 1);
                    break;
                case 4:
                    Stream1Active = (Math.Sign(zStream) == 1);
                    break;
                case 0:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Valid streams range from 1 to 4");
            }
        }

        public char ReadChar(int? time)
        {
            if (screen != null && screen.LowerWindow != null)
                screen.LowerWindow.ForceFlush();

            if (keyboard == null)
                throw new NullReferenceException("Virtual Machine Keyboard is not defined");

            return keyboard.ReadChar(time);
        }

        public string ReadString(int? time)
        {
            if (screen != null && screen.LowerWindow != null)
                screen.LowerWindow.ForceFlush(); 
            
            if (keyboard == null)
                throw new NullReferenceException("Virtual Machine Keyboard is not defined"); 
            
            return keyboard.ReadString(time);
        }

        public void SoundEffect()
        {
            if (sound != null)
                sound.SoundEffect();   
        }

        public void BufferMode(int flag)
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.BufferMode(flag);
        }

        public void Reset()
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined"); 
            
            screen.Reset();
        }

        public void SetWindow(int zWindow)
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined"); 
            
            screen.SetWindow(zWindow);
        }

        public void ShowStatus()
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.ShowStatus();
        }

        public void SplitWindow(int height)
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.SplitWindow(height);
        }

        public void EraseWindow(int zWindow)
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.EraseWindow(zWindow);
        }

        public void EraseLine()
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.EraseLine();
        }

        public void SetColor(int foreground, int background)
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.SetColor(foreground, background);
        }

        public void SetCursor(short Y, short X)
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.SetCursor(Y, X);
        }

        public void SetFont(int zFont)
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.SetFont(zFont);
        }

        public void SetStyle(int zStyle)
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.SetStyle(zStyle);
        }

        public void Print(string text)
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            screen.Print(text);
        }

        public short[] GetCursor()
        {
            if (screen == null)
                throw new NullReferenceException("Virtual Machine Screen is not defined");

            return screen.GetCursor();
        }
	}
}