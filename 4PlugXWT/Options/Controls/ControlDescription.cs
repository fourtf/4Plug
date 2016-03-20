using FPlug.Options.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Options.Controls
{
    public class ControlType
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Dictionary<string, Property> Properties { get; set; } = new Dictionary<string, Property>();

        public Control CreateInstance()
        {
            return (Control)Activator.CreateInstance(Type);
        }
    }
}
