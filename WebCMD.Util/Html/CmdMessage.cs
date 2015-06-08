using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Util.Html
{
    public static class CmdMessage
    {
        public const string _CSS_CLASS_CMD_MESSAGE = "console-message";
        public const string _CSS_CLASS_MSG_TYPE_SERVER = "msg-server";
        public const string _CSS_CLASS_MSG_TYPE_CLIENT = "msg-client";
        public const string _CSS_CLASS_MSG_TYPE_ERROR = "msg-error";
        public const string _CSS_CLASS_MSG_TYPE_INFO = "msg-info";
        public const string _CSS_CLASS_MSG_TYPE_SUCCESS = "msg-success";
        public const string _CSS_CLASS_MSG_TYPE_WARN = "msg-warn";

        public enum Type
        {
            Server, Client, Error, Info, Success, Warning
        }
        public enum IconType
        {
            Success, Error, Warning, Info, Question, None
        }

        public const string _Template_MessageLine = "<div>{0} {1}</div>";


        public static string Get(string icon, Type msgtype, params string[] html)
        {
            return HTML.CreateCssClassOutput(String.Format("{0} {1}", _CSS_CLASS_CMD_MESSAGE, GetTypeClass(msgtype)),
                String.Format(_Template_MessageLine, HTML.Encode(icon.PadRight(4)), String.Concat(html)));
        }

        public static string Get(IconType icontype, Type msgtype, params string[] html)
        {
            return Get(GetIcon(icontype), msgtype, html);
        }

        public static string Get(Type msgtype, params string[] html)
        {
            IconType it;
            switch (msgtype)
            {
                case Type.Server:
                    it = IconType.Warning;
                    break;
                case Type.Client:
                    it = IconType.Warning;
                    break;
                case Type.Error:
                    it = IconType.Error;
                    break;
                case Type.Info:
                    it = IconType.Info;
                    break;
                case Type.Success:
                    it = IconType.Success;
                    break;
                case Type.Warning:
                    it = IconType.Warning;
                    break;
                default:
                    it = IconType.Info;
                    break;
            }

            return Get(GetIcon(it), msgtype, html);
        }

        public static string GetServerMessage(params string[] html)
        {
            return Get(Type.Server, html);
        }
        public static string GetErrorMessage(params string[] html)
        {
            return Get(Type.Error, html);
        }
        public static string GetWarningMessage(params string[] html)
        {
            return Get(Type.Warning, html);
        }
        public static string GetInfoMessage(params string[] html)
        {
            return Get(Type.Info, html);
        }
        public static string GetClientMessage(params string[] html)
        {
            return Get(Type.Client, html);
        }
        public static string GetSuccessMessage(params string[] html)
        {
            return Get(Type.Success, html);
        }

        public static string GetTypeClass(Type t)
        {
            switch (t)
            {
                case Type.Server:
                    return _CSS_CLASS_MSG_TYPE_SERVER;
                case Type.Client:
                    return _CSS_CLASS_MSG_TYPE_CLIENT;
                case Type.Error:
                    return _CSS_CLASS_MSG_TYPE_ERROR;
                case Type.Info:
                    return _CSS_CLASS_MSG_TYPE_INFO;
                case Type.Success:
                    return _CSS_CLASS_MSG_TYPE_SUCCESS;
                case Type.Warning:
                    return _CSS_CLASS_MSG_TYPE_WARN;
                default:
                    return _CSS_CLASS_MSG_TYPE_CLIENT;
            }
        }

        public static string GetIcon(IconType it)
        {
            switch (it)
            {
                case IconType.Success:
                    return TextIcons.Basic_Success;
                case IconType.Error:
                    return TextIcons.Basic_Error;
                case IconType.Warning:
                    return TextIcons.Basic_Warning;
                case IconType.Info:
                    return TextIcons.Basic_Info;
                case IconType.Question:
                    return TextIcons.Basic_Question;
                default:
                    return "";
            }
        }
    }
}
