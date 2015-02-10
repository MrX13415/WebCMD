using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WebCMD.Core;
using WebCMD.Net.IO;
using WebCMD.Net;
using WebCMD.Com;
using WebCMD.Util.Html;
using Microsoft.AspNet.SignalR;
using WebCMD.Core.Net.SignalR;
using System.Reflection;
using System.Diagnostics;

namespace WebCMD.Core
{
    public partial class WebConsolePage : System.Web.UI.Page
    {
        public WebConsole WebConsole { get; protected set; }
        public Guid GUID;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack){
                ProcessPageLoad(sender, e);
            }

            Debug.WriteLine(" (i)  Page Loaded ");
        }

        private void ProcessPageLoad(object sender, EventArgs e)
        {
            GUID = Guid.NewGuid(); //<==== http://www.codedigest.com/Articles/ASPNET/347_Pass_Values_from_CodeBehind_to_JavaScript_and_From_JavaScript_to_CodeBehind_in_ASPNet.aspx
            Client client = Client.Create(GUID);

            //init ...
            WebConsole = WebConsole.Instance(GUID);

            if (RequestHandler.GetListener.Count == 0)
            {
                RequestHandler.AddListener(OnCommandRequest, typeof(CommandRequest));
                RequestHandler.AddListener(OnUpdateRequest, typeof(UpdateEvent));
            }

            string querypath = "";

            try
            {
                querypath = Request.QueryString.Get("p");

                WebConsole.WorkingDir = querypath;
            }
            catch { }

            //process query here
            //make sure p is valid!

            //WebConsole.UpdateHeaderMessage(client);

            ConsoleDebug.InnerHtml += String.Format("<span class=\"yellow\">[Site-Name: </span><span class=\"blue\">{0}</span><span class=\"yellow\">]</span>\n", HostingEnvironment.SiteName);
            ConsoleDebug.InnerHtml += String.Format("<span class=\"yellow\">[VirtualPath: </span><span class=\"blue\">{0}</span><span class=\"yellow\">]</span>\n", WebConsole.WorkingDir);
            ConsoleDebug.InnerHtml += String.Format("<span class=\"yellow\">[WebConsole: </span><span class=\"blue\">{0}</span><span class=\"yellow\">]</span>\n", WebConsole);

            ComLoader.Load();
        }

        public static string _HTMLTemplate_ServerMessage = "<div class=\"console-line msg-server\">\n<span>{0}</span><br><div style=\"margin-left: 60px;\">{1}</div>\n</div>";


        public void OnCommandRequest(ServerRequest ev)
        {
            CommandRequest e = (CommandRequest)ev;
            ComHandler.ProcessCommand(e);
        }

        public void OnUpdateRequest(ServerRequest ev)
        {
            
        }

        /**********************
        if (e.Command.ToLower().Equals("dir") || e.Command.ToLower().Equals("ls"))
            {
                if (WebConsole.CurrentPhysicalPath != null)
                {
                    cmdOut += "<div>\n<span>Directory Content:</span>\n</div>\n<br>\n";
                    foreach (DirectoryInfo dir in WebConsole.CurrentPhysicalPath.GetDirectories())
                    {
                        cmdOut += "<div>\n<span class=\"green\" style=\"margin-left: 50px;\">" + dir.Name + "/</span>\n</div>\n";
                    }

                    foreach (FileInfo file in WebConsole.CurrentPhysicalPath.GetFiles())
                    {
                        cmdOut += "<div>\n<span class=\"blue\" style=\"margin-left: 50px;\">" + file.Name + "</span>\n</div>\n";
                    }
                }
                else if (WebConsole.InvalidPath)
                {
                    cmdOut = "<div>\n<span class=\"magenta\">Server: Invalid server path or not allowed to view the directory content!</span>\n</div>\n";
                }
            }
            else if (e.Command.ToLower().Equals("todo") || e.Command.ToLower().Equals("bug") || e.Command.ToLower().Equals("bugs"))
            {

                try
                {
                    string[] args = Regex.Split(e.Arguments.Trim(), "\\s+");

                    if (args.Length == 1)
                    {
                        List<TODOObj> l = getList();

                        foreach (TODOObj o in l)
                        {
                            cmdOut += getO(o);
                        }
                    }
                    else if (args.Length >= 2 && args[0].ToLower().Equals("add") || args[0].ToLower().Equals("report") || args[0].ToLower().Equals("-r") || args[0].ToLower().Equals("-a"))
                    {
                        List<TODOObj> l = getList();

                        TODOObj o = new TODOObj(e.Arguments.Substring(args[0].Length).Trim());
                        o.ID = l[l.Count - 1].ID + 1;
                        o.State = TODOObj.BugSate.NEW;

                        l.Add(o);                        
                        saveList(l);
                        cmdOut += getO(o);
                    }
                    else if (args.Length >= 2 && args[0].ToLower().Equals("fixed") || args[0].ToLower().Equals("-f"))
                    {
                        List<TODOObj> l = getList();
                        int n = Int32.Parse(args[1]);

                        foreach (TODOObj o in l)
                        {
                            if (o.ID == n)
                            {
                                o.State = TODOObj.BugSate.FIXED;
                                cmdOut += getO(o);
                            }
                        }

                        saveList(l);
                    }
                    else if (args.Length >= 2 && args[0].ToLower().Equals("invalid") || args[0].ToLower().Equals("-i"))
                    {
                        List<TODOObj> l = getList();
                        int n = Int32.Parse(args[1]);

                        foreach (TODOObj o in l)
                        {
                            if (o.ID == n)
                            {
                                o.State = TODOObj.BugSate.INVALID;
                                cmdOut += getO(o);
                            }
                        }

                        saveList(l);
                    }
                    else if (args.Length >= 2 && args[0].ToLower().Equals("new") || args[0].ToLower().Equals("-n"))
                    {
                        List<TODOObj> l = getList();
                        int n = Int32.Parse(args[1]);

                        foreach (TODOObj o in l)
                        {
                            if (o.ID == n)
                            {
                                o.State = TODOObj.BugSate.NEW;
                                cmdOut += getO(o);
                            }
                        }

                        saveList(l);
                    }
                    else if (args.Length >= 2 && args[0].ToLower().Equals("remove") || args[0].ToLower().Equals("-rm"))
                    {
                        List<TODOObj> l = getList();
                        int n = Int32.Parse(args[1]);

                        foreach (TODOObj o in l)
                        {
                            if (o.ID == n)
                            {
                                cmdOut += "<div class=\"red\">Removed: </div>" + getO(o);
                                l.Remove(o);
                                break;
                            }
                        }

                        saveList(l);
                    }
                }
                catch (Exception ex)
                {

                    cmdOut += "<div class=\"console-line\">\n<span class=\"red\">Error: " + ex + "</span></div>";
                }

                
            }
            else cmdOut = "<div class=\"console-line\">\n<span class=\"red\">Error: Unknow Command: \'</span><span class=\"lightgray\">" + e.Command + "</span><span class=\"red\">\'</span></div>";

            foreach (Exception ex in CmdError.GetAll)
            {
                cmdOut += String.Format("<br><div class=\"magenta\">\n<span>Server: {0} Error: {1}: {2}</span><br><div style=\"margin-left: 60px;\">{3}</div>\n</div>\n",
                    CmdError.GetAll.IndexOf(ex).ToString("D3"), ex.GetType(), ex.Message, ex.StackTrace);
            }

            cbout += cmdOut;

            //Register Label2 control with current DataTime as DataItem


            string tmpl = "_RS:{0}:{1}:{2}";

            cbout = String.Format(tmpl, ConsoleOutput.ClientID, "_AddHTML", cbout);

            ScriptManager1.RegisterDataItem(ConsoleOutput, cbout);
        }

        public string getO(TODOObj o)
        {
            string tmpla = "<div><span class=\"yellow\" style=\"display: table-cell;\">{0} </span><span class=\"{1}\" style=\"display: table-cell;\">{2} </span><span style=\"display: table-cell;\">{3}</span><div>";
            string s = String.Format("{0, -8}", o.State).Replace(" ", "&nbsp;");
            string n = String.Format("{0, -6}", o.ID.ToString("D4")).Replace(" ", "&nbsp;");
            return String.Format(tmpla, n, o.State == TODOObj.BugSate.INVALID ? "red" :
                o.State == TODOObj.BugSate.FIXED ? "green-light" : "blue-light", s, o.Description);
        }

        private List<TODOObj> getList()
        {
            int counter = 0;
            string line;
            List<TODOObj> l = new List<TODOObj>();

            System.IO.StreamReader file = null;
            try
            {
                // Read the file and display it line by line.
                file = new System.IO.StreamReader(@"C:\Servers\Web\WEBCMD-TODO-LIST.txt");
                while ((line = file.ReadLine()) != null)
                {
                    int id = Int32.Parse(line.Substring(0, line.IndexOf("::")));
                    string state = line.Substring(line.IndexOf("::") + 2, line.IndexOf("::", line.IndexOf("::") + 2) - (line.IndexOf("::") + 2));
                    string d = line.Substring(line.IndexOf("::", line.IndexOf("::")+1)+2);
                    TODOObj o = new TODOObj(d);
                    o.ID = id;
                    if (state.Equals(TODOObj.BugSate.NEW.ToString())) o.State = TODOObj.BugSate.NEW;
                    if (state.Equals(TODOObj.BugSate.FIXED.ToString())) o.State = TODOObj.BugSate.FIXED;
                    if (state.Equals(TODOObj.BugSate.INVALID.ToString())) o.State = TODOObj.BugSate.INVALID;
                    l.Add(o);

                    counter++;
                }

                file.Close();
            }
            catch (Exception ex)
            {
                if (file != null) file.Close();
            }

            return l;
        }

        private void saveList(List<TODOObj> l)
        {
            // Read the file and display it line by line.
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Servers\Web\WEBCMD-TODO-LIST.txt");
           
            foreach (TODOObj o in l)
            {
                string s = o.ToString();
                file.WriteLine(s);    
            }
            
            file.Close();
        }

        public class TODOObj
        {
            public enum BugSate{
                FIXED, NEW, INVALID
            }
            public int ID { get; set; }
            public String Description { get; set; }
            public BugSate State { get; set; }

            public TODOObj(string txt)
            {
                State = BugSate.NEW;
                Description = txt;
                ID = 0;
            }

            public override string ToString()
            {
                return String.Format("{0}::{1}::{2}", ID, State, Description);
            }
        }
        ***********************************/

    }
}