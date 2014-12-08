using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebCMD.Util.Html
{
    public static class HTML
    {
        public const string _Template_StdConsoleLine = "<div class=\"console-line\">{0}</div>";
        public const string _Template_StdConsoleLine_Class = "<div class=\"console-line {0}\">{1}</div>";

        public static string Encode(params string[] s)
        {
            return HttpUtility.HtmlEncode(String.Concat(s)).Replace(" ", "&nbsp;");
        }

        public static string CreateConsoleLine(params string[] s)
        {
            return String.Format(_Template_StdConsoleLine, String.Concat(s));
        }
        public static string CreateConsoleLineClass(string cssClass, params string[] s)
        {
            return String.Format(_Template_StdConsoleLine_Class, cssClass, String.Concat(s));
        }
    }

    
}
