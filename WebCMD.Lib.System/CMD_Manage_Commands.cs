using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net.IO;
using WebCMD.Net;
using System.Threading;
using WebCMD.Util;
using WebCMD.Com;
using WebCMD.Util.Html;

namespace WebCMD.Lib.System
{
    public class CMD_Manage_Commands : Command
    {
        public CMD_Manage_Commands()
        {
            Label = "Manage-Commands";
            SetAliase("cmds", "commands", "mcmds");
        }

        protected override bool _Execute(CommandRequest e)
        {
            string[] args = e.ArgumentList;
            Client client = e.Source;

            bool r = false;

            switch (args[0].ToLower())
            {
                case "list":
                    r = List(e);
                    break;
                case "reload":
                    r = Load(e);
                    break;
                case "load":
                    r = Load(e);
                    break;
                case "unload":
                    r = Unload(e);
                    break;
                default:
                    client.Response.Send(ResponseHandler.CreateOutputResponse("<div class=\"console-line msg-error\">Bad arguments</div>" +
                        "<div>Manage-Commands list|(re)load|unload</div>"));
                    break;
            }

            return r;
        }

        private bool List(CommandRequest e)
        {
            e.Source.Response.Send(Color.Get("blue", "Available server commands:"));

            int index = 0;
            foreach (Command com in ComHandler.CommandList)
            {
                index++;
                string libline = "    #{0:D4} {1,-20} {2,-30} {3} {4}";

                string sout = String.Format(libline, index, com.Label, string.Join(", ", com.Aliase), com.Description, com.Library.File.Name);
                e.Source.Response.Send(Color.Get("yellow", HTML.Encode(sout)));
            }


            //try
            //{
            //    //ComLoader.Unload();
            //    e.Source.Response.Send(CmdMessage.GetSuccessMessage("Unloaded successfully!"));
            //}
            //catch (Exception ex)
            //{
            //    e.Source.Response.Send(CmdMessage.GetErrorMessage("Faild: ", ex.ToString()));
            //}
            return true;
        }

        private bool Unload(CommandRequest e)
        {
            //e.Source.Response.Send(CmdMessage.GetServerMessage("Unloading server command libraries ... "));

            //try
            //{
            //    //ComLoader.Unload();
            //    e.Source.Response.Send(CmdMessage.GetSuccessMessage("Unloaded successfully!"));
            //}
            //catch (Exception ex)
            //{
            //    e.Source.Response.Send(CmdMessage.GetErrorMessage("Faild: ", ex.ToString()));
            //}

            return true;
        }

        private bool Load(CommandRequest e)
        {
            e.Source.Response.Send(CmdMessage.GetServerMessage("Loading server command libraries ... "));

            ProgressInfo pi = ComLoader.ProgressInfo;
            ComLoader.Load();
            do
            {
                lock (ComLoader.ProgressInfo)
                {
                    ComLoader.Wait();
                    e.Source.Response.Send(CmdMessage.Get(ComLoader.ProgressInfo.Library.IsValid ? CmdMessage.Type.Success : CmdMessage.Type.Server, HTML.Encode(pi.ToString())));
                }
            } while (pi.IsAlive);

            e.Source.Response.Send(CmdMessage.GetSuccessMessage(String.Concat("Loading server command libraries completed ... (", ComHandler.CommandList.Length, " commands loaded)")));

            return true;
        }
    }
}