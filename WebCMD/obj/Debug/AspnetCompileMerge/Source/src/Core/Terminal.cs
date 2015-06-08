using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using WebCMD.Com;
using WebCMD.Core.IO;
using WebCMD.Net;
using WebCMD.Net.IO;
using WebCMD.Util;
using WebCMD.Util.Html;
using WebCMD.Util.WebConfig;


//TODO: DEBUG: Add detailed message for the dir state and such (e.g. why is it an invalid path?)
//TODO: Improve root dir loc. determination. (see dev-branch)


namespace WebCMD.Core
{
    public partial class Terminal
    {
        // Map of all clients ...
        private static Dictionary<Guid, Terminal> _clientInstances = new Dictionary<Guid, Terminal>();

        private Dictionary<string, string> _debugItems = new Dictionary<string, string>();

        private string _WorkingDir = VirtualPath.DirSeparatorChar.ToString();   // The current virtual path
        private ComProcess _CurrentProcess = null;

        public bool DebugMode { get; set; }

        public HttpContext HttpContext { get; private set; }                    // The current HTTP Context
        public WebConfiguration WebConfiguration { get; private set; }          // The web-config of the current path
        public string SystemRootDirectory { get; set; }                         // The root directory of the console
        public string PreviousWorkingDir { get; private set; }                 // The previous virtual path


        public Terminal()
        {
            HttpContext = HttpContext.Current;

            // init default ...
            WorkingDir = VirtualPath.DirSeparatorChar.ToString();
        }

        public bool RunCommand(CommandRequest req)
        {
            if (_CurrentProcess != null)
                if (_CurrentProcess.Process.IsAlive) return false;

            ComProcess proccess = ComHandler.ProcessCommand(req);
            _CurrentProcess = proccess;

            if (proccess == null) return false;
            return true;
        }

        public static Terminal Instance(Guid guid)
        {
            if (guid == null) return null;
            Terminal con = null;

            // _clientInstances GUID not in list exception (?)

            if (_clientInstances.ContainsKey(guid)) con = _clientInstances[guid] as Terminal;

            if (con == null)
            {
                con = new Terminal();
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
            // Handle parent dir alias
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

            vpath = vpath.Trim();
            if (vpath.Length > 1 && vpath.EndsWith(VirtualPath.DirSeparatorChar.ToString()))
                vpath = vpath.Substring(0, vpath.Length - 1);

            //Only a virtual path with an existing physical path and with "directory browse" enabled are allowed ...
            if (VirtualPath.Exists(vpath))
            {
                //store the current web config
                WebConfiguration = new WebConfiguration(vpath);
                //Backup current path
                PreviousWorkingDir = _WorkingDir;
                //set the new directory
                _WorkingDir = vpath;

                return true;
            }
           
            throw new ArgumentException("Invalid path: '" + vpath + "'");
        }

        public void UpdateHeaderMessage(Client client)
        {
            string html = string.Format("<div></br><span class=\"orange\">WebCMD v1.0 </span><span class=\"white\">-- CMD.ICELANE.NET{0}</br></br></span></div>\n", WorkingDir);
            ServerResponse rs = new ServerResponse(Ref.ConsoleHeaderID);
            rs.SetData(html);
            rs.Mode = ServerResponse.PropertyMode.Set;
            rs.UseAnimation = false;
            client.Response.Send(rs);
        }

        public void UpdateDebugHeader(Client client, string itemName, string htmlContent)
        {
            _debugItems[itemName] = htmlContent;

            //ConsoleDebug.InnerHtml += String.Format("<span class=\"yellow\">[Site-Name: </span><span class=\"blue-light\">{0}</span><span class=\"yellow\">]</span>\n", HostingEnvironment.SiteName);
            //ConsoleDebug.InnerHtml += String.Format("<span class=\"yellow\">[VirtualPath: </span><span class=\"blue-light\">{0}</span><span class=\"yellow\">]</span>\n", cmd.WorkingDir);
            //ConsoleDebug.InnerHtml += String.Format("<span class=\"yellow\">[WebConsole: </span><span class=\"blue-light\">{0}</span><span class=\"yellow\">]</span>\n", cmd);

            string html = "";
            foreach (string key in _debugItems.Keys)
            {
                string item = _debugItems[key];
                html += string.Format("<span class=\"yellow\">[{0}: </span><span class=\"blue-light\">{1}</span><span class=\"yellow\">]</span<br/>", HTML.Encode(key), HTML.Encode(item));
            }

            ServerResponse rs = new ServerResponse(Ref.ConsoleDebugID);
            rs.SetData(html);
            rs.Mode = ServerResponse.PropertyMode.Set;
            rs.UseAnimation = false;
            client.Response.Send(rs);
        }

        public void UpdatePrompt(Client client)
        {
            string html = string.Format("<span class=\"red\">{0}</span>{1}", HTML.Encode(WorkingDir.Substring(0, 1)), HTML.Encode(WorkingDir.Substring(1)));
            ServerResponse rs = new ServerResponse(Ref.CmdPromtID);
            rs.SetData(html);
            rs.Mode = ServerResponse.PropertyMode.Set;
            rs.UseAnimation = false;
            client.Response.Send(rs);
        }

        public override string ToString()
        {
            string outstr = "";
            Type myType = this.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                object propValue = prop.GetValue(this, null);
                string propValStr = propValue != null ? propValue.ToString() : "(null)";
                outstr += String.Format("{0} = {1}  ", prop.Name, propValStr);
            }
            return outstr;
        }
    }
}