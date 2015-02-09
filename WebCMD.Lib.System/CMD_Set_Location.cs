using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCMD.Com;
using WebCMD.Core;
using WebCMD.Core.IO;
using WebCMD.Net;
using WebCMD.Util.Html;

// Version 1.0
namespace WebCMD.Lib.System
{
    class CMD_Set_Location : Command
    {

        public CMD_Set_Location()
        {
            Label = "Set-Location";
            SetAliase("cd", "srvcd");
        }

        protected override bool _Execute(Net.IO.CommandRequest e)
        {
            string[] args = e.ArgumentList;

            Client client = e.Source;
            WebConsole cmd = WebConsole.Instance(client.GUID);
            
            if (args.Length <= 0)
            {
                //write out the current directory ...
                client.Response.Send(Color.Get("yellow", "Current Directory: "), Color.Get("blue-light", cmd.CurrentVirtualPath));
            }
            else
            {
                string path0 = args[0].Trim();
                
                //determine if the path was put into quotes ...
                char quoteChar = path0.Contains('\"') ? '\"' : path0.Contains('\'') ? '\'' : '0';
                bool quote = quoteChar != '0';

                // Handle ".../bla h/..." and '.../bla h/...'
                if (quote)
                {
                    path0 = e.ArgumentString.Substring(1); //remove the "char" from the beginning
                    int eIndex = path0.IndexOf(quoteChar); //get the index of the ending "char"
                    path0 = path0.Substring(0, eIndex);    //retrive the actuall path
                }

                cmd.ChangeDirectory(path0);

                if (cmd.InvalidPath)
                {
                    //send invalid path message ...
                    client.Response.Send(CmdMessage.GetErrorMessage("Invalid path: '", Color.White(path0), "'"));
                }

                //update current path display
                cmd.UpdateHeaderMessage(client);
            }

            return true;
        }
    }
}
