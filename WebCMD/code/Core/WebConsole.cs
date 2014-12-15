using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WebCMD.Net;
using WebCMD.Util;
using WebCMD.Util.WebConfig;

namespace WebCMD.Core
{
    public partial class WebConsole
    {
        private static Dictionary<Guid, WebConsole> _clientInstances = new Dictionary<Guid, WebConsole>();
        
        public bool DebugMode { get; set; }

        public HttpContext HttpContext { get; private set; }

        public WebConfiguration WebConfiguration { get; private set; }

        public string PreviousVirtualPath { get; private set; }

        private string _CurrentVirtualPath = "";
        public string CurrentVirtualPath
        {
            get { return _CurrentVirtualPath; }
            set { CheckVirtualPath(value); }
        }

        public DirectoryInfo CurrentPhysicalPath
        {
            get { return InvalidPath ? null : new DirectoryInfo(HttpContext.Server.MapPath(CurrentVirtualPath)); }
        }
        
        public bool InvalidPath { get; private set; }


        public WebConsole()
        {
            HttpContext = HttpContext.Current;
            // init default path ...
            CurrentVirtualPath = "";
        }

        public static WebConsole Instance(Guid guid)
        {
            if (guid == null) return null;

            WebConsole con = null;

            try
            { 
                if (_clientInstances.ContainsKey(guid)) 
                    con = _clientInstances[guid] as WebConsole;
            }
            catch{ }

            if (con == null)
            {
                con = new WebConsole();
                _clientInstances[guid] = con;
            }

            return con;
        }

        private void CheckVirtualPath(string vpath)
        {
            //Backup current path and set the the new path ...
            PreviousVirtualPath = _CurrentVirtualPath;
            _CurrentVirtualPath = GetFormatedVirtualPath(vpath);

            //reset validation ...
            InvalidPath = false;

            WebConfiguration cfg = new WebConfiguration(_CurrentVirtualPath);

            //Only a virtual path with an existing physical path and with "directory browse" enabled are allowed ...
            if (cfg.DirectoryBrowse.Enabled && CurrentPhysicalPath.Exists)
            {
                //path is valid ...
                WebConfiguration = cfg;
                return;
            }

            //path is invalid! Return to the previous path ...
            InvalidPath = true;
           // _CurrentVirtualPath = PreviousVirtualPath;
        }

        private string GetFormatedVirtualPath(string vpath)
        {
            string nvp = vpath == null ? "" : vpath;
            nvp = nvp.Trim();  
            // "nvp\nvp" => "nvp/nvp"
            nvp = nvp.Replace("\\", "/");
            // "/nvp" => "nvp"
            nvp = nvp.TrimStart('/');
            // "nvp/" => "nvp"
            nvp = nvp.TrimEnd('/');

            //make sure there are no invalid chars ...
            foreach (char c in Path.GetInvalidPathChars())
            {
                if (nvp.Contains(c))
                {
                    nvp = "";
                    break;
                }
            }
            
            return nvp;
        }
    }
}