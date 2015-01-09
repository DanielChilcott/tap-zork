// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;
using System.Text;

// NOTE: Some of this code is a port of the ZAX zmachine ZCPU code
// as released in 1997 by Matthew E. Kimmel (unable to find recent contact info)

namespace ZMachine.Common
{
    public static class ZText
    {
        private static readonly string[] Alpha = new string[]  
        {   " \0\0\0\0\0abcdefghijklmnopqrstuvwxyz",
            " \0\0\0\0\0ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            " \0\0\0\0\0\0\n0123456789.,!?_#\'\"/\\-:()"
        };

        internal static string DecodeZString(Queue<byte> zBytes, IZMemory z_memory)
        {
            var sb = new StringBuilder();

            int currentAlphabet = 0;
            int lockAlphabet = 0;

            char c, c2, c3;
            int abbrevAddr = 0;

            while (zBytes.Count > 0)
            {
                c = (char)zBytes.Dequeue();

                switch ((int)c)
                {
                    case 1:
                    case 2:
                    case 3:
                        // Abbreviation in V2+
                        if (zBytes.Count == 0) // This is all we're getting.
                            break;

                        c2 = (char)zBytes.Dequeue();

                        abbrevAddr = z_memory.GetWord(z_memory.Header.AbbreviationTableLocation + (((((int)c) - 1) * 32 + ((int)c2)) * 2));
                        abbrevAddr *= 2; // Word address
                        sb.Append(PrintZString(z_memory, abbrevAddr));

                        break;

                    case 4: // Always a shift up
                        if (currentAlphabet == 2)
                            currentAlphabet = 0;
                        else
                            currentAlphabet++;
                        break;
                    case 5: // Always a shift down
                        if (currentAlphabet == 0)
                            currentAlphabet = 2;
                        else if (currentAlphabet == 2)
                            currentAlphabet = 1;
                        else
                            currentAlphabet = 0;
                        break;
                    case 6: // Literal output character if alphabet is P.
                        if (currentAlphabet == 2)
                        {
                            if (zBytes.Count == 0)
                                break;
                            c2 = (char)zBytes.Dequeue();

                            if (zBytes.Count == 0)
                                break;
                            c3 = (char)zBytes.Dequeue();

                            int w = ((((int)c2 << 5) & 0x03e0) | ((int)c3 & 0x1f));
                            sb.Append((char)w);
                            currentAlphabet = lockAlphabet;
                        }
                        else
                        {
                            sb.Append(Alpha[currentAlphabet][(int)c]);
                            currentAlphabet = lockAlphabet;
                        }
                        break;
                    default:
                        sb.Append(Alpha[currentAlphabet][(int)c]);
                        currentAlphabet = lockAlphabet;
                        break;
                }
            }

            // We're done!
            return sb.ToString();
        }

        public static string PrintZString(IZMemory z_memory, int addr)
        {
            int tmp = 0;

            Queue<byte> zBytes = new Queue<byte>();

            while ((tmp & 0x8000) == 0)
            {
                tmp = z_memory.GetWord(addr);
                addr += 2;

                zBytes.Enqueue((byte)((tmp >> 10) & 0x1f));
                zBytes.Enqueue((byte)((tmp >> 5) & 0x1f));
                zBytes.Enqueue((byte)(tmp & 0x1f));
            }

            string ret = ZMachine.Common.ZText.DecodeZString(zBytes, z_memory);

            zBytes = null;
            return ret;
        }
    }
}
