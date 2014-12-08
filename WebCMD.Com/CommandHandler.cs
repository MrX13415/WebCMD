using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net;
using WebCMD.Net.Event;
using System.Threading;
using WebCMD.Util.Html;

namespace WebCMD.Com
{
    public class CommandHandler
    {
        private static List<Command> cmdlist = new List<Command>();
        public static Command[] CommandList
        {
            get { return cmdlist.ToArray(); }
        }

        internal static void Register(Command cmd)
        {
            if (Exists(cmd))
                throw new CommandAlreadyRegisteredException("Command already known and can't be registered again.");
            
            switch (cmd.Type)
            {
                case Command.CommandType.ServerCommand:

                    break;
                case Command.CommandType.ClientCommand:

                    break;
                default:
                    throw new InvalidParameterException("The command type if invalid!");
            }

            cmdlist.Add(cmd);
        }

        public static bool ProcessCommand(CommandEvent e)
        {
            int responds = e.EventSource.Response.GetQueueSize;
            bool returnval = false;
            bool match = false;

            for (int index = 0; index < cmdlist.Count; index++)
            {
                Command cmd = cmdlist[index];
                match = cmd.Check(e.Command);

                if (match && cmd.Type == Command.CommandType.ServerCommand)
                {
                    ServerCommand scmd = (ServerCommand)cmd;
                    returnval = scmd.Execute(e);
                    break;
                }
            }

            if (!match) printUnknowCommandError(e);

            //make sure there is always a response ...
            //if (e.EventSource.Response.GetQueueSize == responds)
            //    e.EventSource.Response.Send(ResponseHandler.CreateOutputResponse(""));

            return returnval;
        }

        public static bool Exists(Command cmd)
        {
            for (int index = 0; index < cmdlist.Count; index++)
            {
                Command ccmd = cmdlist[index];

                if (ccmd.GetType().Name.Equals(cmd.GetType().Name))
                    return true;
            }
            return false;
        }

        private static void printUnknowCommandError(CommandEvent e)
        {
            string errstr = CmdMessage.Get(CmdMessage.Type.Error, @"Error: Unknow Command: '", Color.White(e.Command) , @"</span>'");
            e.EventSource.Response.Send(ResponseHandler.CreateOutputResponse(errstr));
        }
    }

    public class CommandAlreadyRegisteredException : Exception
    {
        public CommandAlreadyRegisteredException(string message) : base(message) { }
    }

    public class InvalidParameterException : Exception
    {
        public InvalidParameterException(string message) : base(message) { }
    }
}