using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCMD.Com
{
    public class CommandBase
    {
        public enum CommandType
        {
            ServerCommand, ClientCommand
        }

        public CommandType Type { get; protected set; }

        public bool Check(string command)
        {
            switch (Type)
            {
                case CommandType.ServerCommand:
                    return CheckServerCommand(command);
                case CommandType.ClientCommand:
                    return CheckClientCommand(command);
                default:
                    return false;
            }
        }

        private bool CheckServerCommand(string command)
        {
            ServerCommand cmd = (ServerCommand)this;

            //check label
            if (cmd.Label.Trim().ToLower().Equals(command.Trim().ToLower()))
                return true;

            //check aliase
            foreach (string a in cmd.Aliase)
            {
                if (a.Trim().ToLower().Equals(command.Trim().ToLower()))
                    return true;
            }

            return false;
        }

        private bool CheckClientCommand(string command)
        {

            return false;
        }
    }
}