using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Net
{
    class ConnectionInfo
    {
        public Guid ClientGUID { get; private set; }

        public void UpdateState()
        {
            Client client = Client.Instance(ClientGUID);


            



            client.Response.Send();


        }
    }
}
