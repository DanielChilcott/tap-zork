// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.Common 
{
    /// <summary>
    /// ZCallFrame contains the current state information
    /// </summary>
    [Serializable]
    public class ZCallFrame
    {
        // Provide a link to the previous call frame, which is restored after the current routine returns
        private ZCallFrame m_prevCallFrame = null;

        /// <summary>
        /// Constructor.  Initializes the program counter to the value provided.
        /// </summary>
        /// <param name="PC">Program Counter</param>
        public ZCallFrame(int PC)
        {
            this.PC = PC;
        }

        /// <summary>
        /// Local variables array.  Note that this is used during CALL persistence only, and that the active list
        /// of local variables is found in ZMemory.
        /// </summary>
        public short[] Locals { get; set; }

        /// <summary>
        /// Program Counter
        /// </summary>
        public int PC { get; set; }

        /// <summary>
        /// Keeps track of the variable where the result is stored after a routine returns (and this call frame is restored).  
        /// 00H = Stack; 01H-0FH = Locals; 10H-FFH = Globals
        /// </summary>
        public byte? Result { get; set; }

        /// <summary>
        /// Reference to the previous call frame that was in place when this call frame was created.
        /// </summary>
        public ZCallFrame PrevCallFrame
        {
            get { return m_prevCallFrame; }
            set { m_prevCallFrame = value; }
        }

        public int ArgCount { get; set; }
        public int FrameNumber { get; set; }
    }
}
