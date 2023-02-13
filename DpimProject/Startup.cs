using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using System;

[assembly: OwinStartupAttribute(typeof(DpimProject.Startup))]
namespace DpimProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

            // Make long polling connections wait a maximum of 110 seconds for a
            // response. When that time expires, trigger a timeout command and
            // make the client reconnect.

            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(110);

            // Wait a maximum of 30 seconds after a transport connection is lost
            // before raising the Disconnected event to terminate the SignalR connection.

            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30);

            // For transports other than long polling, send a keepalive packet every
            // 10 seconds. 
            // This value must be no more than 1/3 of the DisconnectTimeout value.

            GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(10);

            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);
            //app.MapSignalR();
            //ConfigureAuth(app);
        }
    }
}