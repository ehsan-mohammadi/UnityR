using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Server.Startup))]
namespace Server
{
    /// <summary>
    /// The startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configure the signalR program
        /// </summary>
        public void Configuration(IAppBuilder app)
        {
            // Maps signalR hubs to the app builder pipeline at "/signalr"
            app.MapSignalR();
        }
    }
}