using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using WebCMD.Net;
using WebCMD.Core;
using WebCMD.Net.Event;
using WebCMD.Util;
using WebCMD.Com;
using WebCMD.Util.Html;

namespace WebCMD.Com.Commands
{
    public class CMD_Get_TodoList : ServerCommand
    {
        public string bugListDBPath = @"C:\Servers\Web\WEBCMD-TODO-LIST.txt";

        public CMD_Get_TodoList()
        {
            Label = "Get-TodoList";
            SetAliase("Get-BugList", "bugs", "bug", "todo");
        }

        protected override bool Process(CommandEvent e)
        {
            string[] args = e.ArgumentList;
            ServerResponse response = ResponseHandler.NewOutputResponse;

            try
            {
                if (args.Length == 1 && args[0].Equals(""))
                {
                    List<BugObject> l = GetBugList();

                    foreach (BugObject o in l)
                    {
                        response.AddData(CreateListEntry(o));
                    }
                }
                else if (args.Length >= 2){
                    //ADD
                    if (args[0].ToLower().Equals("add") || args[0].ToLower().Equals("report") || args[0].ToLower().Equals("-r") || args[0].ToLower().Equals("-a"))
                    {
                        List<BugObject> l = GetBugList();

                        string desc = Regex.Replace(e.ArgumentString.Substring(args[0].Length), "\\s+", " ");
                        BugObject.BugType type = BugObject.BugType.TODO;

                        string b = e.Command.ToLower();
                        if (b.Contains("bug")) type = BugObject.BugType.BUG;

                        BugObject o = new BugObject(type, desc);
                        o.ID = l[l.Count - 1].ID + 1;
                        o.State = BugObject.BugSate.NEW;

                        l.Add(o);
                        SaveBugList(l);
                        response.AddData(HTML.CreateConsoleLineClass("green", "Added: "));
                        response.AddData(CreateListEntry(o));
                    }
                    //REMOVE
                    else if (args[0].ToLower().Equals("remove") || args[0].ToLower().Equals("-rm"))
                    {
                        List<BugObject> l = GetBugList();
                        int n = Int32.Parse(args[1]);
                        bool match = false;

                        foreach (BugObject o in l)
                        {
                            if (o.ID == n)
                            {
                                match = true;
                                response.AddData(HTML.CreateConsoleLineClass("red", "Removed: "));
                                response.AddData(CreateListEntry(o));
                                l.Remove(o);
                                break;
                            }
                        }

                        if (match) SaveBugList(l);
                        else response.AddData(CmdMessage.Get(CmdMessage.Type.Error, HTML.Encode("Error: ID Not found!")));
                    }
                    else if (args[0].ToLower().Equals("fixed") || args[0].ToLower().Equals("-f") || args[0].ToLower().Equals("done") || args[0].ToLower().Equals("-d"))
                    {
                        BugObject o = GetObject(Int32.Parse(args[1]));
                        if (o != null)
                        {
                            o.State = BugObject.BugSate.DONE;
                            if (o.Type == BugObject.BugType.BUG) o.State = BugObject.BugSate.FIXED;

                            response.AddData(HTML.CreateConsoleLineClass("blue-light", "Changed: "));
                            response.AddData(CreateListEntry(UpdateObject(o)));
                        }
                        else response.AddData(CmdMessage.Get(CmdMessage.Type.Error, HTML.Encode("Error: ID Not found!")));
                    }
                    else if (args.Length >= 2 && args[0].ToLower().Equals("invalid") || args[0].ToLower().Equals("-i"))
                    {
                        BugObject o = GetObject(Int32.Parse(args[1]));
                        if (o != null)
                        {
                            o.State = BugObject.BugSate.INVALID;
                            response.AddData(HTML.CreateConsoleLineClass("blue-light", "Changed: "));
                            response.AddData(CreateListEntry(UpdateObject(o)));
                        }
                        else response.AddData(CmdMessage.Get(CmdMessage.Type.Error, HTML.Encode("Error: ID Not found!")));
                    }
                    else if (args.Length >= 2 && args[0].ToLower().Equals("new") || args[0].ToLower().Equals("-n"))
                    {
                        BugObject o = GetObject(Int32.Parse(args[1]));
                        if (o != null)
                        {
                            o.State = BugObject.BugSate.NEW;
                            response.AddData(HTML.CreateConsoleLineClass("blue-light", "Changed: "));
                            response.AddData(CreateListEntry(UpdateObject(o)));
                        }
                        else response.AddData(CmdMessage.Get(CmdMessage.Type.Error, HTML.Encode("Error: ID Not found!")));
                    }
                    else if (args.Length >= 2 && args[0].ToLower().Equals("work") || args[0].ToLower().Equals("-w"))
                    {
                        BugObject o = GetObject(Int32.Parse(args[1]));
                        if (o != null)
                        {
                            o.State = BugObject.BugSate.IN_PROGRESS;
                            response.AddData(HTML.CreateConsoleLineClass("blue-light", "Changed: "));
                            response.AddData(CreateListEntry(UpdateObject(o)));
                        }
                        else response.AddData(CmdMessage.Get(CmdMessage.Type.Error, HTML.Encode("Error: ID Not found!")));
                    }
                    else if (args.Length >= 3 && args[0].ToLower().Equals("prio") || args[0].ToLower().Equals("priority") || args[0].ToLower().Equals("-p"))
                    {
                        BugObject o = GetObject(Int32.Parse(args[1]));
                        if (o != null)
                        {
                            o.Priority = BugObject.GetPrio(BugObject.GetPrio(args[2].ToLower().Trim()));

                            response.AddData(HTML.CreateConsoleLineClass("blue-light", "Changed: "));
                            response.AddData(CreateListEntry(UpdateObject(o)));
                        }
                        else response.AddData(CmdMessage.Get(CmdMessage.Type.Error, HTML.Encode("Error: ID Not found!")));
                    }
                }
                else
                {
                    response.AddData(HTML.CreateConsoleLine(Color.Get("orange", "TODO/BUG DB"), " -- Version 2.0 -- Team Icelane (c) 2014"));

                }
            }
            catch (Exception ex)
            {
                ServerError.Add(ex);
            }

            e.EventSource.Response.Send(response);

            return true;
        }

