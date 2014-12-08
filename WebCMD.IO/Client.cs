using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCMD.Net.Event;

namespace WebCMD.Net
{
    public class Client
    {
        private static Dictionary<string, Client> clients = new Dictionary<string, Client>();
        
        public string ConnectionID { get; private set; }
        public ResponseHandler Response { get; private set; }
        public Int64 RequestCount { get; set; }

        public Client(string connectionID, Action<ResponseEvent> function)
        {
            ConnectionID = connectionID;
            Response = new ResponseHandler(connectionID, function);

            Register();
        }

        public static Client Instance(string connectionID)
        {
            if (clients.ContainsKey(connectionID))
                return clients[connectionID];
            else return null;
        }

        public void Register()
        {
            if (clients.ContainsKey(ConnectionID))
                throw new Exception("Client ID allready known");
            clients.Add(ConnectionID, this);
        }

    }
}
