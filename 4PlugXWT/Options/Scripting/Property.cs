using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Options.Scripting
{
    public class Property
    {
        public string Name { get; set; }

        public Action<object, Variable> SetVariable { get; set; }
        public Func<object, Variable> GetVariable { get; set; }
    }
}
