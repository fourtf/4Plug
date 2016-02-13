using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Scripting
{
    public class TF2SchemeLine
    {
        public string Text { get; set; }
        public string Name { get; set; }

        public int NameStart { get; set; }
        public int NameLength { get; set; }
        public int ValueStart { get; set; }
        public int ValueLength { get; set; }

        public TF2SchemeLine(string text)
        {
            Text = text;
        }

        //private string GetName()
        //{
        //    return Text.Substring(NameStart, NameLength);
        //}

        public void SetValue(string value)
        {
            Text = Text.Remove(ValueStart) + value + Text.Substring(ValueStart + ValueLength);
            ValueLength = value.Length;
        }

        public string GetValue()
        {
            return Text.Substring(ValueStart, ValueLength);
        }
    }
}
