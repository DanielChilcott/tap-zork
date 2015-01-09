using System;
using System.Collections.Generic;

namespace ZMachine.Common
{
    [Serializable]

    public class ZObjectTableCore
    {
        protected int DEFAULTSIZE;
        protected int OBJATTRSIZE;
        protected int OBJHANDLESIZE;
        protected int objEntrySize;

        protected IZMemory m_memory;

     

        // NOTE: This code is was derived from the ZAX zmachine object table code
        // as released in 1997 by Matthew E. Kimmel.  Many thanks for having previously
        // figured out this confusing aspect of the ZMachine and providing code
        // that I could study!

        // https://github.com/mattkimmel/zax/blob/master/src/com/zaxsoft/zmachine/ZObjectTable.java


        /////////////////////////////////////////////////////////////////
        // Object manipulation routines
        /////////////////////////////////////////////////////////////////

        // Return the sibling of an object
        public virtual byte getSibling(byte obj)
        {
            return m_memory.GetByte(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE + OBJHANDLESIZE);
        }
        public virtual short getSibling(short obj)
        {
            return m_memory.GetWord(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE + OBJHANDLESIZE);
        }



        // Set the sibling of an object
        public virtual void setSibling(byte obj, byte sib)
        {
            m_memory.PutByte((m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE + OBJHANDLESIZE), sib);
        }
        public virtual void setSibling(short obj, short sib)
        {
            m_memory.PutWord((m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE + OBJHANDLESIZE), sib);
        }




        // Return the first child of an object
        public virtual byte getChild(byte obj)
        {
            return m_memory.GetByte(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE + (2 * OBJHANDLESIZE));
        }
        public virtual short getChild(short obj)
        {
            return m_memory.GetWord(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE + (2 * OBJHANDLESIZE));
        }




        // Set the child of an object
        public virtual void setChild(byte obj, byte child)
        {
            m_memory.PutByte((m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE + (2 * OBJHANDLESIZE)), child);
        }
        public virtual void setChild(short obj, short child)
        {
            m_memory.PutWord((m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE + (2 * OBJHANDLESIZE)), child);
        }



        // Return an object's parent
        public virtual byte getParent(byte obj)
        {
            return m_memory.GetByte(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE);
        }
        public virtual short getParent(short obj)
        {
            return m_memory.GetWord(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE);
        }



        // Set the parent of an object
        public virtual void setParent(byte obj, byte parent)
        {
            m_memory.PutByte((m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE), parent);
        }
        public virtual void setParent(short obj, short parent)
        {
            m_memory.PutWord((m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE), parent);
        }



        // Given its (non-zero) parent, remove an object from the
        // sibling chain.
        public virtual void removeObject(byte parent, byte obj)
        {
            byte curObj, prevObj;

            // It is legal for parent to be 0, in which case we just return.
            if (parent == 0)
                return;

            curObj = getChild(parent);
            if (curObj == 0)
                throw new Exception("Corrupted object table");

            if (curObj == obj)
            {
                // Remove the object
                setChild(parent, getSibling(obj));
                setSibling(obj, 0);
                setParent(obj, 0);
                return;
            }

            // Traverse the sibling chain until we find the object
            // and its predecessor.
            prevObj = curObj;
            curObj = getSibling(prevObj);
            while ((curObj != obj) && (curObj != 0))
            {
                prevObj = curObj;
                curObj = getSibling(prevObj);
            }

            // If we get here, curObj is either the object we're looking
            // for or 0 (which is an error).
            if (curObj == 0)
                throw new Exception("Corrupted object table");

            // Remove the object from the chain, and set its sibling and parent to 0.
            setSibling(prevObj, getSibling(curObj));
            setSibling(obj, 0); // Is this necessary?
            setParent(obj, 0);
        }
        public virtual void removeObject(short parent, short obj)
        {
            short curObj, prevObj;

            // It is legal for parent to be 0, in which case we just return.
            if (parent == 0)
                return;

            curObj = getChild(parent);
            if (curObj == 0)
                throw new Exception("Corrupted object table");

            if (curObj == obj)
            {
                // Remove the object
                setChild(parent, getSibling(obj));
                setSibling(obj, 0);
                setParent(obj, 0);
                return;
            }

            // Traverse the sibling chain until we find the object
            // and its predecessor.
            prevObj = curObj;
            curObj = getSibling(prevObj);
            while ((curObj != obj) && (curObj != 0))
            {
                prevObj = curObj;
                curObj = getSibling(prevObj);
            }

            // If we get here, curObj is either the object we're looking
            // for or 0 (which is an error).
            if (curObj == 0)
                throw new Exception("Corrupted object table");

            // Remove the object from the chain, and set its sibling and parent to 0.
            setSibling(prevObj, getSibling(curObj));
            setSibling(obj, 0); // Is this necessary?
            setParent(obj, 0);
        }





        // Insert obj1 as obj2's first child.
        public virtual void insertObject(byte obj1, byte obj2)
        {
            byte oldfirst = getChild(obj2);

            // First, remove the given object from its current
            // position (if any).
            if (getParent(obj1) > 0)
                removeObject(getParent(obj1), obj1);

            // Now insert it.
            setSibling(obj1, oldfirst);
            setChild(obj2, obj1);
            setParent(obj1, obj2);
        }
        public virtual void insertObject(short obj1, short obj2)
        {
            // First, remove the given object from its current
            // position (if any).
            if (getParent(obj1) > 0)
                removeObject(getParent(obj1), obj1);

            // Now insert it.
            setSibling(obj1, getChild(obj2));
            setChild(obj2, obj1);
            setParent(obj1, obj2);
        }




        //////////////////////////////////////////////////////////////
        // Property manipulation routines
        //////////////////////////////////////////////////////////////

