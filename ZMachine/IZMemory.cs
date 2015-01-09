using System;
using System.Collections.Generic;

namespace ZMachine
{
    public interface IZMemory
    {
        IZHeader Header { get; }
        short[] Locals { get; set; }
        
        void LoadStory(byte[] storyBytes);
        byte GetByte(int address);
        short GetWord(int address);
        void PutByte(int address, byte value);
        void PutWord(int address, short value);
        void ClearLocals();
        short PeekStack();
        short GetVariable(int v);
        void PutVariable(int v, short value);

        GameState GetState();

        void SetState(GameState state);
    }
}
