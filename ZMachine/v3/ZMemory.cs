// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.v3
{
    

    /// <summary>
    /// Represents the memory space of the ZMachine.
    /// </summary>
    [Serializable]
    public class ZMemory : IZMemory
    {
        // The raw memory bytes.
        protected byte[] m_bytes;

        // ZHeader helper class (used for decoding header data)
        protected IZHeader m_header;

        // The "live" local variables for the current call state
        protected short[] m_locals = new short[16];

        // The ZMachine's stack.  Note that this is not a safe stack in the
        // sense that my quick-and-dirty code will allow a routine to pop
        // more values than the routine actually pushed (which is a no-no, but
        // this version is just demonstrative code, so fix it later somehow)
        protected Stack<short> m_stack = new Stack<short>();

        // ZHeader helper object to decode header info.
        public IZHeader Header
        {
            get { return m_header; }
        }

        public short[] Locals
        {
            get
            {
                return m_locals;
            }
            set
            {
                if (value.Length != 16)
                    throw new ArgumentException("Locals array must be of length 16");
                else
                    m_locals = value;
            }
        }

        /// <summary>
        /// Initializes the memory with the provided byte array
        /// </summary>
        /// <param name="storyBytes">Byte array containing the story file bytes</param>
        public virtual void LoadStory(byte[] storyBytes)
        {
            m_bytes = storyBytes;
            m_header = new ZHeader(this);
        }


        /// <summary>
        /// Get one byte from the given address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public byte GetByte(int address)
        {
            // Something weird with signed integers, etc...  This hack works
            address &= 0x1ffff;
            if (address > m_bytes.GetUpperBound(0))
                address &= 0xffff;
            return m_bytes[address];
        }

        /// <summary>
        /// Gets a word from the given address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public short GetWord(int address)
        {
            // Something weird with signed integers, etc...  This hack works
            address &= 0x1ffff;
            if (address > m_bytes.GetUpperBound(0))
                address &= 0xffff;
            return (short)((m_bytes[address] << 8) + m_bytes[address + 1]);
        }

        /// <summary>
        /// Writes a byte to the given address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        public void PutByte(int address, byte value)
        {
            // Something weird with signed integers, etc...  This hack works
            address &= 0x1ffff;
            if (address > m_bytes.GetUpperBound(0))
                address &= 0xffff;
            m_bytes[address] = value;
        }

        /// <summary>
        /// Writes a word to the given address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="value"></param>
        public void PutWord(int address, short value)
        {
            // Something weird with signed integers, etc...  This hack works
            address &= 0x1ffff;
            if (address > m_bytes.GetUpperBound(0))
                address &= 0xffff;
            m_bytes[address] = (byte)((value >> 8) & 0xff);
            m_bytes[address + 1] = (byte)(value & 0xff);
        }

        /// <summary>
        /// Clears the local variables
        /// </summary>
        public void ClearLocals()
        {
            m_locals = new short[16];
        }

        /// <summary>
        /// Used for seeing what the next value to be popped will be without 
        /// actually popping it
        /// </summary>
        /// <returns></returns>
        public short PeekStack()
        {
            if (m_stack.Count > 0)
                return m_stack.Peek();
            else
                return short.MinValue;
        }

        /// <summary>
        /// Gets the value of a variable
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public short GetVariable(int v)
        {
            if (v == 0)
            {
                // The top of the routine stack
                if (m_stack.Count == 0)
                    throw new Exception("Routine stack underflow");
                else
                {
                    return m_stack.Pop();
                }
            }
            else if (v < 16) // Local variable
                return (m_locals[v]);
            else if (v <= 255) // Global variable
                return (GetWord(m_header.GlobalVarTableLocation + ((v - 16) * 2)));

            return 0;
        }

        /// <summary>
        /// Writes a value to a variable
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        public void PutVariable(int v, short value)
        {
            if (v == 0)
            {
                // Push this value onto the routine stack
                m_stack.Push(value);
            }
            else if (v < 16) // Local variable
                m_locals[v] = value;
            else if (v <= 255) // Global variable
                PutWord(m_header.GlobalVarTableLocation + ((v - 16) * 2), value);
            else
                return;
        }


        public GameState GetState()
        {
            return new GameState()
            {
                MemoryBytes = m_bytes,
                Locals = m_locals,
                Stack  = m_stack.ToArray()
            };
        }

        public void SetState(GameState state)
        {
            m_bytes = state.MemoryBytes;
            m_locals = state.Locals;

            Array.Reverse(state.Stack);

            m_stack = new Stack<short>(state.Stack);
        }
    }
}
