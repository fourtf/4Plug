using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FPlug.Options
{
    public class ControlDesc
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public ScriptType ScriptType { get; set; }
        public List<PropertyDesc> Properties = new List<PropertyDesc>();
        public Dictionary<string, PropertyInfo> Events = new Dictionary<string, PropertyInfo>();

        //public ControlDesc(string name, Types type)
        //{
        //    Name = name;
        //    Type = type;
        //}

        public override string ToString()
        {
            return Name + " :" + Type;
        }
    }

    public class PropertyDesc
    {
        public ScriptType ScriptType { get; set; }
        public string Name { get; set; }
        public Action<object, object> Set { get; set; }
        public Func<object, object> Get { get; set; }
    }
}
