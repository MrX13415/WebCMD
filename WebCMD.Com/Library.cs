using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Com
{
    public class Library
    {
        public int CommandCount { get; private set; }
        public int RegisteredCount { get; private set; }
        public bool IsValid { get; private set; }

        public FileInfo File { get; private set; }
        public Assembly Assembly { get; private set; }

        private Library(FileInfo file, Assembly assembly)
        {
            this.Assembly = assembly;
            this.File = file;
            this.CommandCount = 0;
            this.RegisteredCount = 0;
            this.IsValid = false;
        }

        public static Library From(string s)
        {
            try
            { return From(new FileInfo(s)); }
            catch
            { return new Library(null, null); }
        }

        public static Library From(FileInfo f)
        {
            try
            {
                Library lib = From(Assembly.LoadFile(f.FullName));
                lib.File = f;
                return lib;
            }
            catch { return new Library(f, null); }
        }

        public static Library From(Assembly assembly)
        {
            return new Library(new FileInfo(assembly.Location), assembly);
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
            { return t.BaseType.Equals(typeof(WebCMD.Com.ServerCommand)); }
            catch
            { return false; }
        }
    }
}
