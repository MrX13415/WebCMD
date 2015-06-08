using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net.IO;
using WebCMD.Net;
using System.Threading;
using WebCMD.Core;
using System.Net;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Helpers;
using WebCMD.Util;
using WebCMD.Com;
using WebCMD.Util.Html;

namespace WebCMD.Lib.System
{
    public class CMD_Get_PlaylistEntrys : Command
    {

        public const string YouTube_GDATA_PlaylistQuery_Template = "http://gdata.youtube.com/feeds/api/playlists/{0}?start-index={1}&alt=json";

        public CMD_Get_PlaylistEntrys()
        {
            Label = "Get-PlaylistEntrys";
            SetAliase("plentrys", "ytplentrys", "ytple");
        }

        protected override bool _Execute(CommandRequest e)
        {
            bool r = false;
            r = GetLinks(e);

            if (!r)
            {
                e.Source.Response.Send(CmdMessage.GetErrorMessage("Bad arguments"));
                e.Source.Response.Send(HTML.Encode(String.Concat("Returns the Video-Links of a YouTube Playlist\n\n", Label, " <playlist id> <start index>")));
            }
               
            return r;
        }

        private bool GetLinks(CommandRequest e)
        {
            if (e.ArgumentList.Length <= 0) return false;

            string id = e.ArgumentList[0];

            if (id.Equals(":3")) id = "PLoJGguUgeh_5HM_kQhT014krKrVzCOXri";

            int index = 0;

            try { index  = Int32.Parse(e.ArgumentList[1]); }
            catch { return false; }

            string json = getJSON(String.Format(YouTube_GDATA_PlaylistQuery_Template, id, index));

            JavaScriptSerializer ser = new JavaScriptSerializer();
            string JSONString = ser.Serialize(json); //JSON encoded

            dynamic data = Json.Decode(json);
            //JSON/feed/entry/x/link/0/href

            var x = data.feed.entry[0].link[0].href;

            string cout = "";
            int count = 0;

            foreach (dynamic item in data.feed.entry)
            {
                string link = item.link[0].href;
                cout += "<div>" + link + "</div>";
                count++;
            }

            cout = "<span class=\"msg-error\">YOUTUBE | PLAYLIST ID: " + id + " | COUNT: " + count + " | START INDEX: " + index + "</span></br>" +
                   "<div class=\"yellow\" style=\"margin-left: 20px\">" + cout;
            cout += "</div>";

            e.Source.Response.Send(ResponseHandler.CreateOutputResponse(cout));

            return true;
        }

        public string getJSON(string urlAddress)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                return data;
            }

            return "";
        }
    }
}