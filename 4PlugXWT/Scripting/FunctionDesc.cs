using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Scripting
{
    public class FunctionDesc
    {
        public string Name { get; private set; }
        public ScriptTypeID InTypes { get; private set; }
        public ScriptTypeID ParameterTypes { get; private set; }
        public ScriptTypeID OutType { get; private set; }
        public Func<Function> Create { get; private set; }

        public FunctionDesc(string name, ScriptTypeID inTypes, ScriptTypeID parameterType, ScriptTypeID outType, Func<Function> create)
        {
            Name = name;
            InTypes = inTypes;
            ParameterTypes = parameterType;
            OutType = outType;
            Create = create;
        }
    }
}
