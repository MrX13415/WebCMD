using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebCMD.Util.WebConfig;

namespace WebCMD.Core.IO
{
    public class VirtualPath
    {
        public static string CurrentDirAlias = ".";
        public static string ParentDirAlias = "..";
        public static char DirSeparatorChar = '/';
        public static char AltDirSeparatorChar = '\\';
        public static string SystemRootDirectory = Map(DirChar);

        private static string DirChar { get { return DirSeparatorChar.ToString(); } }
        private static string AltDirChar { get { return AltDirSeparatorChar.ToString(); } }
        

        public static string Combine(params string[] paths){

            string finalpath = "";

            foreach (string rawpath in paths)
            {
                string path = rawpath.Trim().Replace(AltDirChar, DirChar); // "  fency/raw\path " ==> "fency/raw/path"
                if (path.Equals("")) continue;

                finalpath = String.Concat(finalpath, path, path.EndsWith(DirChar) ? "" : DirChar);
                
                if (path.StartsWith(DirChar)) finalpath = path; 
            }

            return finalpath;
        }

        public static bool Compare(string path0, string path1)
        {
            return path0.Trim().Replace(AltDirChar, DirChar).ToLower().Equals(path1.Trim().Replace(AltDirChar, DirChar).ToLower());
        }

        public static string Map(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }

        public static string GetServerPath(string path)
        {
            path = path.Trim().Replace(AltDirChar, DirChar);
            if (path.StartsWith(DirChar)) path = path.Substring(1);
            return Combine(SystemRootDirectory, path);
        }
        
        public static string GetSimplifiedPath(string vpath)
        {
            try
            {
                vpath = vpath.Trim().Replace(AltDirChar, DirChar);

                string parentDirChar = "..";
                string pattern = String.Concat(DirChar, parentDirChar, DirChar);
                string current = vpath;

                while (current.Contains(pattern))
                {
                    int i = current.IndexOf(pattern);

                    string area = current.Substring(0, i);

                    int a = area.LastIndexOf(DirChar);
                    int z = i + 3;

                    current = String.Concat(current.Substring(0, a), current.Substring(z));
                }

                // ".../test/blah/.." ==> ".../test"
                if (current.EndsWith(String.Concat(DirChar, parentDirChar)))
                {
                    current = current.Substring(0, current.Length - 3);
                    current = current.Substring(0, current.LastIndexOf(DirChar));
                }

                return current;
            }
            catch
            {
                throw new ArgumentException("Invalid path provied!");
            }
        }

        public static string GetSmartPath(string baseDirectory, string vpath)
        {
            //tirm 
            baseDirectory = baseDirectory.Trim().Replace(AltDirChar, DirChar);
            vpath = vpath.Trim().Replace(AltDirChar, DirChar);

            //make sure the base path is valid 
            if (baseDirectory.Equals("")) baseDirectory = DirChar;
            if (!baseDirectory.EndsWith(DirChar)) baseDirectory += DirChar;

            // make sure the provied path does not already contain the base path
            if (vpath.Length > baseDirectory.Length && vpath.Substring(0, baseDirectory.Length).Equals(baseDirectory))
                vpath = vpath.Substring(baseDirectory.Length);

            if (vpath.StartsWith(DirChar))
            {
                baseDirectory = DirChar;
                vpath = vpath.Substring(1);
            }

            // determine the root search path
            string phyBase = GetServerPath(baseDirectory);
            if (!Directory.Exists(phyBase)) baseDirectory = DirChar;
            
            //split the path into directorys
            string[] dirs = vpath.Split(DirSeparatorChar);

            string current = baseDirectory;

            foreach (string dir in dirs)
            {
                string next = Combine(current, dir);

                // directory with exact name found!
                if (Directory.Exists(GetServerPath(next)))
                {
                    current = next; continue;
                }

                string pattern = String.Concat("*", dir, "*");
                string[] list = Directory.GetDirectories(GetServerPath(current), pattern);

                // path is invalid!
                if (list.Count() == 0) return current;

                current = Combine(current, Path.GetFileName(list[0]));
            }

            return current;
        }

        public static string GetParentPath(string path)
        {
            //trim
            path = path.Trim().Replace(AltDirChar, DirChar);
            //remove last "/"
            if (path.EndsWith(DirChar)) path = path.Substring(0, path.Length - 1);
            // the given path has no more parents
            if (!path.Contains(DirChar)) return path;
            // get the parent dor path
            path = path.Substring(0, path.LastIndexOf(DirChar));
            // empty string is root level
            if (String.IsNullOrEmpty(path)) path = DirChar;
            return path;
        }

        public static bool ServerPathExists(string path)
        {
            return File.Exists(GetServerPath(path)) || Directory.Exists(GetServerPath(path));
        }

        public static bool Exists(string path)
        {
            //check config
            WebConfiguration cfg = new WebConfiguration(path);
            return cfg.DirectoryBrowse.Enabled && ServerPathExists(path);
        }

        public static bool IsFullPath(string path)
        {
            path = path.Trim().Replace(AltDirChar, DirChar);
            return path.StartsWith(DirChar);
        }

        public static bool IsValid(string path)
        {
            //make sure there are no invalid chars ...
            foreach (char c in Path.GetInvalidPathChars())
                if (path.Contains(c)) return false;

            return true;
        }

    }
}
