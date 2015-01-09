// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM.Console
{
    [Serializable]
    public class ConsoleKeyboard : Keyboard
    {
        public override char ReadChar(int? time)
        {
            return (char)System.Console.Read();
        }

        public override string ReadString(int? time)
        {
            return System.Console.ReadLine();
        }
    }
}