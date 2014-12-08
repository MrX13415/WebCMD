using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCMD.Net.Event
{
    public class RequestEvent
    {
        public Type EventClass { get; set; }

        public string EventTypeID { get; set; }

        public string TargetElementID { get; set; }

        public Client EventSource { get; set; }

        public RequestEvent(Client client, string targetID, Type eventClass)
        {
            EventSource = client;
            TargetElementID = targetID;
            EventClass = eventClass;
            EventTypeID = eventClass.Name;
        }

    }
}