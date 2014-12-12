using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Net.Event
{
    public class ResponseEvent
    {
        public string Data { get; private set; }
        public string[] ConnectionIDs { get; private set; }

        public ResponseEvent(string data, params string[] targetIDs)
        {
            ConnectionIDs = targetIDs;
            Data = data;
        }
    }
}
