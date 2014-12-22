using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net;
using WebCMD.Core;
using WebCMD.Net.IO;
using WebCMD.Util;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using WebCMD.Util.Html;
using WebCMD.Com;
using WebCMD.Net.IO.Event;

namespace WebCMD.Net.SignalR
{
    public class ConsoleHub : Hub
    {
        public void Respond(ResponseEventArgs args)
        {
            Clients.Clients(args.ConnectionIDs).processServerData(args.Data);
            Log.Add("       <--- " + args.Data);
        }

        public void Request(string eventArgument, string connectionID)
        {
            //Invalid connection ...
            if (String.IsNullOrEmpty(connectionID)) return;
            
            //Try to optain the current client object or create a new one ...
            Client client = Client.Instance(connectionID, this.Respond);

            //TODOD SERVER <-> Client connection cant be restored ??!

            client.RequestCount++;   //update requestcounter

            Log.Add(" ---> " + eventArgument);

            //process event ...
            RequestHandler.HandleEvents(client, eventArgument);


            client.Response.Send(CmdMessage.GetServerMessage("RQ: ", client.RequestCount.ToString(), "| RS: ", client.Response.ResponseCount.ToString(), " | R: <rscountdata> | CONID: ", client.ConnectionID, "</div>"));
        }
        
        public override Task OnDisconnected(bool stopCalled)
        {
            Client client = Client.Instance(Context.ConnectionId);
            Debug.WriteLine(" (i) Disconnected " + client);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnConnected()
        {
            Guid guid = Guid.Parse(Context.QueryString["guid"]);
            Client client = Client.Map(guid, this.Context.ConnectionId, this.Respond);
            Debug.WriteLine(" (i) Connected " + client);

            return base.OnConnected();
        }

        //HANDLE SERVER CRASH 

        public override Task OnReconnected()
        {
            Client client = Client.Instance(Context.ConnectionId, this.Respond);
            Debug.WriteLine(" (i) Reconnected " + client);
            return base.OnReconnected();
        }

    }

}