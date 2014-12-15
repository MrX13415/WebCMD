using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Util.Html
{
    public class CmdError
    {
        public static string _HTMLTemplate_ServerMessage = "<div class=\"console-line msg-server\">\n<span>{0}</span><br><div style=\"margin-left: 60px;\">{1}</div>\n</div>";

        public static string Get(Exception ex)
        {
            string errstr = String.Format(_HTMLTemplate_ServerMessage, String.Format("Server-Error: {0}: {2}", ex.GetType(), ex.Message), ex.StackTrace);
            return CmdMessage.GetErrorMessage(HTML.Encode(errstr));
        }
    }
}