        public string CreateListEntry(BugObject o)
        {
            string tmpla = "<div>" +
                           "<span class=\"yellow\" style=\"display: table-cell;\">{0}</span>" +
                           "<span class=\"{1}\" style=\"display: table-cell;\">{2}</span>" +
                           "<span class=\"{3}\" style=\"display: table-cell;\">{4}</span>" +
                           "<span style=\"display: table-cell;\"><span class=\"red\">{5}</span>{6}</span>" +
                           "<div>";
            
            string id = HTML.Encode(String.Format("{0, -6}", o.ID.ToString("D4")));
            string state = HTML.Encode(String.Format("{0, -13}", o.State).Replace("_", " "));
            
            string statecolor = o.State == BugObject.BugSate.INVALID ? "red" :
                                   o.State == BugObject.BugSate.IN_PROGRESS ? "orange" :
                                   o.State == BugObject.BugSate.FIXED || o.State == BugObject.BugSate.DONE ? "green-light" : "blue-light";

            string prio = HTML.Encode(String.Format("{0, -5}", o.Priority));
            string priocolor = o.Priority.Equals("[!]") ? "red" :
                                o.Priority.Equals("[+]") ? "orange" :
                                o.Priority.Equals("[?]") ? "green-light" :
                                o.Priority.Equals("[-]") ? "blue-light" : "gray-light";
            
            string type = HTML.Encode(o.Type == BugObject.BugType.BUG ? "Bug: " : "");

            return String.Format(tmpla, id, statecolor, state, priocolor, prio, type, HTML.Encode(o.Description));
        }

