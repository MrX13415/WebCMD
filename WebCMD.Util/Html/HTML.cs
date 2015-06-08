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
        public const string _Template_StdOutputLN = "<span class=\"console-line\">{0}<br/></span>"; //"<div class=\"console-line\">{0}</div>";
        public const string _Template_StdOutputLN_Class = "<span class=\"console-line {0}\">{1}<br/></span>"; //"<div class=\"console-line {0}\">{1}</div>";
        public const string _Template_StdOutput = "<span class=\"console-line\">{0}</span>"; //"<div class=\"console-line\">{0}</div>";
        public const string _Template_StdOutput_Class = "<span class=\"console-line {0}\">{1}</span>"; //"<div class=\"console-line {0}\">{1}</div>";

        public static string Encode(params string[] s)
        {
            string encoded = HttpUtility.HtmlEncode(String.Concat(s));
            encoded = encoded.Replace(" ", "&nbsp;");
            encoded = encoded.Replace("\r\n", "</br>");
            encoded = encoded.Replace("\n", "</br>");
            return encoded;
        }

        public static string CreateOutputLine(params string[] s)
        {
            return String.Format(_Template_StdOutputLN, String.Concat(s));
        }
        public static string CreateCssClassOutputLine(string cssClass, params string[] s)
        {
            return String.Format(_Template_StdOutputLN_Class, cssClass, String.Concat(s));
        }

        public static string CreateOutput(params string[] s)
        {
            return String.Format(_Template_StdOutput, String.Concat(s));
        }
        public static string CreateCssClassOutput(string cssClass, params string[] s)
        {
            return String.Format(_Template_StdOutput_Class, cssClass, String.Concat(s));
        }
    }

    
}
