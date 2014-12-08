using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCMD.Util.WebConfig.Section
{
    public class DirectoryBrowseSection
    {
        private ConfigurationSection section;

        public const string SectionPath = "system.webServer/directoryBrowse";

        public DirectoryBrowseSection(Configuration config)
        {
            section = config.GetSection(SectionPath);
        }

        public bool Enabled
        {
            get { return Boolean.Parse(section.GetAttributeValue("enabled").ToString()); }
        }

    }
}