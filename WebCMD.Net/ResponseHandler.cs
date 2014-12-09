using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using WebCMD.Net.Event;
using WebCMD.Util;
using System.Diagnostics;

namespace WebCMD.Net
{
    public class ResponseHandler
    {
        private Queue responseQueue = Queue.Synchronized(new Queue());

        public string ConnectionID { get; set; }
        public Action<ResponseEvent> Respond { get; set; }
        public Thread ResponseWorker { get; private set; }
        public Int64 ResponseCount { get; set; }

        public ResponseHandler(string connectionID, Action<ResponseEvent> function)
        {
            this.Respond = function;
            this.ConnectionID = connectionID;
        }

        public void DoRespond()
        {
            try
            {
                if (ResponseWorker != null && ResponseWorker.IsAlive) return;

                ResponseWorker = new Thread(this.RSWorker);
                ResponseWorker.Name = String.Concat("ResponseWorker_", ConnectionID);
                ResponseWorker.IsBackground = true;
                ResponseWorker.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Concat("ResponseWorker_", ConnectionID, " unable to start: " + ex));
            }
        }

        private void RSWorker()
        {
            Debug.WriteLine(String.Concat(ResponseWorker.Name, " started"));
            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                do
                {
                    while (GetQueueSize > 0)
                    {
                        Respond(new ResponseEvent(ConnectionID, NextMessageBlock));
                        ResponseCount++;
                    }

                    Thread.Sleep(20);
                } while (watch.ElapsedMilliseconds <= 5000 || GetQueueSize > 0); //stay active for at least 5 second

                Debug.WriteLine(String.Concat(ResponseWorker.Name, " completed after ", watch.ElapsedMilliseconds, "ms"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(String.Concat(ResponseWorker.Name, " stoped after ", watch.ElapsedMilliseconds, "ms with error: " + ex));
            }
            finally
            {
                watch.Stop();
            }
        }

        public void Send(params string[] html)
        {
            lock (this) { responseQueue.Enqueue(CreateOutputResponse(html)); }
            DoRespond();
        }

        public void Send(ServerResponse sp)
        {
            lock (this) { responseQueue.Enqueue(sp); }
            DoRespond();
        }

        public int GetQueueSize
        {
            get
            {
                lock (this) { return responseQueue.Count; }
            }
        }

        public ServerResponse Next
        {
            get
            {
                lock (this)
                {
                    try { return (ServerResponse)responseQueue.Dequeue(); }
                    catch { return null; } 
                }
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
                if (GetQueueSize == 0) return "";

                ServerResponse block = null;
                ServerResponse response = null;

                do
                {
                    response = Next;
                    if (response == null) break;
                    if (block == null) block = new ServerResponse(response.HtmlControl);
                    block.SetData(response.Mode, response.Data);
                }
                while (block.HtmlControl.ClientID.Equals(response.HtmlControl.ClientID));

                return block.GetResponseMessage;
            }
        }

        public static ServerResponse NewHeaderResponse { get { return new ServerResponse(Ref.ConsoleHeader); } }
        public static ServerResponse NewDebugResponse { get { return new ServerResponse(Ref.ConsoleDebug); } }
        public static ServerResponse NewOutputResponse { get { return new ServerResponse(Ref.ConsoleOutput); } }

        public static ServerResponse CreateHeaderResponse(params string[] html)
        {
            ServerResponse sr = new ServerResponse(Ref.ConsoleHeader);
            sr.AddData(html);
            return sr;
        }

        public static ServerResponse CreateDebugResponse(params string[] html)
        {
            ServerResponse sr = new ServerResponse(Ref.ConsoleDebug);
            sr.AddData(html);
            return sr;
        }

        public static ServerResponse CreateOutputResponse(params string[] html)
        {
            ServerResponse sr = new ServerResponse(Ref.ConsoleOutput);
            sr.AddData(html);
            return sr;
        }

    }
}