        public BugObject UpdateObject(BugObject obj)
        {
            List<BugObject> l = GetBugList();
            BugObject nObj = null;

            for (int index = 0; index < l.Count; index++)
			{
                nObj = l[index];
                if (nObj.ID == obj.ID)
                {
                    nObj.State = obj.State;
                    nObj.Priority = obj.Priority;
                    nObj.Description = obj.Description;
                    break;
                }
            }

            SaveBugList(l);
            return nObj;
        }
        
        public BugObject GetObject(int id)
        {
            List<BugObject> l = GetBugList();
            BugObject nObj = null;

            for (int index = 0; index < l.Count; index++)
            {
                if (l[index].ID != id) continue;
                nObj = l[index];
                break;
            }

            return nObj;
        }

        private List<BugObject> GetBugList()
        {
            int counter = 0;
            string line;
            List<BugObject> l = new List<BugObject>();

            System.IO.StreamReader file = null;
            try
            {
                // Read the file and display it line by line.
                file = new System.IO.StreamReader(bugListDBPath);
                while ((line = file.ReadLine()) != null)
                {
                    string[] tbl = line.Split('\t');

                    BugObject.BugType bt = (BugObject.BugType)Enum.Parse(typeof(BugObject.BugType), tbl[2].Trim(), true);

                    BugObject o = new BugObject(bt, tbl[4].Trim());
                    o.ID = Int32.Parse(tbl[0].Trim());
                    o.State = (BugObject.BugSate) Enum.Parse(typeof(BugObject.BugSate), tbl[1].Trim(), true);
                    o.Priority = tbl[3].Trim();
                    l.Add(o);

                    counter++;
                }

                file.Close();
            }
            catch (Exception)
            {
                if (file != null) file.Close();
            }

            return l;
        }

        private void SaveBugList(List<BugObject> l)
        {
            // Read the file and display it line by line.
            System.IO.StreamWriter file = new System.IO.StreamWriter(bugListDBPath);

            foreach (BugObject o in l)
            {
                string s = o.ToString();
                file.WriteLine(s);
            }

            file.Close();
        }

    }

    public class BugObject
    {
        public enum BugSate{
            FIXED, DONE, NEW, INVALID, IN_PROGRESS
        }

        public enum BugType
        {
            TODO, BUG
        }

        public enum BugPrio
        {
            CRITICAL, HEIGH, NORMAL, LOW, MAYBE, QUESTION
        }

        public int ID { get; set; }
        public String Description { get; set; }
        public BugSate State { get; set; }
        public BugType Type { get; set; }
        public string Priority { get; set; }

        public BugObject(BugType t, string txt)
        {
            Type = t;
            State = BugSate.NEW;
            Description = txt;
            Priority = "[ ]";
            ID = 0;
        }

        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}\t{3}\t{4}", ID, State, Type, Priority, Description);
        }

        public static string GetPrio(BugPrio e)
        {
            switch (e)
            {
                case BugPrio.CRITICAL:
                    return "[!]";
                case BugPrio.HEIGH:
                    return "[+]";
                case BugPrio.NORMAL:
                    return "[ ]";
                case BugPrio.LOW:
                    return "[-]";
                case BugPrio.MAYBE:
                    return "[?]";
                case BugPrio.QUESTION:
                    return "[?]";
                default:
                    return "[ ]";
            }
        }

        public static BugPrio GetPrio(string p)
        {
            BugPrio e = BugPrio.NORMAL;
            try { e = (BugPrio)Enum.Parse(typeof(BugPrio), p, true); }
            catch
            {
                switch (p)
                {
                    case "!":
                        e = BugPrio.CRITICAL;
                        break;
                    case "+":
                        e = BugPrio.HEIGH;
                        break;
                    case "":
                        e = BugPrio.NORMAL;
                        break;
                    case "-":
                        e = BugPrio.LOW;
                        break;
                    case "?":
                        e = BugPrio.MAYBE;
                        break;
                    default:

                        BugPrio[] bps = (BugPrio[])Enum.GetValues(typeof(BugPrio));

                        foreach (BugPrio bp in bps)
                        {
                            if (bp.ToString().ToLower().StartsWith(p))
                                e = bp;
                        }

                        break;
                }   
            }
            return e;
        }
    }

}