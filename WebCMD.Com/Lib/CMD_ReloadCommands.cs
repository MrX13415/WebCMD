using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net.Event;
using WebCMD.Net;
using System.Threading;
using WebCMD.Util;
using WebCMD.Com;
using WebCMD.Util.Html;

namespace WebCMD.Com.Lib
{
    public class CMD_ReloadCommands : ServerCommand
    {
        public CMD_ReloadCommands()
        {
            Label = "Reload-Commands";
            SetAliase("reloadcmds", "reloadcommands");
        }

        protected override bool Process(CommandEvent e)
        {
            bool r = false;

                //switch (e.ArgumentList[0].ToLower())
                //{
                //    case "response":
                //        r = DoResponseTest(e);
                //        break;
                //    case "error":
                //        r = DoErrorTest(e);
                //        break;
                //    case "savelog":
                //        r = SaveDebugRequestLog(e);
                //        break;
                //    default:
                //        e.EventSource.Response.Send(ResponseHandler.CreateOutputResponse("<div class=\"console-line msg-error\">Bad arguments</div>" +
                //            "<div>Server-Test [response &lt;interval ms&gt; &lt;time ms&gt;|error|savelog]</div>"));
                //        break;
                //}

                Reload(e);

            
            return r;
        }

        private bool Reload(CommandEvent e)
        {

            ProgressInfo pi = ComLoader.ProgressInfo;
            ComLoader.LoadAsync();
            do
            {
                lock (ComLoader.ProgressInfo)
                {
                    ComLoader.Wait();
                    e.EventSource.Response.Send(CmdMessage.Get(ComLoader.ProgressInfo.Library.IsValid ? CmdMessage.Type.Success : CmdMessage.Type.Server, pi.ToString()));
                }
            } while (pi.IsAlive);

            e.EventSource.Response.Send(CmdMessage.Get(CmdMessage.Type.Server, pi.ToString()));

            return true;
        }
    }
}