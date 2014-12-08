using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WebCMD.Net.Event
{
    public class CommandEvent : RequestEvent
    {

        public string Command { get; set; }
        private string _Arguments = "";
        public string ArgumentString
        {
            get { return _Arguments; }
            set
            {
                ArgumentList = Regex.Split(value.Trim(), @"\s+");
                _Arguments = value.Trim();
            }
        }

        public string[] ArgumentList { get; private set; }


        private CommandEvent(Client client, string targetid) : base(client, targetid, typeof(CommandEvent))
        {
        }

        public CommandEvent(Client client, string targetid, string command, string args) : this(client, targetid)
        {
            this.Command = command;
            this.ArgumentString = args;
        }

        /// <summary>
        /// jsEventArgs = "_CommandEvent <command> <args>"
        /// Spaced by tabs
        /// see ToString();
        /// </summary>
        /// <param name="jsEventArgs">_CommandEvent <command> <args></param>
        public CommandEvent(Client client, string targetid, string eventArgs) : this(client, targetid)
        {
            process(eventArgs);
        }

        private void process(string eventArgs)
        {
            try
            {
                String[] args = Regex.Split(eventArgs, @"\s+");

                this.Command = args[0].Trim();
                this.ArgumentString = eventArgs.Substring(this.Command.Length).Trim();
            }
            catch (Exception ex)
            {
                throw (InvalidParameterException)ex;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}:{3} {4}", RequestHandler.EventIdentString, TargetElementID, EventTypeID, Command, ArgumentString);
        }
    }
}