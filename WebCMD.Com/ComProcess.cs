using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCMD.Net.IO;

namespace WebCMD.Com
{
    public class ComProcess
    {
        public Command Command { get; private set; }
        public CommandRequest Request { get; private set; }
        public Thread Process { get; private set; }

        internal ComProcess(Command com, CommandRequest request)
        {
            this.Command = com;
            this.Request = request;
        }

        internal void Start(ParameterizedThreadStart target)
        {
            ParameterizedThreadStart threadStart = new ParameterizedThreadStart(target);
            Process = new Thread(threadStart);
            Process.Name = "WebCMD-COMMAND_" + this.GetType().Name;
            Process.Start(Request);
        }
    }
}
