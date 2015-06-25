using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WebCMD.Net.IO
{
    [Obsolete]
    public class UpdateEvent : ServerRequest
    {
        private UpdateEvent(Client client, string targetid) : base(client, targetid, typeof(UpdateEvent))
        {
        }

        /// <summary>
        /// jsEventArgs = "_CommandEvent <command> <args>"
        /// Spaced by tabs
        /// see ToString();
        /// </summary>
        /// <param name="jsEventArgs">_CommandEvent <command> <args></param>
        public UpdateEvent(Client client, string targetid, string eventArgs) : this(client, targetid)
        {
            process(eventArgs);
        }

        private void process(string eventArgs)
        {
            try
            {
                String[] args = Regex.Split(eventArgs, @"\s+");
            }
            catch (Exception ex)
            {
                throw (InvalidParameterException)ex;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}", RequestHandler.EventIdentString, TargetElementID, RequestTypeID);
        }
    }
}