using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.SessionState;
using System.Web;

namespace WebCMD.Util
{
    public static class Ref
    {
        public static HtmlGenericControl ConsoleHeader;
        public static HtmlGenericControl ConsoleDebug;
        public static HtmlGenericControl ConsoleOutput;
        
        public static object GetSessionClassObject(/*string id,*/ Type t)
        {
            try {
                return HttpContext.Current.Session[t.FullName]; }
            catch { return null; }
            //try { return sessionList[id][t.FullName]; }
            //catch { return null; }
        }

        public static void SetSessionClassObject(/*string id,*/ Type t, object obj)
        {
            try { HttpContext.Current.Session[t.FullName] = obj; }
            catch { } 

            //Dictionary<string, object> sd = sessionList[id];

            //if (sd == null){
            //    sd = new  Dictionary<string, object>();
            //    sessionList.Add(id, sd);
            //}
            
            //if(sd[t.FullName] == null){
            //    sd.Add(t.FullName, obj);
            //}else{
            //    sd[t.FullName] = obj;
            //}
            
        }


    }
}
