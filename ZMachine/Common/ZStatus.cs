// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.Common
{
    [Serializable]
    public struct ZStatus
    {
        private bool _IsTimedGame;
        private string _Location;
        private int _Num1;
        private int _Num2;

        public bool IsTimedGame
        {
            get { return _IsTimedGame; }
            set { _IsTimedGame = value; }
        }

        public string Location
        {
            get { return _Location; }
            set { _Location = value; }
        }

        public int Num1
        {
            get { return _Num1; }
            set { _Num1 = value; }
        }

        public int Num2
        {
            get { return _Num2; }
            set { _Num2 = value; }
        }
    }
}