        // Return the length of the property starting at the given
        // byte address
        public virtual short getPropertyLength(int baddr)
        {
            int b = m_memory.GetByte(baddr - 1);

            int length;

            if (m_memory.Header.VersionNumber < 4)
                length = (((b >> 5) & 0x07) + 1);
            else
            {
                if ((b & 0x80) == 0x80)
                    length = (b & 0x3f);
                else
                    length = (((b >> 6) & 0x01) + 1);
            }
            return (short)length;
        }

        // Get the address of the property list for the specified object.
        public virtual int getPropertyList(int obj)
        {
            return m_memory.GetWord(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + OBJATTRSIZE + (3 * OBJHANDLESIZE));
        }

        // Get the address of the specified property of the specified
        // object.  Return 0x0000 if there is no such property.
        public virtual short getPropertyAddress(int obj, int prop)
        {
            int p = getPropertyList(obj);
            int o = m_memory.GetByte(p);
            int s;
            int pnum;
            int psize;

            // Now step through, looking for the specified property.
            // Start by jumping over text header.
            p = p + (o * 2) + 1;

            // Now we're at the start of the property table.
            s = m_memory.GetByte(p);
            while (s != 0)
            {
                // Get the property number and the size of this property.
                if (m_memory.Header.VersionNumber < 4)
                {
                    pnum = (s & 0x1f);
                    psize = ((s >> 5) & 0x07) + 1;
                }
                else
                {
                    pnum = (s & 0x3f);
                    if ((s & 0x80) == 0x80)
                    {
                        p++;
                        psize = m_memory.GetByte(p);
                        psize = psize & 0x3f;
                    }
                    else
                        psize = ((s >> 6) & 0x03) + 1;
                }

                // Step over the size byte
                p++;

                // If this is the correct property, return its address;
                // otherwise, step over the property and loop.
                if (pnum == prop)
                    return (short)p;
                else
                    p = p + psize;

                s = m_memory.GetByte(p);
            }

            // If we make it here, the property was not found.
            return 0;
        }

        // Get the first byte or word of a property--use the default
        // property if this one doesn't exist.
        public virtual short getProperty(int obj, int prop)
        {
            short pdata = getPropertyAddress(obj, prop);

            // If the property exists, return it; otherwise, return
            // the appropriate value from the defaults table.

            if (pdata > 0)
            {
                if (getPropertyLength(getPropertyAddress(obj, prop)) == 1)
                    return (short)m_memory.GetByte(pdata);
                else
                    return (short)m_memory.GetWord(pdata);
            }
            else
                return (short)m_memory.GetWord(m_memory.Header.ObjectTableLocation + ((prop - 1) * 2));
        }

        // Return the property number of the property that follows
        // the specified property in the property list, or 0 if
        // the specified property doesn't exist.
        public virtual short getNextProperty(int obj, int prop)
        {
            int propaddr;
            int proplen;
            int propnum;

            // First, if the property number is 0, just return the
            // number of the first property of this object.
            if (prop == 0)
            {
                propaddr = getPropertyList(obj);
                // Skip over text header
                propaddr = propaddr + (m_memory.GetByte(propaddr) * 2);
                // Return the number of the first property.
                // This will work if the property number is 0, too.
                propnum = m_memory.GetByte(propaddr) & 0x3f;
                return (short)propnum;
            }

            // First, get the address of the specified property.
            // If it doesn't exist, return 0.
            propaddr = getPropertyAddress(obj, prop);
            if (propaddr == 0)
                return 0;

            // Now find out its length.
            proplen = getPropertyLength(propaddr);

            // Skip over the property data
            propaddr += proplen;

            // Now return the number of the next property.  This will
            // return 0 if the property is a 0 byte.
            propnum = m_memory.GetByte(propaddr) & 0x3f;
            return (short)propnum;
        }

        // Return the address of the Z-String containing the specified
        // object's name.
        public virtual short getObjectName(int obj)
        {
            int addr = getPropertyList(obj) + 1;
            return (short)addr;
        }

        // Put the specified value as the specified property of the
        // specified object.
        public virtual void putProperty(int obj, int prop, int value)
        {
            if (getPropertyAddress(obj, prop) == 0)
                return;

            // Now set the property, depending on its length.
            if (getPropertyLength(getPropertyAddress(obj, prop)) == 1)
                m_memory.PutByte(getPropertyAddress(obj, prop), (byte)(value & 0xff));
            else
                m_memory.PutWord(getPropertyAddress(obj, prop), (short)value);
        }


        //////////////////////////////////////////////////////////////
        // Attribute manipulation routines
        //////////////////////////////////////////////////////////////

        // Return true if the specified object has the specified
        // attribute; otherwise return false.
        public virtual bool hasAttribute(int obj, int attr)
        {
            int attrbyte = m_memory.GetByte(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + (attr / 8));
            if ((attrbyte & 0x80 >> (attr % 8)) == (0x80 >> (attr % 8)))
                return true;
            else
                return false;
        }

        // Set an attribute on an object.
        public virtual void setAttribute(int obj, int attr)
        {
            byte attrbyte = m_memory.GetByte(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + (attr / 8));
            attrbyte = (byte)(attrbyte | (byte)(0x80 >> (attr % 8)));
            m_memory.PutByte((m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + (attr / 8)), attrbyte);
        }

        // Clear an attribute on an object.
        public virtual void clearAttribute(int obj, int attr)
        {
            byte attrbyte = m_memory.GetByte(m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + (attr / 8));
            attrbyte = (byte)((attrbyte & ~(0x80 >> (attr % 8))) & 0xff);
            m_memory.PutByte((m_memory.Header.ObjectTableLocation + DEFAULTSIZE + ((obj - 1) * objEntrySize) + (attr / 8)), attrbyte);
        }
    }
}
