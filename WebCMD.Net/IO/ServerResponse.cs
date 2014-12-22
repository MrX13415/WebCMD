using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;

namespace WebCMD.Net.IO
{
    public class ServerResponse : ResponseBase
    {
        public enum PropertyMode
        {
            AddEnd, Set, AddTop, FunctionCall
        }

        private const string _DefaultProperty = "innerHTML";
        public new const string _ResponseMessageTemplate = "_RS:{0}:{1}:{2}:{3}:{4}";  //  request-event-name : target-control-id : property-mode : target-JS-property : data

        public string HtmlControlID { get; set; }

        public string[] FunctionCallArguments
        {
            get { return Data.Split(' '); }
            set { Data = CreateFunctionCallData(value); }
        }

        public string Property { get; set; }
        public PropertyMode Mode { get; set; }


        public ServerResponse(string requestTypeID, string controlID) : base(requestTypeID)
        {
            this.HtmlControlID = controlID;
            Mode = PropertyMode.AddEnd;
            Property = _DefaultProperty;
        }

        public ServerResponse(string controlID) : this(GetDefaultRequestID, controlID)
        { }

        public static string GetDefaultRequestID { get { return ServerRequest.GetRequestTypeID(typeof(CommandRequest)); } }

        public void SetData(PropertyMode mode, params string[] data)
        {
            switch (mode)
            {
                case PropertyMode.AddEnd:
                    AddData(data);
                    break;
                case PropertyMode.Set:
                    SetData(data);
                    break;
                case PropertyMode.AddTop:
                    AddDataTop(data);
                    break;
                case PropertyMode.FunctionCall:
                    SetData(data);
                    break;
            }
        }

        public override string GetResponseMessage{
            get { return CreateResponseMessage(RequestTypeID, HtmlControlID, Property, Mode, Data); }
        }
        
        /// <summary>
        /// Creates a string representing all ars of a function call
        /// </summary>
        /// <param name="data">A array containing all args e.g: {true, 3.5f, "a string"}</param>
        /// <returns>A string containg all args seperated by ';' e.g: "true;3.5f;"a string""</returns>
        public static string CreateFunctionCallData(params string[] data)
        {
            string rdata = "";
            foreach (string item in data)
            {
                if (item.Trim().Contains(' ') && !(item.Trim().StartsWith("\"") && item.Trim().EndsWith("\"")))
                    rdata += "\"" + item + "\";";
                else
                    rdata += item + ";";
            }
            return rdata.Substring(rdata.Length - 1);
        }

        public static string CreateResponseMessage(string requesttypeID, string controlID, string property, PropertyMode mode, params string[] data)
        {
            string modestr = "0";

            switch (mode)
            {
                case PropertyMode.AddEnd:
                    modestr = "1";
                    break;
                case PropertyMode.Set:
                    modestr = "2";
                    break;
                case PropertyMode.AddTop:
                    modestr = "3";
                    break;
                default:
                    modestr = "0";
                    break;
            }

            return String.Format(_ResponseMessageTemplate, requesttypeID, controlID, modestr, property, String.Concat(data));
        }

    }
}