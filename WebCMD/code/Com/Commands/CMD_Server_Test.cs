using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCMD.Net.Event;
using WebCMD.Net;
using System.Threading;
using WebCMD.Core;
using WebCMD.Util;
using WebCMD.Com;

namespace WebCMD.Com.Commands
{
    public class CMD_Server_Test : ServerCommand
    {
        public CMD_Server_Test()
        {
            Label = "Server-Test";
            SetAliase("srvertest", "srvtest", "srvt");
        }

        protected override bool Process(CommandEvent e)
        {
            bool r = false;

            try
            {
                switch (e.ArgumentList[0].ToLower())
                {
                    case "response":
                        r = DoResponseTest(e);
                        break;
                    case "error":
                        r = DoErrorTest(e);
                        break;
                    case "savelog":
                        r = SaveDebugRequestLog(e);
                        break;
                    default:
                        e.EventSource.Response.Send(ResponseHandler.CreateOutputResponse("<div class=\"console-line msg-error\">Bad arguments</div>" +
                            "<div>Server-Test [response &lt;interval ms&gt; &lt;time ms&gt;|error|savelog]</div>"));
                        break;
                }
            }
            catch (Exception ex)
            {
                ServerError.Add(ex);
            }
            
            return r;
        }

        private bool SaveDebugRequestLog(CommandEvent e)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Servers\Web\WEBCMD-LOG.txt");

            foreach (string o in Log.Get)
            {
                file.WriteLine(o);
            }

            file.Close();

            e.EventSource.Response.Send(ResponseHandler.CreateOutputResponse("<div class=\"console-line msg-success\">Log successfully saved</div>"));


            return true;
        }

        private bool DoErrorTest(CommandEvent e)
        {
            for (int i = 0; i < 5; i++)
            {
                ServerError.Add(new Exception("Some internal test errors occured! Don't panic! This is just a test :)"));    
            }
                       
            return true;
        }

        private bool DoResponseTest(CommandEvent e)
        {
            string[] args = e.ArgumentList;
            int index = 0;
            long time = args.Length >= 3 ? Int32.Parse(args[2]) : 0; //3000
            int ms = args.Length >= 2 ? Int32.Parse(args[1]) : 0; //50

            if (time <= 0) time = 3000;
            if (ms <= 0) ms = 20;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            e.EventSource.Response.Send(ResponseHandler.CreateOutputResponse("<div class=\"console-line msg-success\">Response Test | Started | T: " + time + " | Interval: " + ms + "</div>"));

            do
            {
                ServerResponse response = ResponseHandler.NewOutputResponse;
                response.AddData(String.Format("<div class=\"console-line msg-warn\">Response Test | Response: #{0, -4} | T: {1, -6} | Buffer: {2}</div>", String.Format("{0, -6}", index.ToString("D3")), stopWatch.ElapsedMilliseconds.ToString("D5"), e.EventSource.Response.GetQueueSize));
                e.EventSource.Response.Send(response);
                index++;

                Thread.Sleep(ms);

            } while (stopWatch.ElapsedMilliseconds <= time);

            e.EventSource.Response.Send(ResponseHandler.CreateOutputResponse("<div class=\"console-line msg-success\">Response Test | Successfully completed | Count: " + index + " | Buffer: " + e.EventSource.Response.GetQueueSize + "</div>"));

            stopWatch.Stop();

            return true;
        }

    }
}