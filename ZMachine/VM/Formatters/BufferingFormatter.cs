using System;
using System.Collections.Generic;
using System.Text;

namespace ZMachine.VM.Console.Formatters
{
    [Serializable]
    public class BufferingFormatter : VM.Formatter
    {
        StringBuilder wrappedtext = new StringBuilder();
        string currentline = String.Empty;

        public override void Print(string text)
        {
            currentline += text;

            int lastBreak = 0;

            for (int i = 0; i < currentline.Length; i++)
            {
                if (currentline[i] == ' ')
                {
                    lastBreak = i;
                }
                else if (currentline[i] == '\n')
                {
                    wrappedtext.Append(currentline.Substring(0, i + 1));
                    currentline = currentline.Substring(i + 1);
                    i = 0;
                }
                if (i >= Window.Width - 2)
                {
                    wrappedtext.AppendLine(currentline.Substring(0, lastBreak + 1));
                    currentline = currentline.Substring(lastBreak + 1);
                    i = 0;
                }
            }
        }

        public override void Flush()
        {
            wrappedtext.Append(currentline);

            if (EndOutput != null)
                EndOutput(wrappedtext.ToString());

            wrappedtext = new StringBuilder();
        }
    }
}
