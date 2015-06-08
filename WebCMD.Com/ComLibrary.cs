using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Com
{
    public class ComLibrary
    {
        public int CommandCount { get; private set; }
        public int RegisteredCount { get; private set; }

        public bool IsValid { get; private set; }
        public bool IsLoaded { get; private set; }

        public FileInfo File { get; private set; }
        public Assembly Assembly { get; private set; }

        private ComLibrary(FileInfo file)
        {
            this.CommandCount = 0;
            this.RegisteredCount = 0;
            this.IsValid = false;
            this.IsLoaded = false;

            this.File = file;
        }
        
        public static ComLibrary Instance(string s)
        {
            return Instance(new FileInfo(s));
        }

        public static ComLibrary Instance(FileInfo f)
        {
            return new ComLibrary(f);
        }

        public static ComLibrary Instance(Assembly assembly)
        {
            return new ComLibrary(new FileInfo(assembly.Location));
        }

        public bool Load()
        {

            byte[] libfile = System.IO.File.ReadAllBytes(File.FullName);
            byte[] pdbfile = null;

            try { pdbfile = System.IO.File.ReadAllBytes(File.FullName.ToLower().Replace(".dll", ".pdb")); } catch { }
            
            if (pdbfile != null) this.Assembly = Assembly.Load(libfile, pdbfile);
            else this.Assembly = Assembly.Load(libfile);

            this.IsValid = false;
            this.CommandCount = 0;
            this.RegisteredCount = 0;
            TypeInfo[] types = Assembly.DefinedTypes.ToArray() as TypeInfo[];

            foreach (TypeInfo typeI in types)
            {
                if (!IsServerCommand(typeI)) continue;
                CommandCount++;

                try
                {
                    // Register Assembly type
                    Command command = Assembly.CreateInstance(typeI.FullName) as Command;

                    if (command != null)
                    {
                        command.Library = this;     //define the lib
                        command.Register();         //register the command
                        RegisteredCount++;          //update the counter
                        IsValid = true;             
                        IsLoaded = true;
                        Debug.WriteLine(" (i)  Command type loaded: " + typeI.Name + " Library: " + Assembly.FullName);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(" (x)  Error while loading command type:  " + typeI.Name + " Library: " + Assembly.FullName + " Error: " + ex);
                }
            }

            if (IsValid)
                Debug.WriteLine(" (i)  Library loaded" + (pdbfile != null ? " with debug symbols" : "") + ": " + Assembly.FullName + " (" + RegisteredCount + " commands)");
            else
                Debug.WriteLine(" /!\\  Invalid library: " + Assembly.FullName);

            return IsValid;
        }

        public static bool IsServerCommand(TypeInfo t)
        {
            if (t.BaseType == null) return false;
            return t.BaseType.FullName == typeof(WebCMD.Com.Command).FullName;
        }
    }
}
