using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachine
{
    [Serializable]
    public class GameState
    {
        public byte[] MemoryBytes { get; set; }

        public short[] Locals { get; set; }

        public short[] Stack { get; set; }
    }
}
