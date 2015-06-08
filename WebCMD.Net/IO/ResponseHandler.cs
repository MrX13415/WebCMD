using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using WebCMD.Net.IO;
using WebCMD.Util;
using System.Diagnostics;
using WebCMD.Net.IO.Event;
using WebCMD.Util.Html;

namespace WebCMD.Net.IO
{
    public class ResponseHandler
    {
        private Queue buffer = Queue.Synchronized(new Queue());
        private Queue lostQueue = Queue.Synchronized(new Queue());

        public string ConnectionID { get; set; }
        public Action<ResponseEventArgs> Respond { get; set; }
        public Thread ResponseWorker { get; private set; }
        public Int64 ResponseCount { get; set; }
        public int WorkerTimout { get; set; }
        public int WorkerMinActiveTime { get; set; }

        public ResponseHandler(string connectionID, Action<ResponseEventArgs> function)
        {
            this.Respond = function;
            this.ConnectionID = connectionID;
            this.WorkerTimout = 40;
            this.WorkerMinActiveTime = 2500;
        }

        public void DoRespond()
        {
            try
            {
                if (ResponseWorker != null && ResponseWorker.IsAlive) return;

                Thread rsWorker = new Thread(this.RSWorker);
                rsWorker.Name = String.Concat("ResponseWorker_", ConnectionID);
                rsWorker.IsBackground = true;
                rsWorker.Priority = ThreadPriority.BelowNormal;
                rsWorker.Start();

                ResponseWorker = rsWorker;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Concat(" (!)  ResponseWorker_", ConnectionID, " unable to start: " + ex));
            }
        }

        private void RSWorker()
        {
            Debug.WriteLine(String.Concat(" (!)  ", Thread.CurrentThread.Name, " started"));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                do
                {
                    string data = NextMessageBlock;
                    if (!String.IsNullOrEmpty(data)){   
                        Respond(new ResponseEventArgs(data, ConnectionID));
                        ResponseCount++;
                    }

                    Thread.Sleep(WorkerTimout);
                } while (watch.ElapsedMilliseconds <= WorkerMinActiveTime || GetQueueSize > 0); //stay active for at least 2.5 second (default for WorkerMinActiveTime)

                Debug.WriteLine(String.Concat(" (!)  ", Thread.CurrentThread.Name   , " completed after ", watch.ElapsedMilliseconds, "ms"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Concat(" /!\\  ", Thread.CurrentThread.Name   , " stoped after ", watch.ElapsedMilliseconds, "ms with error: " + ex));
            }
            finally
            {
                watch.Stop();
            }
        }

        public void Send(params string[] html)
        {
            //TODO: improve this with regex ...
            //Like: he = new HTMLElement("<div class=..."); ... string[] classes = he.ClassList; ... he.equals(HTML._Template_HE...)
            if (String.Concat(html).ToLower().Trim().StartsWith("<span class=\"console-line"))
                SendRaw(html);
            else
                SendRaw(HTML.CreateOutputLine(html));
        }
        public void SendClass(string cssClass, params string[] html)
        {
            SendRaw(HTML.CreateCssClassOutputLine(cssClass, html));
        }

        public void SendRaw(params string[] html)
        {
            lock (this) { buffer.Enqueue(CreateOutputResponse(html)); }
            DoRespond();
        }

        public void Send(ServerResponse sp)
        {
            lock (this) { buffer.Enqueue(sp); }
            DoRespond();
        }

        public int GetQueueSize
        {
            get
            {
                lock (this) { return buffer.Count; }
            }
        }

        public ServerResponse Next
        {
            get
            {
                lock (this)
                { return buffer.Count > 0 ? buffer.Dequeue() as ServerResponse : null; }
            }
        }
        public ServerResponse Peek
        {
            get
            {
                lock (this)
                { return buffer.Count > 0 ? buffer.Peek() as ServerResponse : null; }
            }
        }

        /// <summary>
        /// Get a response message
        /// </summary>
        /// <returns>A response message</returns>
        public string NextMessage
        {
            get
            {
                lock (this)
                {
                    ServerResponse sr = Next;
                    return sr != null ? sr.GetResponseMessage : null;
                }
            }
        }

        /// <summary>
        /// Get multiple response messages for one html-control at once.
        /// </summary>
        /// <returns>A response message</returns>
        public string NextMessageBlock
        {
            get{
                ServerResponse response = Next;
                if (response == null) return "";

                ServerResponse block = ServerResponse.Copy(response);

                int index = 0;          

                do
                {
                    block.SetData(response.Mode, response.Data);
                                       
                    Thread.Yield();
                    index++;

                    if (index > 10) break;

                    response = Next; //get next ...
                    if (response == null) break; //HTMLcontrol null?
                }
                while (block.Equals(response));

                return block.GetResponseMessage;
            }
        }

        public static ServerResponse NewHeaderResponse { get { return new ServerResponse(Ref.ConsoleHeaderID); } }
        public static ServerResponse NewDebugResponse { get { return new ServerResponse(Ref.ConsoleDebugID); } }
        public static ServerResponse NewOutputResponse { get { return new ServerResponse(Ref.ConsoleOutputID); } }

        public static ServerResponse CreateHeaderResponse(params string[] html)
        {
            return CreateResponse(Ref.ConsoleHeaderID, html);
        }

        public static ServerResponse CreateDebugResponse(params string[] html)
        {
            return CreateResponse(Ref.ConsoleDebugID, html);
        }

        public static ServerResponse CreateOutputResponse(params string[] html)
        {
            return CreateResponse(Ref.ConsoleOutputID, html);
        }

        public static ServerResponse CreateFooterResponse(params string[] html)
        {
            return CreateResponse(Ref.ConsoleFooterID, html);
        }

        public static ServerResponse CreateResponse(string controlID, params string[] html)
        {
            ServerResponse sr = new ServerResponse(controlID);
            sr.AddData(html);
            return sr;
        }

    }
}
