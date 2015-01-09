using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using ZMachineHost;


namespace Zork
{
    [RoutePrefix("api/game")]
    public class GameController : ApiController
    {
        public GameResponse ExecuteCommand(string gameId, [FromBody]GameCommand command)
        {
            try
            {
                int junk;
                if ((gameId.Length < 4) || (gameId.Length > 20) || (!int.TryParse(gameId, out junk)))
                    throw new Exception("gameId is not valid!");

                var game = HostManager.GetGame(gameId);

                bool isNewGame = game.IsNewGame;

                if (command.Command != null) command.Command = command.Command.Trim().ToLower();

                switch (command.Command)
                {
                    case "forcesave":
                        HostManager.SaveGame(game);
                        return new GameResponse() { Response =new List<string>() {"Game saved."} };

                    case "forcerestore":
                        HostManager.UnloadGame(game.GameId);
                        game = HostManager.GetGame(gameId);
                        return new GameResponse() { Response =new List<string> {"Game restored."} };

                    default:
                        var gr = game.ExecuteCommand(command);

                        if (isNewGame)
                        {
                            gr.Response.InsertRange(0, string.Format(HostManager.WelcomeText, gameId).Split('\n'));
                        }
                        return gr;
                }
            }
            catch (Exception ex)
            {
                Logging.LogEvent(string.Format("Unable to execute command '{0}' for game {1}", command.Command, gameId), ex);

                return new GameResponse()
                {
                    Response = new List<string>() { "A problem occured while processing the command" }
                };
            }
        }
    }
}