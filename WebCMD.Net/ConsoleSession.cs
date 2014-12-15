using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebCMD.Net
{
    public class ConsoleSession
    {
        
        //TODO IMPROVE THIS CLASS !
        
        private Dictionary<object, object> _session = new Dictionary<object, object>();

        public object this[object key]
        {
            get { return Retrieve(key); }
            set { Store(key, value); }
        }

        public void Store(object key, object value)
        {
            string _uid = key.ToString();

            if (key is Type) _uid = String.Concat("Type_", (key as Type).AssemblyQualifiedName);
            if (key is TypeInfo) _uid = String.Concat("TypeInfo_", (key as TypeInfo).AssemblyQualifiedName);
   
            _session[_uid] = value;
        }

        public object Retrieve(object key)
        {
            if (!_session.ContainsKey(key)) return null;
            return _session[key]; 
        }
    }
}
