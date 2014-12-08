using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net;

namespace WebCMD.Util
{
    public class ServerError
    {
        private static List<Exception> _errors = new List<Exception>();
        private static List<Exception> _silent = new List<Exception>();

        public static void Add(Exception ex)
        {
            _errors.Add(ex);
        }

        public static void AddSilent(Exception ex)
        {
            _silent.Add(ex);
        }

        public static bool HasErrors
        {
            get { return _errors.Count > 0 ; }
        }

        public static void ClearErrors()
        {
            _errors.Clear();
        }

        public static bool HasSilentErrors
        {
            get { return _silent.Count > 0; }
        }

        public static Exception Get(int index)
        {
            return _errors.Count < 1 ? null : _errors[index];
        }

        public static Exception GetLast
        {
            get { return _errors.Count < 1 ? null : Get(0); }
        }

        public static List<Exception> GetAll
        {
            get { return _errors; }
        }

        public static List<Exception> GetAllSilent
        {
            get { return _silent; }
        }


        public static string _HTMLTemplate_ServerMessage = "<div class=\"console-line msg-server\">\n<span>{0}</span><br><div style=\"margin-left: 60px;\">{1}</div>\n</div>";


        public static void SendErrors(Client client)
        {
            Exception[] errors = ServerError.GetAll.ToArray();

            foreach (Exception ex in errors)
            {
                string errstr = String.Format(_HTMLTemplate_ServerMessage,
                    String.Format("Server-Error: {0} {1}: {2}", ServerError.GetAll.IndexOf(ex).ToString("D3"), ex.GetType(), ex.Message),
                    ex.StackTrace);

                client.Response.Send(ResponseHandler.CreateOutputResponse(errstr));
            }
            ServerError.ClearErrors();
        }
    }
}