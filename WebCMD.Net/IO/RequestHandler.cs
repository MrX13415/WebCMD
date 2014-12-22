using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Remoting;
using WebCMD.Util;
using System.Diagnostics;

namespace WebCMD.Net.IO
{
    public class RequestHandler
    {
        public const string EventIdentString = "_RQ";

        private static List<ConsoleEventListener> eventListeners = new List<ConsoleEventListener>();


        public static ServerRequest GenerateEventObject(Client client, string evenstring)
        {
            if (!evenstring.Contains(":")) return null;

            //_REQ:__Page:_CommandEvent:test data
            string[] eventArgs = evenstring.Split(':');
            
            if (!eventArgs[0].Equals(EventIdentString)) return null;

            if (eventArgs.Count() < 3) return null;

            string id = eventArgs[1];
            string eventname = eventArgs[2];
            string args = eventArgs.Count() == 3 ? "" : evenstring.Substring(EventIdentString.Length + id.Length + eventname.Length + 3);

            try
            {   
                //Create the event object ...
                string fqcn = String.Format("{0}.{1}", typeof(ServerRequest).Namespace, eventname);
                return (ServerRequest)Activator.CreateInstance(Type.GetType(fqcn), client, id, args);
            }
            catch
            {
                return null;
            }
        }

        public static void HandleEvents(Client client, string eventstring)
        {
            ServerRequest e = GenerateEventObject(client, eventstring);

            for (int index = 0; index < eventListeners.Count; index++)
            {
                ConsoleEventListener listener = eventListeners[index];
                if (e == null) break;

                Type x = e.RequestClass;
                Type y = listener.EventFilter;

                if (x.IsAssignableFrom(y) || y.IsAssignableFrom(x))
                {
                    listener.Function(e);
                }
            }
        }

        public static List<ConsoleEventListener> GetListener
        {
            get { return eventListeners; }
        }

        public static void AddListener(Action<ServerRequest> lFunction, Type filterType)
        {
            eventListeners.Add(new ConsoleEventListener(lFunction, filterType));
        }

        public static void RemoveListener(Action<ServerRequest> lFunction, Type filterType)
        {
            for (int index = 0; index < eventListeners.Count; index++)
            {
                ConsoleEventListener listener = eventListeners[index];
                if (listener.Function.Equals(lFunction) && listener.EventFilter.Equals(filterType))
                {
                    eventListeners.Remove(listener);
                }
            }
        }
    }

    public class ConsoleEventListener
    {
        public Action<ServerRequest> Function { get; private set; }
        public Type EventFilter { get; private set; }

        public ConsoleEventListener(Action<ServerRequest> lFunction, Type filterType)
        {
            Function = lFunction;
            EventFilter = filterType;
        }
    }
}