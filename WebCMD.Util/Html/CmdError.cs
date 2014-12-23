using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Util.Html
{
    public class CmdError
    {
        public static string _HTMLTemplate_ErrorMessage = "<span>Server-Error: {0}: <span class=\"red-light\">{1}</span></span><br><div class=\"red-light\" style=\"margin-left: 60px;\">{2}</div>";

        public static string Get(Exception ex)
        {
            string errstr = String.Format(_HTMLTemplate_ErrorMessage, HTML.Encode(ex.GetType().ToString()), HTML.Encode(ex.Message), HTML.Encode(ex.StackTrace));
            return CmdMessage.GetErrorMessage(errstr);
        }

    }
}
