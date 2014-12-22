using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCMD.Net.IO
{
    public class ServerRequest
    {
        public Type RequestClass { get; set; }

        public string RequestTypeID { get; set; }

        public string TargetElementID { get; set; }

        public Client Source { get; set; }

        public ServerRequest(Client client, string targetID, Type requestClass)
        {
            Source = client;
            TargetElementID = targetID;
            RequestClass = requestClass;
            RequestTypeID = GetRequestTypeID(requestClass);
        }

        public static string GetRequestTypeID(Type requestClass)
        {
            return requestClass.Name;
        }
    }
}