using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WebCMD.Util.WebConfig.Section;

namespace WebCMD.Util.WebConfig
{
    public class WebConfiguration
    {
        public string VirtualPath { get; private set; }

        private ServerManager servermanager = new ServerManager();
        private Configuration config;

        public WebConfiguration(string virtualpath)
        {
            VirtualPath = virtualpath;

            config = servermanager.GetWebConfiguration(HostingEnvironment.SiteName, VirtualPath);
        }

        public bool Exists
        {
            get { return config != null; }
        }

        public DirectoryBrowseSection DirectoryBrowse
        {
            get { return new DirectoryBrowseSection(config); }
        }

    }
}