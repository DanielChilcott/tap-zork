using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using System.Web.Http;
using System.IO;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;


namespace Zork
{
    public class Global : System.Web.HttpApplication
    {


        private Timer _cleanupTimer;


        protected void Application_Start(object sender, EventArgs e)
        {
            try
            {
                Logging.SetFilePath(HttpContext.Current.Server.MapPath("~/App_Data/" + System.Configuration.ConfigurationManager.AppSettings["LogFile"].ToString()));

                HostManager.Initialise(
                    HttpContext.Current.Server.MapPath("~/App_Data/" + System.Configuration.ConfigurationManager.AppSettings["GameFile"].ToString()),
                    HttpContext.Current.Server.MapPath("~/App_Data/games/{0}.bin"),
                    int.Parse(System.Configuration.ConfigurationManager.AppSettings["GameUnloadTimeSecs"].ToString()),
                    System.Configuration.ConfigurationManager.AppSettings["WelcomeText"].ToString().Replace("\\n", "\n")
                    );


                _cleanupTimer = new Timer(5000);
                _cleanupTimer.Elapsed += (p1, p2) =>
                {
                    try
                    {
                        HostManager.HandleCleanSessions(false);
                    }
                    catch (Exception ex)
                    {
                        Logging.LogEvent("Error in cleanup timer", ex);
                    }
                };
                _cleanupTimer.Start();

                GlobalConfiguration.Configure((config) =>
                {
                    config.MapHttpAttributeRoutes();

                    config.Routes.MapHttpRoute(
                        name: "game",
                        routeTemplate: "api/{controller}/{gameId}"
                    );
                });
            }
            catch (Exception ex)
            {
                Logging.LogEvent("Error in Application_Start", ex);
            }
        }


        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Logging.LogEvent("Application_Error", Server.GetLastError());
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            try
            {
                _cleanupTimer.Stop();
                HostManager.HandleCleanSessions(true);
            }
            catch (Exception ex)
            {
                Logging.LogEvent("Eror in Application_End", ex);
            }
        }
    }
}