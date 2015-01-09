// ==================================================================
// ZMachine.NET / Jason Follas  (http://zmachine.codeplex.com) 
// ==================================================================

using System;
using System.Collections.Generic;

namespace ZMachine.v5
{
    /// <summary>
    /// Reads and caches a Dictionary from the story file.
    /// </summary>
    [Serializable]
    public class ZDictionary : IZDictionary
    {
        // Holds the reference to the current ZMemory object
        protected ZMemory m_memory = null;

        // Number of Word Separators as read from the dictionary header
        protected byte numWordSeparators = 0;

        // Char array containing the declared Word Separators
        protected char[] wordSeparatorChars;

        // Number of dictionary entries
        protected int numEntries = 0;

        // Length of each dictionary entry
        protected byte entryLength = 0;

        // Typed Dictionary collection (.NET definition, not Infocom definition) that caches the story file
        // words and memory addresses, which is used during parsing
        protected Dictionary<string, int> entries = null;


        /// <summary>
        /// Constructor.  Initializes the ZDictionary to the default story file's dictionary location.
        /// </summary>
        /// <param name="mem">Active ZMemory object</param>
        public ZDictionary(ZMemory mem)
        {
            m_memory = mem;
            Initialize(m_memory.Header.DictionaryLocation);
        }

        /// <summary>
        /// Constructor.  Initializes the ZDictionary to the specified story file's dictionary location.  (Supposedly, some
        /// games can have multiple dictionaries).
        /// </summary>
        /// <param name="mem">Active ZMemory object</param>
        /// <param name="dictionaryAddress">Address location of the specified dictionary</param>
        public ZDictionary(ZMemory mem, int dictionaryAddress)
        {
            m_memory = mem;
            Initialize(dictionaryAddress);
        }

        /// <summary>
        /// Populates the object from values read from the ZMemory
        /// </summary>
        /// <param name="dictAddr">Address of the dictionary</param>
        protected virtual void Initialize(int dictAddr)
        {
            // Dictionary header starts with a byte declaring the number of Word Separators
            numWordSeparators = m_memory.GetByte(dictAddr);

            // Immediately followed by that number of character codes (ZSCII, but close enough to ASCII to not worry about for now)
            wordSeparatorChars = new char[numWordSeparators];

            for (int i = 0; i < numWordSeparators; i++)
            {
                wordSeparatorChars[i] = (char)m_memory.GetByte(dictAddr + i + 1);
            }

            // Next header byte declares the entry length
            entryLength = m_memory.GetByte(dictAddr + numWordSeparators + 1);

            //Next header word declares the number of entries
            numEntries = m_memory.GetWord(dictAddr + numWordSeparators + 2);

            // Create a new typed Dictionary object using string keys and int values.
            entries = new Dictionary<string, int>();

            // Address of first byte past the header
            int startOfEntries = dictAddr + numWordSeparators + 4;

            // Get each story file dictionary entry, decode the text, and store in our typed Dictionary object using the
            // text as the key, and the story file address as the value
            for (int i = 0; i < numEntries; i++)
            {
                entries.Add(ZMachine.Common.ZText.PrintZString(m_memory, startOfEntries + (i * entryLength)), startOfEntries + (i * entryLength));
            }
        }

        /// <summary>
        /// Returns the story file address for the given word (text)
        /// </summary>
        /// <param name="entry">Word (text) entered by the user</param>
        /// <returns>Address of dictionary entry for the given text, or 0 if the word does not exist in the story file</returns>
        public virtual int GetAddressForEntry(string entry)
        {
            // Version 3: All words are truncated at 6 characters
            if (entry.Length > 6)
                entry = entry.Substring(0, 6);

            int ret = 0;
            if (entries.TryGetValue(entry, out ret))
                return ret;
            else
                return 0;
        }

        /// <summary>
        /// Concatenates all of the Word Separator characters into a single string
        /// </summary>
        /// <returns>String</returns>
        public virtual string GetWordSeparatorsAsString()
        {
            return new string(wordSeparatorChars);
        }
    }
}
