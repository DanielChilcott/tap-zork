// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM.Console
{
    [Serializable]
    public class ConsoleZMachine : VirtualMachine
    {
        protected ConsoleZMachine() { }

        public static ConsoleZMachine Create(byte[] storyBytes)
        {
            var ret = new ConsoleZMachine();
            ret.Load(storyBytes);
            return ret;
        }

        protected override Guid Load(byte[] storyBytes)
        {
            Guid g = base.Load(storyBytes);

            z_io.Screen.LowerWindow = new ConsoleWindow(new ZMachine.VM.Console.Formatters.BufferingFormatter()) { Width = (short)System.Console.WindowWidth, Height = 255 };
            z_io.Screen.UpperWindow = new ConsoleTitleWindow(new ZMachine.VM.Console.Formatters.TextBlockFormatter()) { Width = (short)System.Console.WindowWidth };
            z_io.Reset();
            z_io.Keyboard = new ConsoleKeyboard();

            return g;
        }

    }
}