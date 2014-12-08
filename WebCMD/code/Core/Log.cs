using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCMD.Core
{
    public class Log
    {
        private static List<string> log = new List<string>();

        public static void Add(string t)
        {
            log.Add(t);
        }

        public static List<string> Get
        {
            get { return log;  }
        }
    }
}