using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net.Event;
using System.Runtime.Remoting;
using WebCMD.Util;

namespace WebCMD.Net
{
    public class RequestHandler
    {
        public const string EventIdentString = "_RQ";

        private static List<ConsoleEventListener> eventListeners = new List<ConsoleEventListener>();


        //public static RequestHandler Instance
        //{
        //    get
        //    {
        //        object rsh = Session;

        //        if (rsh != null && rsh.GetType().FullName.Equals(typeof(RequestHandler).FullName))
        //            return (RequestHandler)rsh;
        //        else
        //        {
        //            Session = new RequestHandler();
        //            return (RequestHandler)Session;
        //        }
        //    }
        //}

        //private static object Session
        //{
        //    get { return Ref.GetSessionClassObject(typeof(RequestHandler)); }
        //    set { Ref.SetSessionClassObject(typeof(RequestHandler), value); }
        //}

        //public static RequestHandler Instance(string id)
        //{

        //    object rsh = Ref.GetSessionClassObject(id, typeof(RequestHandler));

        //        if (rsh != null && rsh.GetType().FullName.Equals(typeof(RequestHandler).FullName))
        //            return (RequestHandler)rsh;
        //        else
        //        {
        //            Ref.SetSessionClassObject(id, typeof(RequestHandler), new RequestHandler());
        //            return (RequestHandler)Ref.GetSessionClassObject(id, typeof(RequestHandler));
        //        }

        //}

        public static RequestEvent GenerateEventObject(Client client, string evenstring)
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
                string fqcn = String.Format("{0}.{1}", typeof(RequestEvent).Namespace, eventname);
                return (RequestEvent)Activator.CreateInstance(Type.GetType(fqcn), client, id, args);
            }
            catch
            {
                return null;
            }
        }

        public static void HandleEvents(Client client, string eventstring)
        {
            RequestEvent e = GenerateEventObject(client, eventstring);

            for (int index = 0; index < eventListeners.Count; index++)
            {
                ConsoleEventListener listener = eventListeners[index];
                if (e == null) break;

                Type x = e.EventClass;
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

        public static void AddListener(Action<RequestEvent> lFunction, Type filterType)
        {
            eventListeners.Add(new ConsoleEventListener(lFunction, filterType));
        }

        public static void RemoveListener(Action<RequestEvent> lFunction, Type filterType)
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
        public Action<RequestEvent> Function { get; private set; }
        public Type EventFilter { get; private set; }

        public ConsoleEventListener(Action<RequestEvent> lFunction, Type filterType)
        {
            Function = lFunction;
            EventFilter = filterType;
        }
    }
}