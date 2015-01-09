using System;
using System.Collections.Generic;

namespace ZMachine.VM
{
    [Serializable]
    public abstract class Keyboard
    {
        public virtual char ReadChar(int? time)
        {
            throw new NotImplementedException();
        }

        public virtual string ReadString(int? time)
        {
            throw new NotImplementedException();
        }

    }
}
