using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Net.Event
{
    public class ResponseEvent
    {
        public string Data { get; set; }
        public string TagretConnectionID { get; set; }

        public ResponseEvent(string targetID, string data)
        {
            TagretConnectionID = targetID;
            Data = data;
        }
    }
}
