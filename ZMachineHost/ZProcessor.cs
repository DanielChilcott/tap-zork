using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZMachineHost
{
    [Serializable]
    public class ZProcessor : ZMachine.v3.ZProcessor
    {

        string _currentText = "";

        bool _isFirstRun = false;

        public ZProcessor()
            : base()
        {

        }


        public override void Start()
        {
            if (Running)
                throw new Exception("Game has already started");

            z_io.Keyboard = new Keyboard();

            z_io.Screen = new Screen();// new V5Screen(z_processor as ZProcessor);

            ((Screen)z_io.Screen).ScreenPrint += (source, text) =>
            {
                if (source == z_io.Screen)
                    _currentText += text;

            };

            if (z_memory == null)
                throw new Exception("Memory not ready!");

            Running = true;
            z_instruct.Decode(z_frame.PC);

#if (VERBOSEDEBUG)
            System.Diagnostics.Debug.WriteLine(z_instruct.ToString());
#endif

            z_frame.PC += z_instruct.Length;

            _isFirstRun = true;
        }

        ///// <summary>
        ///// Returns a list of names of all top level objects
        ///// </summary>
        ///// <returns></returns>
        //public List<string> GetRoomObjects()
        //{
        //    int locationId = z_memory.GetVariable(16);

        //    //recursively enumerate objects
        //    Stack<int> stack = new Stack<int>();
        //    List<string> objectNames = new List<string>();

        //    stack.Push(locationId);

        //    while (stack.Count > 0)
        //    {
        //        var currentObjectId = stack.Pop();

        //        int objectNameId = z_objtable.getObjectName(currentObjectId);

        //        objectNames.Add(ZMachine.Common.ZText.PrintZString(z_memory, objectNameId));

        //        int childId = z_objtable.getChild((byte)currentObjectId);

        //        if (childId > 0)
        //        {
        //            //get siblings of this child

        //            while (childId > 0)
        //            {
        //                stack.Push(childId);
        //                childId = z_objtable.getSibling((byte)childId);
        //            }
        //        }
        //    }

        //    return objectNames;
        //}

        public string ExecuteCommand(string command)
        {
            lock (this)
            {
                if (!Running)
                    throw new Exception("Game has not started");

                ((Keyboard)z_io.Keyboard).NextString = command;
                _currentText = "";

                try
                {

                    bool nextStepConsumesInput = false;

                    if (_isFirstRun)
                    {
                        _isFirstRun = false;

                        while (!nextStepConsumesInput)
                        {
                            nextStepConsumesInput = Execute();
                        }
                    }

                    if (command.Length > 0)
                    {
                        nextStepConsumesInput = false;

                        while (!nextStepConsumesInput)
                        {
                            nextStepConsumesInput = Execute();
                        }
                    }

                    return CleanResponse(_currentText);
                }
                finally
                {
                    _currentText = "";
                }
            }

        }

        private string CleanResponse(string text)
        {
            //trim repeat line endings
            int i = 0;
            while (i != text.Length)
            {
                i = text.Length;
                text = text.Replace("\n\n", "\n");
            }

            if (text.EndsWith(">"))
                text = text.Substring(0, text.Length - 1);

            text = text.Trim();

            return text;
        }

        private new bool Execute()
        {

            switch (z_instruct.OpcodeType)
            {
                case ZMachine.Common.OpcodeType.OP_0:
                    OP_0[z_instruct.OpCode](z_instruct);
                    break;
                case ZMachine.Common.OpcodeType.OP_1:
                    OP_1[z_instruct.OpCode](z_instruct);
                    break;
                case ZMachine.Common.OpcodeType.OP_2:
                    OP_2[z_instruct.OpCode](z_instruct);
                    break;
                case ZMachine.Common.OpcodeType.OP_VAR:
                    OP_VAR[z_instruct.OpCode](z_instruct);
                    break;
                case ZMachine.Common.OpcodeType.OP_EXT:
                    OP_EXT[z_instruct.OpCode](z_instruct);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Operation code was {0}",z_instruct.OpcodeType));
            }


            z_instruct.Decode(z_frame.PC);

#if (VERBOSEDEBUG)
            System.Diagnostics.Debug.WriteLine(z_instruct.ToString());
#endif

            z_frame.PC += z_instruct.Length;

            //determine whether the next step will consume input
            return
                (z_instruct.OpcodeType == ZMachine.Common.OpcodeType.OP_VAR) &&
                (z_instruct.OpCode == 4);//readstring
        }

        public ZMachine.GameState GetState()
        {
            return base.z_memory.GetState();
        }

        public void SetState(ZMachine.GameState state)
        {
            base.z_memory.SetState(state);
        }


    }
}
