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

        public static char DirectorySeparatorChar = '/';

        private string _CurrentVirtualPath = "/";                               // The current virtual path

        public bool DebugMode { get; set; }

        public HttpContext HttpContext { get; private set; }                    // The current HTTP Context
        public WebConfiguration WebConfiguration { get; private set; }          // The web-config of the current path
        public string SystemRootDirectory { get; set; }                         // The root directory of the console
        public string PreviousVirtualPath { get; private set; }                 // The previous virtual path
        public bool InvalidPath { get; private set; }                           // The current path is invalid



        public WebConsole()
        {
            HttpContext = HttpContext.Current;

            // init default ...
            CurrentVirtualPath = VirtualPath.DirSeparatorChar.ToString();
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

        public string CurrentVirtualPath                                        
        {
            get { return _CurrentVirtualPath; }
            set { ChangeDirectory(value); }
        }
        public DirectoryInfo CurrentPhysicalPath
        {
            get { return InvalidPath ? null : new DirectoryInfo(HttpContext.Server.MapPath(CurrentVirtualPath)); }
        }


        public static string CombinePath(string path0, string path1)
        {
            string p0 = path0.Trim();
            string p1 = path1.Trim();
            
            if (p0.EndsWith("/") || p0.EndsWith("\\")) return p0 + p1;
            else  return p0 + "/" + p1;
        }

        public void ChangeDirectory(string vpath)
        {
            //tirm
            vpath = vpath.Trim();

            // Handle current dir
            if (VirtualPath.Compare(CurrentVirtualPath, vpath)) return;
            // Handle current dir alias
            if (vpath.Equals(VirtualPath.CurrentDirAlias)) return;
            // Handle parrent dir alias
            if (vpath.Equals(VirtualPath.ParentDirAlias)) vpath = VirtualPath.GetParentPath(vpath);
            // Handle current dir alias at the begining of a path
            if (vpath.StartsWith(String.Concat(VirtualPath.CurrentDirAlias, VirtualPath.DirSeparatorChar)))
                vpath = VirtualPath.Combine(CurrentVirtualPath, vpath.Substring(2));
            // Handle relative path
            if (!VirtualPath.IsFullPath(vpath)) vpath = CombinePath(CurrentVirtualPath, vpath);
            // Handle parrent dir alias in path 
            vpath = VirtualPath.GetSimplifiedPath(vpath);
            // Try to find a matching path for the given path-like string
            vpath = VirtualPath.GetSmartPath(CurrentVirtualPath, vpath);

            //check config
            WebConfiguration cfg = new WebConfiguration(vpath);

            //Only a virtual path with an existing physical path and with "directory browse" enabled are allowed ...
            if (cfg.DirectoryBrowse.Enabled && VirtualPath.Exists(vpath))
            {
                //reset validation ...
                InvalidPath = false;

                //path is valid ...
                WebConfiguration = cfg;

                //Backup current path and set the the new path ...
                PreviousVirtualPath = _CurrentVirtualPath;
                _CurrentVirtualPath = vpath;

                return;
            }
           
            //path is invalid! Return to the previous path ...
            InvalidPath = true;
        }


        /*
         
          //Backup current path and set the the new path ...
            PreviousVirtualPath = _CurrentVirtualPath;
            _CurrentVirtualPath = GetFormatedVirtualPath(vpath);

            //reset validation ...
            InvalidPath = false;

            

            //check config
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
         * 
         */

        public void UpdateHeaderMessage(Client client)
        {
            string html = string.Format("<div></br><span class=\"orange\">WebCMD v1.0 </span><span class=\"white\">-- CMD.ICELANE.NET{0}</br></br></span></div>\n", CurrentVirtualPath);
            ServerResponse rs = new ServerResponse(Ref.ConsoleHeaderID);
            rs.SetData(html);
            rs.Mode = ServerResponse.PropertyMode.Set;
            client.Response.Send(rs);
        }
    }
}