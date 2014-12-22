using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;

namespace WebCMD.Net.IO
{
    public class ResponseBase
    {
        public const string _ResponseMessageTemplate = "_RS:{0}:{1}";  // request-event-name : event-response-data

        public string RequestTypeID { get; set; }
        public string Data { get; set; }

        public ResponseBase(string requestTypeID)
        {
            this.RequestTypeID = requestTypeID;
            this.Data = "";
        }

        public void SetData(params string[] data)
        {
            Data = String.Concat(data);
        }

        public void AddData(params string[] data)
        {
            Data += String.Concat(data);   
        }

        public void AddDataTop(params string[] data)
        {
            Data = String.Concat(data) + Data;
        }

        public virtual string GetResponseMessage{
            get { return CreateResponseMessage(Data); }
        }
        
        public static string CreateResponseMessage(string requestTypeID, params string[] data)
        {
            return String.Format(_ResponseMessageTemplate, requestTypeID, String.Concat(data));
        }

    }
}