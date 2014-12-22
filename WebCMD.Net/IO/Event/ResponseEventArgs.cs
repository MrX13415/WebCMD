using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Net.IO.Event
{
    public class ResponseEventArgs
    {
        public string Data { get; private set; }
        public string[] ConnectionIDs { get; private set; }

        public ResponseEventArgs(string data, params string[] targetIDs)
        {
            ConnectionIDs = targetIDs;
            Data = data;
        }
    }
}
