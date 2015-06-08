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

        //mode: 0 0
        //      | |
        //      | \-- property-mode
        //      \-- use Animation
        public new const string _ResponseMessageTemplate = "_RS:{0}:{1}:{2}:{3}:{4}";  //  response-event-name : target-control-id : mode : target-JS-property : data

        public string HtmlControlID { get; set; }

        public string[] FunctionCallArguments
        {
            get { return Data.Split(' '); }
            set { Data = CreateFunctionCallData(value); }
        }

        public string Property { get; set; }
        public PropertyMode Mode { get; set; }
        public bool UseAnimation { get; set; }

        public ServerResponse(string requestTypeID, string controlID) : base(requestTypeID)
        {
            this.HtmlControlID = controlID;
            Mode = PropertyMode.AddEnd;
            Property = _DefaultProperty;
            UseAnimation = true;
        }

        public ServerResponse(string controlID) : this(GetDefaultRequestID, controlID)
        { }

        public static ServerResponse Copy(ServerResponse rs)
        {
            ServerResponse copy = new ServerResponse(rs.RequestTypeID, rs.HtmlControlID);
            copy.Mode = rs.Mode;
            copy.Property = rs.Property;
            copy.UseAnimation = rs.UseAnimation;
            return copy;
        }

        public bool Equals(ServerResponse rs)
        {
            if (this.RequestTypeID == rs.RequestTypeID &&
                this.HtmlControlID == rs.HtmlControlID &&
                this.Mode == rs.Mode &&
                this.Property == rs.Property &&
                this.UseAnimation == rs.UseAnimation) return true;

            return false;
        }

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
            get { return CreateResponseMessage(RequestTypeID, HtmlControlID, Property, Mode, UseAnimation,  Data); }
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
            return CreateResponseMessage(requesttypeID, controlID, property, mode, true, String.Concat(data));
        }

        public static string CreateResponseMessage(string requesttypeID, string controlID, string property, PropertyMode mode, bool useAnimation, params string[] data)
        {
            int modeN = 0;

            switch (mode)
            {
                case PropertyMode.AddEnd:
                    modeN = 1;
                    break;
                case PropertyMode.Set:
                    modeN = 2;
                    break;
                case PropertyMode.AddTop:
                    modeN = 3;
                    break;
                default:
                    modeN = 0;
                    break;
            }

            if (useAnimation) modeN += 10;

            return String.Format(_ResponseMessageTemplate, requesttypeID, controlID, modeN, property, String.Concat(data));
        }

    }
}