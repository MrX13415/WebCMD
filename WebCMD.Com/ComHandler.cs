﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net;
using WebCMD.Net.IO;
using System.Threading;
using WebCMD.Util.Html;

namespace WebCMD.Com
{
    public class ComHandler
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

            cmdlist.Add(cmd);
        }

        public static bool ProcessCommand(CommandRequest e)
        {
            //int responds = e.Source.Response.GetQueueSize;
            bool returnval = false;
            bool match = false;
            string input = e.Command;

            for (int index = 0; index < cmdlist.Count; index++)
            {
                Command cmd = cmdlist[index];
                match = cmd.Match(input);

                if (!match) continue;

                returnval = cmd.Execute(e);
                break;
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

        public static void Clear()
        {
            cmdlist.Clear();
        }

        private static void printUnknowCommandError(CommandRequest e)
        {
            string errstr = CmdMessage.Get(CmdMessage.Type.Error, @"Error: Unknow Command: '", Color.RedLight(e.Command) , @"</span>'");
            e.Source.Response.Send(errstr);
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