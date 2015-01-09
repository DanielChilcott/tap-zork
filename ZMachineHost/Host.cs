using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ZMachineHost
{
    /// <summary>
    /// Hosts a single game instance
    /// </summary>
    [Serializable]
    public class Host
    {

        ZProcessor _processor;

        bool _isJustLoaded = false;
        bool _isNewGame = false;

        public string GameId { get; private set; }

        public bool IsJustLoaded { get { return _isJustLoaded; } }

        public bool IsNewGame { get { return _isNewGame; } }

        /// <summary>
        /// Starts a new game or loads an existing game
        /// </summary>
        /// <param name="stream">When isSavedGame is true, contains a binary-serialised game state stream. When false, contains the z machine story definition</param>
        /// <param name="gameId"></param>
        /// <param name="isSavedGame"></param>
        public Host(Stream stream, string gameId, bool isSavedGame)
        {
            _isJustLoaded = true;

            if (isSavedGame)
            {
                _processor = (ZProcessor)new BinaryFormatter().Deserialize(stream);
                GameId = gameId;
            }
            else
            {
                _isNewGame = true;

                GameId = gameId;

                var ms = new MemoryStream();
                stream.Position = 0;
                stream.CopyTo(ms);

                _processor = new ZProcessor();
                _processor.LoadStory(ms.ToArray());
                _processor.Start();
            }

        }

        public DateTime LastCommandTime { get; private set; }

        public GameResponse ExecuteCommand(GameCommand command)
        {
            
            LastCommandTime = DateTime.UtcNow;

            if (command.Command == null) command.Command = "";

            var gameResponse = new GameResponse()
            {
                Response = _processor.ExecuteCommand(command.Command).Split('\n').ToList()
            };

            _isJustLoaded = false;
            _isNewGame = false;

            return gameResponse;
        }

        public Stream GetState()
        {
            var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, _processor);
            stream.Position = 0;
            return stream;

        }


    }
}
