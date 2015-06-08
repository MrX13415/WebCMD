using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net;
using WebCMD.Net.IO;
using System.Threading;
using WebCMD.Util.Html;
using WebCMD.Util;

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

        public static ComProcess ProcessCommand(CommandRequest e)
        {
            ComProcess process = null;
            bool match = false;
            string input = e.Command;

            for (int index = 0; index < cmdlist.Count; index++)
            {
                Command cmd = cmdlist[index];
                match = cmd.Match(input);

                if (!match) continue;

                ServerResponse rs = new ServerResponse(Ref.ConsoleInputID);
                rs.Property = "style.visibility";
                rs.SetData("hidden");
                rs.Mode = ServerResponse.PropertyMode.Set;
                //e.Source.Response.Send(rs);

                process = cmd.Execute(e);

                break;
            }

            if (!match) printUnknowCommandError(e);

            return process;
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
            string errstr = CmdMessage.Get(CmdMessage.Type.Error, @"Error: Unknow Command: '", Color.RedLight(e.Command) , @"'");
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