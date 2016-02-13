using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options
{
    [ScriptClass("Text")]
    public class STextInput : SSingleWidget<TextEntry>
    {
        [ScriptMember(ScriptTypeID.String)]
        public string Text
        {
            get { return control.Text; }
            set { control.Text = value; }
        }

        [ScriptEvent]
        public Script OnValueChanged { get; set; }

        public STextInput()
        {
            //Height += 2;
            PaddingBottom = 2;
            control.PlaceholderText = "Write Something";
        }
    }
}
