using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FPlug.Scripting;

namespace FPlug.Options
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class SChildAttribute : Attribute
    {
        public string Name { get; private set; }

        public Type Extends { get; set; }

        public SChildAttribute(String name)
        {
            Name = name;
            Extends = typeof(SChild);
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class SPropertyAttribute : Attribute
    {
        public ScriptTypeID Types { get; private set; }

        public String CustomName { get; set; }

        public bool AutoAdd { get; set; }

        public SPropertyAttribute(ScriptTypeID propertyType)
        {
            Types = propertyType;
            AutoAdd = true;
        }
    }
}
