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
    public abstract class Command
    {
       
	    public string Label { get; protected set; }
	    public string Description { get; protected set; }
        public string Usage { get; protected set; }

        private string[] _Aliase = new string[0];
        public string[] Aliase { get { return _Aliase; } }
        protected void SetAliase(params string[] alias) { _Aliase = alias; }
        
        public ComLibrary Library { get; internal set; }


        public void Register()
        {
            ComHandler.Register(this);
        }
                
        public bool Match(string inputCommand)
        {
            Command cmd = (Command)this;

            string label = cmd.Label.Trim().ToLower();
            string input = inputCommand.Trim().ToLower();
            
            //check label
            if (label.Equals(input))
                return true;

            //check aliase
            foreach (string cmdalias in cmd.Aliase)
            {
                string alias = cmdalias.Trim().ToLower();
                if (alias.Equals(input)) return true;
            }

            return false;
        }

        public ComProcess Execute(CommandRequest request)
        {
            ComProcess process = new ComProcess(this, request);
            process.Start(this._Run);
            return process;
        }

        private void _Run(object obj)
        {
            CommandRequest request = obj as CommandRequest;
            try
            {
                _Execute(request);
            }
            catch (Exception ex)
            {
                request.Source.Response.Send(CmdError.Get(ex));
            }
        }

        protected abstract bool _Execute(CommandRequest request);
	
    }
}