using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using WebCMD.Net.IO;
using WebCMD.Util.Html;

namespace WebCMD.Com
{
    public abstract class ServerCommand : CommandBase
    {
       
	    public string Label { get; protected set; }
        private string[] _Aliase = new string[0];
        public string[] Aliase { get { return _Aliase; } }
        protected void SetAliase(params string[] alias) { _Aliase = alias; }
        public Thread Thread { get; private set; }
        public CommandRequest Request { get; set; }

        public ServerCommand()
        {
            this.Type = CommandType.ServerCommand;
        }

        public void Register()
        {
            CommandHandler.Register(this);
        }

	    public string Description { get; protected set; }
        public string Usage { get; protected set; }

        public bool Execute(CommandRequest e)
        {
            Request = e;
            Thread = new Thread(this._Run);
            Thread.Name = "WebCMD-COMMAND_" + this.GetType().Name;
            Thread.Start();
            
            return true;
        }

        public void _Run()
        {
            try
            {
                Process(Request);
            }
            catch (Exception ex)
            {
                Request.Source.Response.Send(CmdError.Get(ex));
            }
        }

        protected abstract bool Process(CommandRequest e);
	
    }
}