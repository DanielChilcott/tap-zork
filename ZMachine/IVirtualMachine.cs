using System;
using System.Collections.Generic;

namespace ZMachine
{
    public interface IVirtualMachine
    {
        void Start();
        bool IsRunning { get; }
    }
}
