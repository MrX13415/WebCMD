﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net.IO;
using WebCMD.Net;
using WebCMD.Com;
using WebCMD.Core;
using System.IO;
using WebCMD.Core.IO;

namespace WebCMD.Lib.System
{
    public class CMD_Get_ChildItem : Command
    {
        //ls, dir, gci

        public CMD_Get_ChildItem()
        {
            Label = "Get-ChildItem";
            SetAliase("ls", "dir", "gci");
        }

        protected override bool _Execute(CommandRequest e)
        {
            string[] args = e.ArgumentList;
            ServerResponse response = ResponseHandler.NewOutputResponse;

            Client client = e.Source;
            Terminal terminal = Terminal.Instance(client.GUID);

            string cmdOut = "";
            if (terminal.ServerWorkingDir != null)
            {
                cmdOut += "<div>\n<span class=\"yellow\">Directory of: </span><span class=\"blue-light\">" + terminal.WorkingDir + "</span>\n</div>\n<br>\n";
                foreach (DirectoryInfo dir in terminal.ServerWorkingDir.GetDirectories())
                {
                    cmdOut += "<div>\n<span class=\"blue-light\" style=\"margin-left: 50px;\">" + dir.Name + "/</span>\n</div>\n";
                }

                foreach (FileInfo file in terminal.ServerWorkingDir.GetFiles())
                {
                    cmdOut += "<div>\n<span class=\"magenta\" style=\"margin-left: 50px;\">" + file.Name + "</span>\n</div>\n";
                }
            }
            else if (VirtualPath.Exists(terminal.WorkingDir))
            {
                cmdOut = "<div>\n<span class=\"magenta\">Server: Invalid server path or not allowed to view the directory content!</span>\n</div>\n";
            }
            client.Response.Send(cmdOut);

            return true;
        }

    }
}