using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Options.Controls
{
    //[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    //sealed class ScriptClassAttribute : Attribute
    //{
    //    public string Name { get; private set; }
    //
    //    //public ScriptType Extends { get; set; }
    //
    //    public ScriptClassAttribute(string name)
    //    {
    //        Name = name;
    //        //Extends = typeof(SChild);
    //    }
    //}

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class ScriptMemberAttribute : Attribute
    {
        public string Name { get; private set; }
    
        public ScriptMemberAttribute(string name)
        {
            Name = name;
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
