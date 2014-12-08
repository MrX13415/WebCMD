using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net;
using WebCMD.Core;
using WebCMD.Net.Event;
using WebCMD.Util;
using System.Threading;

namespace WebCMD.Net.SignalR
{
    public class ConsoleHub : Hub
    {
        public void Respond(ResponseEvent e)
        {
            Clients.Client(e.TagretConnectionID).processServerData(e.Data);
            Log.Add("       <--- " + e.Data);
        }

        public void Request(string eventArgument, string connectionID)
        {
            //Invalid connection ...
            if (connectionID == null || connectionID.Equals("")) return;
            
            //Try to optain the current client object or create a new one ...
            Client client = Client.Instance(connectionID);
            if (client == null) client = new Client(connectionID, this.Respond);

            client.RequestCount++;   //update requestcounter

            Log.Add(" ---> " + eventArgument);

            //process event ...
            RequestHandler.HandleEvents(client, eventArgument);

            ServerError.SendErrors(client);
            
            ServerResponse dsr = ResponseHandler.CreateOutputResponse("<div class=\"msg-server\"> /!\\ RQ: " + client.RequestCount + "| RS: " + client.Response.ResponseCount + " | R: <crrsdata> | CONID: " + client.ConnectionID + "</div>");
            client.Response.Send(dsr);
            

            //send the response to the client
            //client.Response.Respond(new ResponseEvent(client.Response.NextBlock, connectionID));
            
        }

        
    }

}