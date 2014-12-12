using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebCMD.Com
{
    public static class ComLoader
    {
        public const string CommandFileExtention = ".dll";
        public const string DefaultPath = /*@"C:\Servers\Web\cmd";*/ @"C:\Users\MrX13415\Documents\Visual Studio 2013\Projects\WebCMD\WebCMD\bin";
        public static List<string> Path = new List<string>();
        public static Thread WorkerThread { get; private set; }
        public static ProgressInfo _progressInfo;
        private static Object mylock = new Object();
        public static ProgressInfo ProgressInfo
        {
            get { lock (mylock) { return _progressInfo; } }
            private set { }
        }


        static ComLoader(){
            _progressInfo = new ProgressInfo();
            Path.Add(DefaultPath);
        }

        public static void Wait()
        {
            lock (mylock) { Monitor.Wait(mylock); }
        }
        public static void PulseAll()
        {
            lock (mylock) { Monitor.PulseAll(mylock); }            
        }

        public static void LoadAsync()
        {
            try
            {
                if (WorkerThread != null && WorkerThread.IsAlive) return;

                Thread clWorker = new Thread(Load);
                clWorker.Name = String.Concat("ComLoader");
                clWorker.IsBackground = true;
                clWorker.Priority = ThreadPriority.Normal;
                clWorker.Start();

                WorkerThread = clWorker;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Concat("ComLoader: unable to start: " + ex));
            }
        }

        public static void Load()
        {
            ProgressInfo.Index = 0;
            ProgressInfo.ProgressState = ProgressInfo.State.Running;
            ProgressInfo.ProgressStep = ProgressInfo.Step.LoadFileList;
            PulseAll();

            CommandHandler.Clear();
            new WebCMD.Com.Lib.CMD_ReloadCommands().Register();
            FileInfo[] files = GetFileList();

            ProgressInfo.Count = files.Length; 
            ProgressInfo.ProgressStep = ProgressInfo.Step.LoadLibraries;
            PulseAll();

            foreach (FileInfo f in files)
            {
                try
                {
                    //debug output here
                    Library lib = Library.From(f);
                    lib.Load();
                    ProgressInfo.Library = lib;
                    PulseAll();
                }
                catch (Exception ex)
                {
                    
                }
                ProgressInfo.Index++;
            }
            ProgressInfo.ProgressState = ProgressInfo.State.Stoped;
            PulseAll();
        }

        public static void RegisterAssemblyType(Assembly assembly, Type type)
        {
            ServerCommand command = assembly.CreateInstance(type.FullName) as ServerCommand;
            command.Register();
        }

        public static FileInfo[] GetFileList()
        {
            string[] path = Path.ToArray() as string[];
            List<FileInfo> files = new List<FileInfo>();

            foreach (string p in path)
            {
                DirectoryInfo dir = new DirectoryInfo(p);
                if (!dir.Exists) continue;
                files.AddRange(dir.GetFiles(String.Concat("*", CommandFileExtention), SearchOption.AllDirectories));
            }
            return files.ToArray() as FileInfo[];
        }
    }

    public class ProgressInfo
    {
        public enum State
        {
            Stoped, Running, Initial
        }

        public enum Step
        {
            Initial, LoadFileList, LoadLibraries
        }

        public Library Library { get; internal set; }
        public State ProgressState { get; internal set; }
        public Step ProgressStep { get; internal set; }
        public bool IsAlive { get { return ProgressState == State.Running; } internal set { } }
        public int Index { get; internal set; }
        public int Count { get; internal set; }
        
        public float Percentage
        {
            get { return ((float)Index / (float)Count) * 100f; }
            private set { }
        }

        public ProgressInfo()
        {
            ProgressState = State.Initial;
            ProgressStep = Step.Initial;
            Index = 0;
            Count = 0;
            Library = null;            
        }

        public override string ToString()
        {
            return ProgressStep == Step.LoadFileList ?
                String.Format("{0:000.00} -- {1}", Percentage, "Loading file list ...") :
                String.Format("{0:000.00} -- #{1:000} F: {2} R: {3} - {4} | {5}", Percentage, Index, Library.CommandCount, Library.RegisteredCount, Library.File.Name, Library.File.FullName);
        }
    }


}
