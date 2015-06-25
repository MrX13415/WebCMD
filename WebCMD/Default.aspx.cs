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
        public Terminal cmd { get; protected set; }
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
            cmd = Terminal.Instance(GUID);

            if (RequestHandler.GetListener.Count == 0)
            {
                RequestHandler.AddListener(OnCommandRequest, typeof(CommandRequest));
            }

            string querypath = "";

            try
            {
                querypath = Request.QueryString.Get("p");

                cmd.WorkingDir = querypath;
            }
            catch { }

            //process query here
            //make sure p is valid!

            //WebConsole.UpdateHeaderMessage(client);

            ComLoader.Load();
        }

        //public static string _HTMLTemplate_ServerMessage = "<div class=\"console-line msg-server\">\n<span>{0}</span><br><div style=\"margin-left: 60px;\">{1}</div>\n</div>";
        
        public void OnCommandRequest(ServerRequest ev)
        {
            CommandRequest req = (CommandRequest)ev;
            Terminal terminal = Terminal.Instance(req.Source.GUID);
            terminal.RunCommand(req);

            //cmd.UpdateDebugHeader(ev.Source, "Site-Name", HostingEnvironment.SiteName);
            //cmd.UpdateDebugHeader(ev.Source, "Terminal", cmd.ToString());
        }

    }
}