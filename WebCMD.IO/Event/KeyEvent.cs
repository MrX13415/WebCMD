using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebCMD.Net.Event
{
    public class KeyEvent : RequestEvent
    {
        public enum KeyEventType
        {
            KeyDown, KeyPress, KeyUp
        }

        public KeyEventType EventType { get; set; }
        public bool CtrlKeyDown { get; set; }
        public bool AltKeyDown { get; set; }
        public bool ShiftKeyDown { get; set; }
        public bool MetaKeyDown { get; set; }
        public int KeyCode { get; set; }

        private KeyEvent(Client client, string targetid) : base(client, targetid, typeof(KeyEvent))
        {
        }

        public KeyEvent(Client client, string targetid, KeyEventType type, int keycode, bool ctrl, bool alt, bool shift, bool meta) : this(client, targetid)
        {
            EventType = type;
            KeyCode = keycode;
            CtrlKeyDown = ctrl;
            AltKeyDown = alt;
            ShiftKeyDown = shift;
            MetaKeyDown = meta;
        }
        public KeyEvent(Client client, string targetid, KeyEventType type, int keycode) : this(client, targetid, type, keycode, false, false, false, false) { }

        /// <summary>
        /// jsEventArgs = "_KeyEvent <eventtype> <ctrlkey> <altkey> <shiftkey> <metakey> <keycode>"
        /// Spaced by tabs
        /// see ToString();
        /// </summary>
        /// <param name="jsEventArgs"></param>
        public KeyEvent(Client client, string targetid, string eventArgs) : this(client, targetid)
        {
            process(eventArgs);
        }

        private void process(string eventArgs)
        {
            try
            {
                String[] args = eventArgs.Split(':');

                if (args.Count() < 6) throw new InvalidParameterException();

                //type
                switch (args[0])
                {
                    case "keydown":
                        EventType = KeyEventType.KeyDown;
                        break;
                    case "keyup":
                        EventType = KeyEventType.KeyUp;
                        break;
                    case "keypress":
                        EventType = KeyEventType.KeyPress;
                        break;
                    default:
                        break;
                }

                CtrlKeyDown = Boolean.Parse(args[1]);
                AltKeyDown = Boolean.Parse(args[2]);
                ShiftKeyDown = Boolean.Parse(args[3]);
                MetaKeyDown = Boolean.Parse(args[4]);

                KeyCode = Int32.Parse(args[5]);
            }
            catch (Exception ex)
            {
                throw (InvalidParameterException) ex;
            }
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", RequestHandler.EventIdentString, TargetElementID, EventTypeID, EventType, CtrlKeyDown, AltKeyDown, ShiftKeyDown, MetaKeyDown, KeyCode);
        }
    }
}