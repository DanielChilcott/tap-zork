// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM.Console
{
    [Serializable]
    public class ConsoleWindow : Window
    {
        public ConsoleWindow(Formatter formatter) : base(formatter)
        {
            formatter.EndOutput = t => System.Console.Write(t);
        }

        public override void EraseWindow()
        {
            base.EraseWindow();
            System.Console.Clear();
        }

        public override void SetColor(int foreground, int background)
        {
            base.SetColor(foreground, background);

            System.Console.BackgroundColor = ConvertColor(CurrentBackgroundColor);
            System.Console.ForegroundColor = ConvertColor(CurrentForegroundColor);
        }

        private ConsoleColor ConvertColor(VM.Color zColor)
        {
            switch (zColor)
            {
                case Color.Black:
                    return ConsoleColor.Black;
                case Color.Red:
                    return ConsoleColor.Red;
                case Color.Green:
                    return ConsoleColor.Green;
                case Color.Yellow:
                    return ConsoleColor.Yellow;
                case Color.Blue:
                    return ConsoleColor.Blue;
                case Color.Magenta:
                    return ConsoleColor.Magenta;
                case Color.Cyan:
                    return ConsoleColor.Cyan;
                case Color.White:
                    return ConsoleColor.White;
                case Color.DarkishGray:
                    return ConsoleColor.DarkGray;
                default:
                    return ConsoleColor.Black;
            }
        }
    }
}
