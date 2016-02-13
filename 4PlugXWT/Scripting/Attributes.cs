using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FPlug.Scripting;

namespace FPlug.Scripting
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class ScriptClassAttribute : Attribute
    {
        public string Name { get; private set; }

        //public ScriptType Extends { get; set; }

        public ScriptClassAttribute(string name)
        {
            Name = name;
            //Extends = typeof(SChild);
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class ScriptMemberAttribute : Attribute
    {
        public ScriptTypeID ReturnType { get; private set; }

        public string Name { get; set; }

        public bool AutoAdd { get; set; }

        public ScriptMemberAttribute(ScriptTypeID returnType)
        {
            ReturnType = returnType;
            AutoAdd = true;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class ScriptEventAttribute : Attribute
    {
        public ScriptEventAttribute()
        {

        }
    }
}
