using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
[assembly: OwinStartup(typeof(WebCMD.Core.Net.SignalR.Startup))]

namespace WebCMD.Core.Net.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here

            HubConfiguration config = new HubConfiguration();
            app.MapSignalR("/sys/script/signalr", config);
        }
    }
}