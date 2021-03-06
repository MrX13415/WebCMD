﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebCMD.Com
{
    public static class ComLoader
    {
        public const string CommandFileExtention = ".dll";
        public static string BasePath = HttpContext.Current.Server.MapPath("~");
        public static List<string> Path = new List<string>();
        public static Thread WorkerThread { get; private set; }
        public static ProgressInfo _progressInfo;
        private static Object mylock = new Object();
        public static ProgressInfo ProgressInfo
        {
            get { lock (mylock) { return _progressInfo; } }
            private set { }
        }


        static ComLoader()
        {
            _progressInfo = new ProgressInfo();
            Path.Add(HttpContext.Current.Server.MapPath("bin"));
            Path.Add(HttpContext.Current.Server.MapPath("sys/com"));
        }

        public static void Wait()
        {
            lock (mylock) { Monitor.Wait(mylock); }
        }
        public static void PulseAll()
        {
            lock (mylock) { Monitor.PulseAll(mylock); }
        }

        public static void Load()
        {
            try
            {
                if (WorkerThread != null && WorkerThread.IsAlive) return;

                Thread clWorker = new Thread(_Load);
                clWorker.Name = String.Concat("ComLoader-Worker");
                clWorker.IsBackground = true;
                clWorker.Priority = ThreadPriority.Normal;
                clWorker.Start();

                WorkerThread = clWorker;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(String.Concat(" (x)  Error: Unable to start the ComLoader-Worker! Could not load any command librarys. Error: ", ex));
            }
        }

        private static void _Load()
        {
            ProgressInfo.Index = 0;
            ProgressInfo.ProgressState = ProgressInfo.State.Running;
            ProgressInfo.ProgressStep = ProgressInfo.Step.LoadFileList;
            PulseAll();

            ComHandler.Clear();

            FileInfo[] files = GetFileList();

            ProgressInfo.Count = files.Length;
            ProgressInfo.ProgressStep = ProgressInfo.Step.LoadLibraries;
            PulseAll();

            foreach (FileInfo f in files)
            {
                try
                {
                    //debug output here
                    ComLibrary lib = ComLibrary.Instance(f);
                    lib.Load();
                    ProgressInfo.Library = lib;
                    PulseAll();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(" (x)  Error while loading library '" + f.Name + "': " + ex);
                }
                ProgressInfo.Index++;
            }
            ProgressInfo.ProgressState = ProgressInfo.State.Stoped;
            PulseAll();
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

        public ComLibrary Library { get; internal set; }
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
                String.Format("{0:000.00} -- #{1:000} F: {2:00} R: {3:00} - {4, -40} | {5}", Percentage, Index, Library.CommandCount, Library.RegisteredCount, Library.File.Name, Library.File.FullName.Replace(ComLoader.BasePath, "~" + Path.DirectorySeparatorChar));
        }
    }


}
