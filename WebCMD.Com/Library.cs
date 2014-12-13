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

        public FileInfo File { get; private set; }
        public Assembly Assembly { get; private set; }

        private ComLibrary(FileInfo file, Assembly assembly)
        {
            this.Assembly = assembly;
            this.File = file;
            this.CommandCount = 0;
            this.RegisteredCount = 0;
            this.IsValid = false;
        }

        public static ComLibrary From(string s)
        {
            try
            { return From(new FileInfo(s)); }
            catch
            { return new ComLibrary(null, null); }
        }

        public static ComLibrary From(FileInfo f)
        {
            try
            {
                ComLibrary lib = From(Assembly.LoadFile(f.FullName));
                lib.File = f;
                return lib;
            }
            catch { return new ComLibrary(f, null); }
        }

        public static ComLibrary From(Assembly assembly)
        {
            return new ComLibrary(new FileInfo(assembly.Location), assembly);
        }

        public bool Load()
        {
            IsValid = false;
            CommandCount = 0; RegisteredCount = 0;

            foreach (Type type in Assembly.GetTypes())
            {
                  if (!IsServerCommand(type)) continue;
                CommandCount++;

                try
                {
                    ComLoader.RegisterAssemblyType(Assembly, type);
                    RegisteredCount++;
                    IsValid = true;
                }
                catch (Exception)
                {

                }
            }
            return IsValid;
        }

        public static bool IsServerCommand(Type t)
        {
            try
            {
                return t.BaseType.FullName == typeof(WebCMD.Com.ServerCommand).FullName;
            }
            catch
            { return false; }
        }
    }
}
