// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.Common
{
    partial class ZProcessorCore
    {
        // NOTE: ALL Opcode Handlers for all versions are defined here



        /// <summary>
        /// The Generic "ILLEGAL" function that is used by the function pointer
        /// arrays to dispatch illegal opcodes to.  
        /// </summary>
        /// <param name="inst"></param>
        protected virtual void ILLEGAL(IZInstruction inst)
        {
            throw new NotImplementedException(String.Format("Opcode not implemented in this version:\n\n{0}:{1:X2}", Enum.GetName(typeof(ZMachine.Common.OpcodeType), inst.OpcodeType), inst.OpCode));
        }






        protected virtual void OP_JE(IZInstruction inst)
        {
            bool conditionMet = false;

            for (int i = 1; i < inst.OperandCount; i++)
            {
                if (inst.Operands[0] == inst.Operands[i])
                {
                    conditionMet = true;
                    break;
                }
            }

            Branch(conditionMet, inst);
        }

        protected virtual void Branch(bool conditionMet, IZInstruction inst)
        {
            byte branch = inst.Branch;
            int offset = inst.Offset;

            if (conditionMet)
            {
                // if bit 7 of branch byte is 0, branch on false, else branch on true
                if ((branch & 0x80) == 0x80)
                {
#if (VERBOSEDEBUG)
                    System.Diagnostics.Debug.Write(" [TRUE=TRUE: ");
                    if (offset > 1)
                        System.Diagnostics.Debug.Write((z_frame.PC + offset - 2).ToString("X"));
                    else
                        System.Diagnostics.Debug.Write(offset.ToString("X"));
                    System.Diagnostics.Debug.WriteLine("] ");
#endif

                    // offset 0 means return false; offset 1 means return true
                    if (offset == 0)
                    {
                        Return(0);
                        return;
                    }
                    else if (offset == 1)
                    {
                        Return(1);
                        return;
                    }

                    // branch:  PC = PC + Offset - 2
                    z_frame.PC += offset - 2;

                    // Handle twos-compliment negative
                    if ((offset & 0x2000) == 0x2000)
                    {
                        z_frame.PC -= 0x4000;
                    }

                }
                return;
            }

            // if bit 7 of branch byte is 0, branch on false, else branch on true
            if ((branch & 0x80) == 0)
            {
#if (VERBOSEDEBUG)
                System.Diagnostics.Debug.Write(" [FALSE=FALSE: ");
                if (offset > 1)
                    System.Diagnostics.Debug.Write((z_frame.PC + offset - 2).ToString("X"));
                else
                    System.Diagnostics.Debug.Write(offset.ToString("X"));
                System.Diagnostics.Debug.WriteLine("] ");
#endif

                // offset 0 means return false; offset 1 means return true
                if (offset == 0)
                {
                    Return(0);
                    return;
                }
                else if (offset == 1)
                {
                    Return(1);
                    return;
                }

                // branch:  PC = PC + Offset - 2
                z_frame.PC += offset - 2;

                // Handle twos-compliment negative with weird bitlength number
                if ((offset & 0x2000) == 0x2000)
                {
                    z_frame.PC -= 0x4000;
                }

            }
            return;
        }

        protected virtual void OP_JL(IZInstruction inst)
        {
            bool conditionMet = false;

            for (int i = 1; i < inst.OperandCount; i++)
            {
                if (inst.Operands[0] < inst.Operands[i])
                {
                    conditionMet = true;
                    break;
                }
            }

            Branch(conditionMet, inst);
        }

        protected virtual void OP_JG(IZInstruction inst)
        {
            bool conditionMet = false;

            for (int i = 1; i < inst.OperandCount; i++)
            {
                if (inst.Operands[0] > inst.Operands[i])
                {
                    conditionMet = true;
                    break;
                }
            }

            Branch(conditionMet, inst);
        }

        protected virtual void OP_DEC_CHK(IZInstruction inst)
        {
            short v = (z_memory.GetVariable(inst.Operands[0]));
            v--;
            z_memory.PutVariable(inst.Operands[0], v);

            Branch((v < inst.Operands[1]), inst);
        }

        protected virtual void OP_INC_CHK(IZInstruction inst)
        {
            short v = (z_memory.GetVariable(inst.Operands[0]));
            v++;
            z_memory.PutVariable(inst.Operands[0], v);

            Branch((v > inst.Operands[1]), inst);
        }

        protected virtual void OP_JIN(IZInstruction inst)
        {
            if (z_memory.Header.VersionNumber < 4)
                Branch(z_objtable.getParent((byte)(inst.Operands[0] & 0xff)) == (byte)(inst.Operands[1] & 0xff), inst);
            else
                Branch(z_objtable.getParent(inst.Operands[0]) == inst.Operands[1], inst);
        }

        protected virtual void OP_TEST(IZInstruction inst)
        {
            Branch((inst.Operands[0] & inst.Operands[1]) == inst.Operands[1], inst);
        }

        protected virtual void OP_OR(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, (short)(inst.Operands[0] | inst.Operands[1]));
        }

        protected virtual void OP_AND(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, (short)(inst.Operands[0] & inst.Operands[1]));
        }

        protected virtual void OP_TEST_ATTR(IZInstruction inst)
        {
            Branch(z_objtable.hasAttribute(inst.Operands[0], inst.Operands[1]), inst);
        }

        protected virtual void OP_SET_ATTR(IZInstruction inst)
        {
            z_objtable.setAttribute(inst.Operands[0], inst.Operands[1]);
        }

        protected virtual void OP_CLEAR_ATTR(IZInstruction inst)
        {
            z_objtable.clearAttribute(inst.Operands[0], inst.Operands[1]);
        }

        protected virtual void OP_STORE(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Operands[0], inst.Operands[1]);
        }
        protected virtual void OP_INSERT_OBJ(IZInstruction inst)
        {
            if (z_memory.Header.VersionNumber < 4)
                z_objtable.insertObject((byte)inst.Operands[0], (byte)inst.Operands[1]);
            else
                z_objtable.insertObject(inst.Operands[0], inst.Operands[1]);
        }
        protected virtual void OP_LOADW(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, z_memory.GetWord(inst.Operands[0] + 2 * inst.Operands[1]));
        }
        protected virtual void OP_LOADB(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, z_memory.GetByte(inst.Operands[0] + inst.Operands[1]));
        }

        protected virtual void OP_GET_PROP(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, z_objtable.getProperty(inst.Operands[0], inst.Operands[1]));
        }
        protected virtual void OP_GET_PROP_ADDR(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, z_objtable.getPropertyAddress(inst.Operands[0], inst.Operands[1]));
        }
        protected virtual void OP_GET_NEXT_PROP(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, z_objtable.getNextProperty(inst.Operands[0], inst.Operands[1]));
        }

        protected virtual void OP_ADD(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, (short)(inst.Operands[0] + inst.Operands[1]));
        }
        protected virtual void OP_SUB(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, (short)(inst.Operands[0] - inst.Operands[1]));
        }
        protected virtual void OP_MUL(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, (short)(inst.Operands[0] * inst.Operands[1]));
        }
        protected virtual void OP_DIV(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, (short)(inst.Operands[0] / inst.Operands[1]));
        }
        protected virtual void OP_MOD(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, (short)(inst.Operands[0] % inst.Operands[1]));
        }

        protected virtual void OP_JZ(IZInstruction inst)
        {
            Branch(inst.Operands[0] == 0, inst);
        }
        protected virtual void OP_GET_SIBLING(IZInstruction inst)
        {
            if (z_memory.Header.VersionNumber < 4)
            {
                byte sibling = z_objtable.getSibling((byte)inst.Operands[0]);
                z_memory.PutVariable(inst.Result, sibling);

                Branch(sibling != 0, inst);
            }
            else
            {
                short sibling = z_objtable.getSibling(inst.Operands[0]);
                z_memory.PutVariable(inst.Result, sibling);

                Branch(sibling != 0, inst);
            }
        }
        protected virtual void OP_GET_CHILD(IZInstruction inst)
        {
            if (z_memory.Header.VersionNumber < 4)
            {
                byte child = z_objtable.getChild((byte)inst.Operands[0]);
                z_memory.PutVariable(inst.Result, child);

                Branch(child != 0, inst);
            }
            else
            {
                short child = z_objtable.getChild(inst.Operands[0]);
                z_memory.PutVariable(inst.Result, child);

                Branch(child != 0, inst);
            }
        }
        protected virtual void OP_GET_PARENT(IZInstruction inst)
        {
            if (z_memory.Header.VersionNumber < 4)
                z_memory.PutVariable(inst.Result, z_objtable.getParent((byte)(inst.Operands[0] & 0xff)));
            else
                z_memory.PutVariable(inst.Result, z_objtable.getParent(inst.Operands[0]));
        }
        protected virtual void OP_GET_PROP_LEN(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, z_objtable.getPropertyLength(inst.Operands[0]));
        }
        protected virtual void OP_INC(IZInstruction inst)
        {
            int v = (z_memory.GetVariable(inst.Operands[0]) + 1);

            z_memory.PutVariable(inst.Operands[0], (short)v);
        }
        protected virtual void OP_DEC(IZInstruction inst)
        {
            int v = (z_memory.GetVariable(inst.Operands[0]) - 1);

            z_memory.PutVariable(inst.Operands[0], (short)v);
        }

        protected virtual void OP_PRINT_ADDR(IZInstruction inst)
        {
            z_io.Print(ZMachine.Common.ZText.PrintZString(z_memory, inst.Operands[0]));
        }

        protected virtual void OP_REMOVE_OBJ(IZInstruction inst)
        {
            if (z_memory.Header.VersionNumber < 4)
                z_objtable.removeObject(z_objtable.getParent((byte)inst.Operands[0]), (byte)inst.Operands[0]);
            else
                z_objtable.removeObject(z_objtable.getParent(inst.Operands[0]), inst.Operands[0]);
        }

        protected virtual void OP_PRINT_OBJ(IZInstruction inst)
        {
            z_io.Print(ZMachine.Common.ZText.PrintZString(z_memory, z_objtable.getObjectName(inst.Operands[0])));
        }

        protected virtual void OP_RET(IZInstruction inst)
        {
            Return(inst.Operands[0]);

        }

        protected virtual void OP_JUMP(IZInstruction inst)
        {
            int offset = inst.Operands[0];
            z_frame.PC += offset - 2;
        }

        protected virtual void OP_PRINT_PADDR(IZInstruction inst)
        {
            z_io.Print(ZMachine.Common.ZText.PrintZString(z_memory, UnpackAddress(inst.Operands[0])));
        }

        protected virtual void OP_LOAD(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, (short)inst.Operands[0]);
        }
        protected virtual void OP_NOT(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Result, (short)~inst.Operands[0]);
        }
        protected virtual void OP_RTRUE(IZInstruction inst)
        {
            Return(1);
        }
        protected virtual void OP_RFALSE(IZInstruction inst)
        {
            Return(0);
        }

        protected virtual void Return(short resultValue)
        {
            int? result = z_frame.Result;
            z_frame = z_frame.PrevCallFrame;
            z_memory.Locals = z_frame.Locals;

            if (result.HasValue)
                z_memory.PutVariable(result.Value, resultValue);
        }

        protected virtual void OP_PRINT(IZInstruction inst)
        {
            string ret = ZMachine.Common.ZText.PrintZString(z_memory, z_frame.PC);
            AdvancePCBeyondString();

#if (VERBOSEDEBUG)
            System.Diagnostics.Debug.WriteLine(ret);
#endif

            z_io.Print(ret);
        }

        protected void AdvancePCBeyondString()
        {
            int tmp = 0;
            while ((tmp & 0x8000) == 0)
            {
                tmp = z_memory.GetWord(z_frame.PC);
                z_frame.PC += 2;
            }
        }



        protected virtual void OP_PRINT_RET(IZInstruction inst)
        {
            string ret = ZMachine.Common.ZText.PrintZString(z_memory, z_frame.PC);
            AdvancePCBeyondString();

#if (VERBOSEDEBUG)
            System.Diagnostics.Debug.WriteLine(ret);
#endif

            z_io.Print(ret);
            z_io.Print("\n");

            Return(1);
        }

        protected virtual void OP_NOP(IZInstruction inst)
        {
            return;
        }


        protected virtual void OP_SAVE(IZInstruction inst)
        {
            if (OnSave != null)
                OnSave();
            else
                z_io.Print("\n*** Save not currently handled ***\n");
        }


        protected virtual void OP_RESTORE(IZInstruction inst)
        {

            if (OnRestore != null)
                OnRestore();
            else
                z_io.Print("\n*** Restore not currently handled ***\n");
        }
        protected virtual void OP_RESTART(IZInstruction inst)
        {
            Stop();

            if (OnRestart != null)
                OnRestart();
            else
                z_io.Print("\n*** Restart not currently handled ***\n");
        }
        protected virtual void OP_RET_POPPED(IZInstruction inst)
        {
            Return(z_memory.GetVariable(0));
        }
        protected virtual void OP_POP(IZInstruction inst)
        {
            z_memory.GetVariable(0);  // throws it away
        }
        protected virtual void OP_QUIT(IZInstruction inst)
        {
            Stop();  // Probably need a QUIT handler, because this is insufficient
        }
        
        protected virtual void OP_NEW_LINE(IZInstruction inst)
        {
            z_io.Print("\n");
        }

        protected virtual void OP_SHOW_STATUS(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode not yet coded.");

        }
        protected virtual void OP_VERIFY(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode not yet coded.");
        }
        protected virtual void OP_CALL(IZInstruction inst)
        {
            CALL_FUNCTION(inst);
        }

        protected virtual void OP_STOREW(IZInstruction inst)
        {
            z_memory.PutWord(inst.Operands[0] + (2 * inst.Operands[1]), inst.Operands[2]);
        }
        protected virtual void OP_STOREB(IZInstruction inst)
        {
            z_memory.PutByte(inst.Operands[0] + inst.Operands[1], (byte)(inst.Operands[2] & 0xff));
        }
        protected virtual void OP_PUT_PROP(IZInstruction inst)
        {
            z_objtable.putProperty(inst.Operands[0], inst.Operands[1], inst.Operands[2]);
        }


        protected virtual void ParseInput(string input, IZInstruction inst)
        {

#if (VERBOSEDEBUG)
            System.Diagnostics.Debug.WriteLine("SREAD: " + input);
#endif

            int maxchars = z_memory.GetByte(inst.Operands[0]);
            input = input.ToLower();
            if (input.Length > maxchars)
                input = input.Substring(0, maxchars);

            short termlen = (short)input.Length;

            for (int i = 0; i < input.Length; i++)
            {
                z_memory.PutByte(inst.Operands[0] + i + 1, (byte)input[i]);
            }
            z_memory.PutByte(inst.Operands[0] + input.Length + 2, 0);

            string splitters = z_dict.GetWordSeparatorsAsString();
            for (int i = 0; i < splitters.Length; i++)
            {
                input = input.Replace(splitters[i].ToString(), String.Format(" {0} ", splitters[i]));
            }
            string[] split = input.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            int maxwords = z_memory.GetByte(inst.Operands[1]);

            if (split.Length < maxwords)
            {
                maxwords = split.Length;
            }

            z_memory.PutByte(inst.Operands[1] + 1, (byte)maxwords);

            for (int i = 0; i < maxwords; i++)
            {
                int entryAddr = z_dict.GetAddressForEntry(split[i]);
                // addr=2bytes, numLetters=1byte, textBuffPos=1byte

                z_memory.PutWord(inst.Operands[1] + (4 * i) + 2, (short)entryAddr);
                z_memory.PutByte(inst.Operands[1] + (4 * i) + 4, (byte)split[i].Length);
                z_memory.PutByte(inst.Operands[1] + (4 * i) + 5, (byte)(input.IndexOf(split[i]) + 1)); //not totally correct
            }

            if (z_memory.Header.VersionNumber >= 5)
                z_memory.PutVariable(inst.Result, termlen);


            //    putVariable(curResult, termChar);

            //
            //String s;
            //StringBuffer sb;
            //int termChar;
            //int len;
            //int curaddr;
            //int baddr1, baddr2;
            //int time = 0, raddr = 0;

            //baddr1 = vops[0];
            //baddr2 = vops[1];
            //if (numvops > 2)
            //{
            //    time = vops[2];
            //    raddr = vops[3];
            //}

            //// Flush the I/O card's output buffer
            //ioCard.outputFlush();

            //// This implies a SHOW_STATUS in V1-3.
            //if (version < 4)
            //    zop_show_status();

            //// Read a line of text
            //sb = new StringBuffer();
            //if ((time > 0) && (raddr > 0))
            //{ // A timed READ
            //    while (true)
            //    { // Ick.
            //        termChar = ioCard.readLine(sb, time);
            //        if (termChar == -1)
            //        { // A timeout
            //            // ioCard.outputFlush();
            //            // did_newline = false;
            //            for (int i = 0; i < sb.length(); i++)
            //                ioCard.printString("\b");
            //            int rc = interrupt(raddr);
            //            if (rc == 0)
            //            {
            //                // if (did_newline) {
            //                // ioCard.printString("\n" + sb.toString());
            //                // ioCard.outputFlush();
            //                // }
            //                ioCard.printString(sb.toString());
            //                ioCard.outputFlush();
            //                continue;
            //            }
            //            else
            //            {
            //                ioCard.outputFlush();
            //                sb = new StringBuffer();
            //                termChar = 0;
            //                break;
            //            }
            //        }
            //        else // Not a timeout
            //            break;
            //    }
            //}
            //else
            //    termChar = ioCard.readLine(sb, 0);
            //s = sb.toString();

            // If V1-4, just store the line. If V5+, possibly
            // store it after other characters in the buffer.
            //if (version <= 4)
            //{
            //    curaddr = baddr1 + 1;
            //    len = s.length();
            //    for (int i = 0; i < len; i++)
            //    {
            //        memory.putByte(curaddr, Character.toLowerCase(s.charAt(i)));
            //        curaddr++;
            //    }
            //    memory.putByte(curaddr, 0);
            //}
            //else
            //{
            //    int nchars = memory.fetchByte(baddr1 + 1);
            //    curaddr = baddr1 + 2 + nchars;
            //    len = s.length();
            //    for (int i = 0; i < len; i++)
            //    {
            //        memory.putByte(curaddr, Character.toLowerCase(s.charAt(i)));
            //        curaddr++;
            //    }
            //    memory.putByte(baddr1 + 1, (nchars + len));
            //}

            //// Tokenize input
            //if (baddr2 != 0)
            //{
            //    vops[0] = baddr1;
            //    vops[1] = baddr2;
            //    numvops = 2;
            //    zop_tokenise();
            //}

            //// If V5+, store result
            //if (version >= 5)
            //    putVariable(curResult, termChar);
        }



        protected virtual void OP_SREAD(IZInstruction inst)
        {
            ParseInput(z_io.ReadString(null), inst);
        }

        protected virtual void OP_PRINT_CHAR(IZInstruction inst)
        {
            z_io.Print(String.Format("{0}", (char)inst.Operands[0]));
        }

        protected virtual void OP_PRINT_NUM(IZInstruction inst)
        {
            z_io.Print(String.Format("{0}", inst.Operands[0]));
        }

        protected virtual void OP_RANDOM(IZInstruction inst)
        {
            short rnd = 0;

            if (inst.Operands[0] < 0)
            {
                z_random = new Random(inst.Operands[0]);
            }

            else if (inst.Operands[0] == 0)
            {
                z_random = new Random();
            }

            else
            {
                rnd = (short)z_random.Next(1, inst.Operands[0]);
            }

            z_memory.PutVariable(inst.Result, rnd);
        }

        protected virtual void OP_PUSH(IZInstruction inst)
        {
            z_memory.PutVariable(0, inst.Operands[0]);
        }

        protected virtual void OP_PULL(IZInstruction inst)
        {
            z_memory.PutVariable(inst.Operands[0], z_memory.GetVariable(0));
        }

        protected virtual void OP_SPLIT_WINDOW(IZInstruction inst)
        {
            z_io.SplitWindow(inst.Operands[0]);
        }

        protected virtual void OP_SET_WINDOW(IZInstruction inst)
        {
            z_io.SetWindow(inst.Operands[0]);
        }

        protected virtual void OP_OUTPUT_STREAM(IZInstruction inst)
        {
            z_io.SetOutputStream(inst.Operands[0]);
        }

        protected virtual void OP_INPUT_STREAM(IZInstruction inst)
        {
            // I'm not going to implement this in my demo code
            throw new NotImplementedException("Opcode OP_INPUT_STREAM is not implemented");
        }

        protected virtual void OP_CALL_2S(IZInstruction inst)
        {
            CALL_FUNCTION(inst);
        }

        protected virtual void OP_CALL_2N(IZInstruction inst)
        {
            CALL_PROCEDURE(inst);
        }

        protected virtual void OP_SET_COLOR(IZInstruction inst)
        {
            z_io.SetColor(inst.Operands[0], inst.Operands[1]);
        }

        protected virtual void OP_THROW(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_THROW is not implemented");
        }

        protected virtual void OP_CALL_1S(IZInstruction inst)
        {
            CALL_FUNCTION(inst);
        }

        protected virtual void OP_CALL_1N(IZInstruction inst)
        {
            CALL_PROCEDURE(inst);
        }

        protected virtual void OP_CATCH(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_CATCH is not implemented");
        }

        protected virtual void EXTENDED(IZInstruction inst)
        {
            // This isn't an actual opcode in of itself
            throw new NotImplementedException("Opcode EXTENDED is not implemented");
        }

        protected virtual void OP_PIRACY(IZInstruction inst)
        {
            // Arrr, matey.
            Branch(true, inst);
        }

        protected virtual void OP_CALL_VS(IZInstruction inst)
        {
            CALL_FUNCTION(inst);
        }

        protected virtual void OP_AREAD(IZInstruction inst)
        {
            OP_SREAD(inst);
        }

        protected virtual void OP_CALL_VS2(IZInstruction inst)
        {
            CALL_FUNCTION(inst);
        }

        protected virtual void OP_ERASE_WINDOW(IZInstruction inst)
        {
            z_io.EraseWindow(inst.Operands[0]);
        }

        protected virtual void OP_ERASE_LINE(IZInstruction inst)
        {
            if (inst.Operands[0] == 1)
                z_io.EraseLine();
        }

        protected virtual void OP_SET_CURSOR(IZInstruction inst)
        {
            z_io.SetCursor((byte)inst.Operands[0], (byte)inst.Operands[1]);
        }

        protected virtual void OP_GET_CURSOR(IZInstruction inst)
        {
            var pos = z_io.GetCursor();
            z_memory.PutWord(inst.Operands[0], pos[0]);
            z_memory.PutWord(inst.Operands[1], pos[1]);
        }

        protected virtual void OP_SET_TEXT_STYLE(IZInstruction inst)
        {
            z_io.SetStyle(inst.Operands[0]);
        }

        protected virtual void OP_BUFFER_MODE(IZInstruction inst)
        {
            z_io.BufferMode(inst.Operands[0]);
        }

        protected virtual void OP_SOUND_EFFECT(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_SOUND_EFFECT is not implemented");
        }

        protected virtual void OP_READ_CHAR(IZInstruction inst)
        {
            int? time = null;

            //// TODO: Need to handle time/result
            //if (inst.Operands[1] > 0)
            //    time = inst.Operands[1];

            char c = z_io.ReadChar(time);

            z_memory.PutVariable(inst.Result, (short)c);
        }

        protected virtual void OP_SCAN_TABLE(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_SCAN_TABLE is not implemented");
        }

        protected virtual void OP_CALL_VN(IZInstruction inst)
        {
            CALL_PROCEDURE(inst);
        }

        protected virtual void OP_CALL_VN2(IZInstruction inst)
        {
            CALL_PROCEDURE(inst);
        }

        protected virtual void OP_TOKENIZE(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_TOKENIZE is not implemented");
        }

        protected virtual void OP_ENCODE_TEXT(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_ENCODE_TEXT is not implemented");
        }

        protected virtual void OP_COPY_TABLE(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_COPY_TABLE is not implemented");
        }

        protected virtual void OP_PRINT_TABLE(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_PRINT_TABLE is not implemented");
        }

        protected virtual void OP_CHECK_ARG_COUNT(IZInstruction inst)
        {
            Branch(z_frame.ArgCount > inst.Operands[0], inst);
        }

        protected virtual void OP_LOG_SHIFT(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_LOG_SHIFT is not implemented");
        }

        protected virtual void OP_ART_SHIFT(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_ART_SHIFT is not implemented");
        }

        protected virtual void OP_SET_FONT(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_SET_FONT is not implemented");
        }

        protected virtual void OP_SAVE_UNDO(IZInstruction inst)
        {
            
        }

        protected virtual void OP_RESTORE_UNDO(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_RESTORE_UNDO is not implemented");
        }

        protected virtual void OP_PRINT_UNICODE(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_PRINT_UNICODE is not implemented");
        }

        protected virtual void OP_CHECK_UNICODE(IZInstruction inst)
        {
            throw new NotImplementedException("Opcode OP_CHECK_UNICODE is not implemented");
        }



        protected virtual void CALL_FUNCTION(IZInstruction inst)
        {
            Call(inst, true);
        }

        protected virtual void CALL_PROCEDURE(IZInstruction inst)
        {
            Call(inst, false);
        }

        protected virtual void Call(IZInstruction inst, bool store)
        {
            int newPC = UnpackAddress(inst.Operands[0]); // Unpack PC

            if (newPC == 0)
            {
                z_memory.PutVariable(inst.Result, 0);
                return;
            }

            int numArgs = inst.OperandCount - 1;

            // Store snapshot of locals array to be restored later
            z_frame.Locals = z_memory.Locals;
            //z_frame.Locals = new short[16];
            //Array.Copy(z_memory.Locals, z_frame.Locals,0);

            
            int numVars = z_memory.GetByte(newPC++);

            System.Diagnostics.Debug.WriteLine(string.Format("numVars={0}", numVars));

            short[] newLocals = new short[16];

            if (z_memory.Header.VersionNumber < 5)
            {
                for (int i = 0; i < numVars; i++)
                {
                    newLocals[i + 1] = z_memory.GetWord(newPC);
                    newPC += 2;
                }
            }

            for (int i = 1; i < inst.OperandCount; i++)
            {
                newLocals[i] = inst.Operands[i];
            }

            ZMachine.Common.ZCallFrame frame = new ZMachine.Common.ZCallFrame(newPC) { PrevCallFrame = z_frame, FrameNumber = z_frame.FrameNumber + 1 };

            if (store)
                frame.Result = inst.Result;

            if (numArgs > numVars)
                frame.ArgCount = numVars;
            else
                frame.ArgCount = numArgs;

            // Set z_frame variable to the newly created frame instance
            z_frame = frame;

            z_memory.Locals = newLocals;
        }
    }
}
