using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCMD.Net.Event;
using WebCMD.Util.Html;

namespace WebCMD.Net
{
    public class Client
    {
        private static List<Client> clients = new List<Client>();

        public string ConnectionID { get; private set; }
        public Guid GUID { get; private set; }
        public ResponseHandler Response { get; private set; }
        public Int64 RequestCount { get; set; }

        private Client(Guid guid)
        {
            this.GUID = guid;
            Register();
        }

        public void InitializeResponseHandler(Action<ResponseEvent> function)
        {
            Response = new ResponseHandler(ConnectionID, function);
        }

        public static Client Create(Guid guid)
        {
            return new Client(guid);
        }

        public static Client Map(Guid guid, string connectionID, Action<ResponseEvent> function)
        {
            Client client = Instance(guid);
            if (client == null)
                return Client.ReCreate(connectionID, function);
            else 
                return client.Map(connectionID, function);
        }

        public Client Map(string connectionID, Action<ResponseEvent> function)
        {
            return _Map(this, connectionID, function);
        }

        private static Client _Map(Client client, string connectionID, Action<ResponseEvent> function)
        {
            client.ConnectionID = connectionID;
            client.InitializeResponseHandler(function);

            Debug.WriteLine(String.Concat(" (i) Mapped GUID:", client.GUID, " to CONID:", client.ConnectionID));
            return client;
        }

        public static Client ReCreate(string connectionID, Action<ResponseEvent> function)
        {
            Client client = Create(Guid.NewGuid());
            _Map(client, connectionID, function);
            Debug.WriteLine(" /!\\ Previous session lost for client " + client);
            client.Response.Send(CmdMessage.GetWarningMessage("The session has been lost due to an unknown error. A new session has been created. All data from the previous session has been lost."));
            return client;
        }

        public static Client Instance(string connectionID, Action<ResponseEvent> function)
        {
            Client client = Client.Instance(connectionID);
            if (client == null) client = Client.ReCreate(connectionID, function);
            return client;
        }

        public static Client Instance(string connectionID)
        {
            return GetClient(null, connectionID);
        }

        public static Client Instance(Guid guid)
        {
            return GetClient(guid, null);
        }

        private static Client GetClient(Nullable<Guid> guid, string connectionID)
        {
            if (String.IsNullOrEmpty(connectionID) && guid == null) return null;

            for (int i = 0; i < clients.Count; i++)
            {
                Client c = clients[i];

                if ((!String.IsNullOrEmpty(connectionID) &&
                     !String.IsNullOrEmpty(c.ConnectionID) &&
                     c.ConnectionID == connectionID) ||
                    (guid != null && c.GUID.Equals(guid)))
                    return c;
            }
            return null;
        }

        public static bool Exists(Guid guid)
        {
            return Instance(guid) != null;
        }

        public void Register()
        {
            if (Exists(this.GUID)) throw new Exception("Client allready known with GUID: " + this.GUID);
            clients.Add(this);
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(ConnectionID))
                return String.Concat("GUID:", GUID);
            return String.Concat("GUID:", GUID, " via CONID:", ConnectionID);
        }
    }
}
