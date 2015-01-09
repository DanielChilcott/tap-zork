// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM.Console.Formatters
{
    [Serializable]
    public class TextBlockFormatter : VM.Formatter
    {
        char[] block;

        public override void EraseAll()
        {
            block = new string(' ', (Window.Height * Window.Width)).ToCharArray();
        }

        public override void Print(string text)
        {
            if (block == null || (block.Length == 0 && Window.Height > 0))
                EraseAll();

            if (block.Length == 0)
                return;

            int pos = (Window.CurrentY - 1) * Window.Width + Window.CurrentX - 1;
            
            foreach(char c in text.ToCharArray())
            {
                block[pos] = c;
                if (pos < block.Length - 1)
                    pos++;
            }
            Window.CurrentY = (short)((pos / Window.Width) + 1);
            Window.CurrentX = (short)((pos % Window.Width) + 1);

            if (EndOutput != null)
                EndOutput(new string(block));
        }
    }
}
