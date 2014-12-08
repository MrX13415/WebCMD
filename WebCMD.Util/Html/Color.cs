using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Util.Html
{
    public static class Color
    {
        public const string _Template_Color = "<span class=\"{0}\">{1}</span>";
        
        public static string White(params string[] s)
        { return Get("white", s); }

        public static string Get(string cssClass, params string[] s)
        {
            return String.Format(_Template_Color, cssClass, String.Concat(s));
        }


    }
}
