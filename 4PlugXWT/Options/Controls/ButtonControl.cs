using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options.Controls
{
    public class ButtonControl : SingleWidgetControl<Button>
    {
        [ScriptMember("text")]
        public string Text
        {
            get { return control.Label; }
            set { control.Label = value; }
        }

        public ButtonControl()
        {
            control.Label = "sample";
        }
    }
}
