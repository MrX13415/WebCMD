using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WebCMD.Core.IO;
using WebCMD.Net;
using WebCMD.Net.IO;
using WebCMD.Util;
using WebCMD.Util.WebConfig;

namespace WebCMD.Core
{
    public partial class WebConsole
    {
        // Map of all clients ...
        private static Dictionary<Guid, WebConsole> _clientInstances = new Dictionary<Guid, WebConsole>();

        private string _WorkingDir = VirtualPath.DirSeparatorChar.ToString();   // The current virtual path

        public bool DebugMode { get; set; }

        public HttpContext HttpContext { get; private set; }                    // The current HTTP Context
        public WebConfiguration WebConfiguration { get; private set; }          // The web-config of the current path
        public string SystemRootDirectory { get; set; }                         // The root directory of the console
        public string PreviousWorkingDir { get; private set; }                 // The previous virtual path


        public WebConsole()
        {
            HttpContext = HttpContext.Current;

            // init default ...
            WorkingDir = VirtualPath.DirSeparatorChar.ToString();
        }

        public static WebConsole Instance(Guid guid)
        {
            if (guid == null) return null;
            WebConsole con = null;
            
            try
            { 
                if (_clientInstances.ContainsKey(guid)) con = _clientInstances[guid] as WebConsole;
            } catch { }

            if (con == null)
            {
                con = new WebConsole();
                _clientInstances[guid] = con;
            }

            return con;
        }

        public string WorkingDir                                        
        {
            get { return _WorkingDir; }
            set { ChangeDirectory(value); }
        }

        public DirectoryInfo ServerWorkingDir
        {
            get { return new DirectoryInfo(VirtualPath.GetServerPath(WorkingDir)); }
        }

        public bool ChangeDirectory(string vpath)
        {
            //tirm
            vpath = vpath.Trim();

            // Handle current dir
            if (VirtualPath.Compare(WorkingDir, vpath)) return false;
            // Handle current dir alias
            if (vpath.Equals(VirtualPath.CurrentDirAlias)) return false;
            // Handle parrent dir alias
            if (vpath.Equals(VirtualPath.ParentDirAlias)) vpath = VirtualPath.GetParentPath(WorkingDir);
            // Handle current dir alias at the begining of a path
            if (vpath.StartsWith(String.Concat(VirtualPath.CurrentDirAlias, VirtualPath.DirSeparatorChar)))
                vpath = VirtualPath.Combine(WorkingDir, vpath.Substring(2));
            // Handle relative path
            if (!VirtualPath.IsFullPath(vpath)) vpath = VirtualPath.Combine(WorkingDir, vpath);
            // Handle parrent dir alias in path 
            vpath = VirtualPath.GetSimplifiedPath(vpath);
            // Try to find a matching path for the given path-like string
            vpath = VirtualPath.GetSmartPath(WorkingDir, vpath);

            //check config
            WebConfiguration cfg = new WebConfiguration(vpath);

            //Only a virtual path with an existing physical path and with "directory browse" enabled are allowed ...
            if (VirtualPath.Exists(vpath))
            {
                //store the current web config
                WebConfiguration = cfg;
                //Backup current path
                PreviousWorkingDir = _WorkingDir;
                //set the new directory
                _WorkingDir = vpath;

                return true;
            }
           
            throw new ArgumentException("Invalid path!");
        }

        public bool Exists()
        {
            return VirtualPath.Exists(WorkingDir);
        }

        public void UpdateHeaderMessage(Client client)
        {
            string html = string.Format("<div></br><span class=\"orange\">WebCMD v1.0 </span><span class=\"white\">-- CMD.ICELANE.NET{0}</br></br></span></div>\n", WorkingDir);
            ServerResponse rs = new ServerResponse(Ref.ConsoleHeaderID);
            rs.SetData(html);
            rs.Mode = ServerResponse.PropertyMode.Set;
            client.Response.Send(rs);
        }
    }
}