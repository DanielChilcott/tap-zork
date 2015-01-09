using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachineHost
{
    [Serializable]
    class Keyboard : ZMachine.VM.Keyboard
    {
        public string NextString { get; set; }

        public override string ReadString(int? time)
        {
            try
            {
                //Console.WriteLine(NextString);
                return NextString;
            }
            finally
            {
                NextString = "";
            }

        }

        public override char ReadChar(int? time)
        {
            return base.ReadChar(time);
        }
    }
}
