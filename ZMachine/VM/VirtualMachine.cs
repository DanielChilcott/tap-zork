// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// =================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.VM
{
    [Serializable]
    /// <summary>
    /// Represents the "real-world" portion of the ZMachine.
    /// This is an abstract class.
    /// </summary>
    public abstract class VirtualMachine : ZMachine.IVirtualMachine
    {
        protected Guid m_ticket = Guid.Empty;
        protected IZProcessor z_processor = null;
        protected ZMachine.Common.ZIO z_io = null;

        protected virtual Guid Load(byte[] storyBytes)
        {
            m_ticket = Guid.NewGuid();

            switch (storyBytes[0])
            {
                case 3:
                    z_processor = new v3.ZProcessor();
                    z_io = z_processor.LoadStory(storyBytes);
                    z_io.Screen = new v3.V3Screen(z_processor as v3.ZProcessor);
                    break;
                case 5:
                    z_processor = new v5.ZProcessor();
                    z_io = z_processor.LoadStory(storyBytes);
                    z_io.Screen = new v5.V5Screen(z_processor as v5.ZProcessor);
                    break;
                default:
                    throw new Exception("Unsupported story file");
            }

            return m_ticket;
        }

        public void Start()
        {
            z_processor.Start();
        }

        public virtual bool IsRunning
        {
            // Deriving class determines appropriate action
            get { return (z_processor != null && z_processor.Running); }
        }

        protected virtual void Serialize()
        {
            // Deriving class determines appropriate action
            throw new Exception("The method or operation is not implemented.");
        }

        protected virtual void Deserialize()
        {
            // Deriving class determines appropriate action
            throw new Exception("The method or operation is not implemented.");
        }



    }
}
