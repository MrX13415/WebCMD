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
        private static List<Client> _clients = new List<Client>();

        private ConsoleSession _session = new ConsoleSession();
        
        /// <summary>
        /// The connection id of the client
        /// </summary>
        public string ConnectionID { get; private set; }

        /// <summary>
        /// The clients Globally Unique Identifier (GUID) of the client
        /// </summary>
        public Guid GUID { get; private set; }

        /// <summary>
        /// The response handler object for this client.
        /// </summary>
        public ResponseHandler Response { get; private set; }

        /// <summary>
        /// Total number of server requests this client has allready made
        /// </summary>
        public Int64 RequestCount { get; set; }

        /// <summary>
        /// Store client specific custom data and objects.
        /// </summary>
        public ConsoleSession Session
        {
            get { return _session; }
        }

        private Client(Guid guid)
        {
            this.GUID = guid;
            Register();

            object sa = Session["adasd"];
        }

        /// <summary>
        /// Initialize the Response handler and define the function wich will process and send the response to the actuall client
        /// </summary>
        /// <param name="function">The function to be called on any response from the client</param>
        public void InitializeResponseHandler(Action<ResponseEvent> function)
        {
            Response = new ResponseHandler(ConnectionID, function);
        }

        /// <summary>
        /// Creates a new client instance with the given GUID
        /// </summary>
        /// <param name="guid">The clients Globally Unique Identifier (GUID)</param>
        /// <returns>The newly created client object</returns>
        public static Client Create(Guid guid)
        {
            return new Client(guid);
        }

        /// <summary>
        /// Mapps the given connection with given client and initialize the Response handler. (See: <see cref="InitializeResponseHandler(...)"/> )
        /// </summary>
        /// <remarks>
        /// If there is no client with the given GUID, a new one will be created, assuming that the previouse client object has been lost for unknow reasons (e.g. server crash).
        /// </remarks>
        /// <param name="guid">The clients Globally Unique Identifier (GUID)</param>
        /// <param name="connectionID">The ID of the connection to map with the client</param>
        /// <param name="function">The function wich will process and send the response to the actuall client</param>
        /// <returns>The mapped client object</returns>
        public static Client Map(Guid guid, string connectionID, Action<ResponseEvent> function)
        {
            Client client = Instance(guid);
            if (client == null)
                return Client.ReCreate(connectionID, function);
            else 
                return client.Map(connectionID, function);
        }

        /// <summary>
        /// Mapps the given connection to the current client and initialize the Response handler. (See: <see cref="InitializeResponseHandler(...)"/> )
        /// </summary>
        /// <param name="connectionID">The ID of the connection to map with the client</param>
        /// <param name="function">The function wich will process and send the response to the actuall client</param>
        /// <returns>The mapped client object</returns>
        public Client Map(string connectionID, Action<ResponseEvent> function)
        {
            return _Map(this, connectionID, function);
        }

        private static Client _Map(Client client, string connectionID, Action<ResponseEvent> function)
        {
            client.ConnectionID = connectionID;
            client.InitializeResponseHandler(function);

            Debug.WriteLine(String.Concat(" (i)  Mapped GUID:", client.GUID, " to CONID:", client.ConnectionID));
            return client;
        }

        /// <summary>
        /// Creates a client object and inform the client that the previous client object has been lost for unknown reasons (e.g. server chrash)
        /// </summary>
        /// <param name="connectionID">The connection ID to map the new client to</param>
        /// <param name="function">The function wich will process and send the response to the actuall client</param>
        /// <returns>The new created client object</returns>
        public static Client ReCreate(string connectionID, Action<ResponseEvent> function)
        {
            Client client = Create(Guid.NewGuid());
            _Map(client, connectionID, function);
            Debug.WriteLine(" /!\\ Previous session lost for client " + client);
            client.Response.Send(CmdMessage.GetWarningMessage("The session has been lost due to an unknown error."
                + " A new session has been created. All data from the previous session has been lost."));
            return client;
        }

        /// <summary>
        /// Get an instance of the client object which matches the given connection id.
        /// If there is no client with the given conneection id, a new one will be created,
        /// assuming that the previouse client object has been lost for unknow reasons (e.g. server crash).
        /// </summary>
        /// <param name="connectionID">The connection id to search for</param>
        /// <param name="function">The function wich will process and send the response to the actuall client</param>
        /// <returns>The client object which matches with the given connection id</returns>
        public static Client Instance(string connectionID, Action<ResponseEvent> function)
        {
            Client client = Client.Instance(connectionID);
            if (client == null) client = Client.ReCreate(connectionID, function);
            return client;
        }

        /// <summary>
        /// Get an instance of the client object which matches the given connection id. Returns null if there is no matching client.
        /// </summary>
        /// <param name="connectionID">The connection id to search for</param>
        /// <returns>The client object which matches with the given connection id</returns>
        public static Client Instance(string connectionID)
        {
            return GetClient(null, connectionID);
        }

        /// <summary>
        /// Get an instance of the client object which matches the given GUID. Returns null if there is no matching client
        /// </summary>
        /// <param name="guid">The clients Globally Unique Identifier (GUID) to search for</param>
        /// <returns>The client object which matches with the given connection id</returns>
        public static Client Instance(Guid guid)
        {
            return GetClient(guid, null);
        }

        private static Client GetClient(Nullable<Guid> guid, string connectionID)
        {
            if (String.IsNullOrEmpty(connectionID) && guid == null) return null;

            for (int i = 0; i < _clients.Count; i++)
            {
                Client c = _clients[i];

                if ((!String.IsNullOrEmpty(connectionID) &&
                     !String.IsNullOrEmpty(c.ConnectionID) &&
                     c.ConnectionID == connectionID) ||
                    (guid != null && c.GUID.Equals(guid)))
                    return c;
            }
            return null;
        }

        /// <summary>
        /// Returns whether a client object with the given GUID exists or not.
        /// </summary>
        /// <param name="guid">The clients Globally Unique Identifier (GUID) to search for</param>
        /// <returns>True if there is a client object otherwise not</returns>
        public static bool Exists(Guid guid)
        {
            return Instance(guid) != null;
        }

        internal void Register()
        {
            if (Exists(this.GUID)) throw new Exception("Client allready known with GUID: " + this.GUID);
            _clients.Add(this);
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(ConnectionID))
                return String.Concat("GUID:", GUID);
            return String.Concat("GUID:", GUID, " via CONID:", ConnectionID);
        }
    }
}
