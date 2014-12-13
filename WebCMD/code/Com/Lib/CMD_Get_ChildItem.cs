using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net.Event;
using WebCMD.Net;
using WebCMD.Com;

namespace WebCMD.Com.Lib
{
    public class CMD_Get_ChildItem : ServerCommand
    {
        //ls, dir, gci

        public CMD_Get_ChildItem()
        {
            Label = "Get-ChildItem";
            SetAliase("ls", "dir", "gci");
        }

        protected override bool Process(CommandEvent e)
        {
            string[] args = e.ArgumentList;
            ServerResponse response = ResponseHandler.NewOutputResponse;


            return true;
        }

    }
}