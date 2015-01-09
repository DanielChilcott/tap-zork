using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Zork
{
    public class HostManager
    {
        private static Stream _storyDefinition;

        private static Dictionary<string, ZMachineHost.Host> _games = new Dictionary<string, ZMachineHost.Host>();

        private static string _gameStatePathTemplate;

        private static int _gameUnloadTimeSecs;

        private static string _welcomeText;

        public static string WelcomeText { get { return _welcomeText; } }

        public static void Initialise(string storyFilePath, string gameStatePathTemplate, int gameUnloadTimeSecs, string welcomeText)
        {
            _storyDefinition = new MemoryStream(File.ReadAllBytes(storyFilePath));

            _gameStatePathTemplate = gameStatePathTemplate;

            _gameUnloadTimeSecs = gameUnloadTimeSecs;

            _welcomeText = welcomeText;
        }

        public static ZMachineHost.Host GetGame(string gameId)
        {
            ZMachineHost.Host host = null;

            lock (_games)
            {
                if (!_games.ContainsKey(gameId))
                {
                    Logging.LogEvent(string.Format("Starting new game {0}", gameId));

                    if (File.Exists(string.Format(_gameStatePathTemplate, gameId)))
                    {
                        host = handleLoadGame(gameId);
                    }
                    else
                    {
                        host = new ZMachineHost.Host(_storyDefinition, gameId, false);
                    }

                    _games.Add(gameId, host);
                    return host;
                }
                else
                {
                    return _games[gameId];
                }
            }
        }


        public static void HandleCleanSessions(bool forceCleanAll)
        {
            //saves and unloads sessions that haven't had any activity
            lock (_games)
            {
                List<string> cull = new List<string>();

                foreach (var game in _games)
                {
                    if ((DateTime.UtcNow.Subtract(game.Value.LastCommandTime).TotalSeconds >= _gameUnloadTimeSecs) || (forceCleanAll))
                    {
                        Logging.LogEvent(string.Format("Unloading game {0}", game.Value.GameId));
                        SaveGame(game.Value);
                        cull.Add(game.Value.GameId);
                    }
                }

                foreach (string g in cull)
                    UnloadGame(g);
            }
        }

        private static ZMachineHost.Host handleLoadGame(string gameId)
        {
            Logging.LogEvent(string.Format("Loading game {0}", gameId));

            string path = string.Format(_gameStatePathTemplate, gameId);
            var stream = File.OpenRead(path);

            try
            {
                return new ZMachineHost.Host(stream, gameId, true);
            }
            finally
            {
                stream.Close();
            }

        }

        public static void SaveGame(ZMachineHost.Host game)
        {
            try
            {

                Logging.LogEvent(string.Format("Saving game {0}", game.GameId));

                string path = string.Format(_gameStatePathTemplate, game.GameId);

                Directory.CreateDirectory(Path.GetDirectoryName(path));

                if (File.Exists(path)) File.Delete(path);

                var stream = File.Create(path);
                game.GetState().CopyTo(stream);
                stream.Flush();
                stream.Close();
            }
            catch (Exception ex)
            {

                Logging.LogEvent(string.Format("Unable to save game {0}", game.GameId), ex);
            }
        }

        /// <summary>
        /// Unloads the game from memory which means that effectively, the next call to GetGame will cause it to be reloaded. Thus, this method is effectively a cue-restore
        /// </summary>
        /// <param name="gameId"></param>
        public static void UnloadGame(string gameId)
        {
            lock (_games)
            {
                _games.Remove(gameId);
            }
        }
    }
}