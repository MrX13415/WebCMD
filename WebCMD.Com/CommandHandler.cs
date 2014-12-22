using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net;
using WebCMD.Net.IO;
using System.Threading;
using WebCMD.Util.Html;

namespace WebCMD.Com
{
    public class CommandHandler
    {
        private static List<CommandBase> cmdlist = new List<CommandBase>();
        public static CommandBase[] CommandList
        {
            get { return cmdlist.ToArray(); }
        }

        internal static void Register(CommandBase cmd)
        {
            if (Exists(cmd))
                throw new CommandAlreadyRegisteredException("Command already known and can't be registered again.");
            
            switch (cmd.Type)
            {
                case CommandBase.CommandType.ServerCommand:

                    break;
                case CommandBase.CommandType.ClientCommand:

                    break;
                default:
                    throw new InvalidParameterException("The command type if invalid!");
            }

            cmdlist.Add(cmd);
        }

        public static bool ProcessCommand(CommandRequest e)
        {
            int responds = e.Source.Response.GetQueueSize;
            bool returnval = false;
            bool match = false;

            for (int index = 0; index < cmdlist.Count; index++)
            {
                CommandBase cmd = cmdlist[index];
                match = cmd.Check(e.Command);

                if (match && cmd.Type == CommandBase.CommandType.ServerCommand)
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

        public static bool Exists(CommandBase cmd)
        {
            for (int index = 0; index < cmdlist.Count; index++)
            {
                CommandBase ccmd = cmdlist[index];

                if (ccmd.GetType().Name.Equals(cmd.GetType().Name))
                    return true;
            }
            return false;
        }

        public static void Clear()
        {
            cmdlist.Clear();
        }

        private static void printUnknowCommandError(CommandRequest e)
        {
            string errstr = CmdMessage.Get(CmdMessage.Type.Error, @"Error: Unknow Command: '", Color.White(e.Command) , @"</span>'");
            e.Source.Response.Send(ResponseHandler.CreateOutputResponse(errstr));
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