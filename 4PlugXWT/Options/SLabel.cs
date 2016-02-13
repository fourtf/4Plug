using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options
{
    [FPlug.Scripting.ScriptClass("Label")]
    public class SLabel : SSingleWidget<Label>
    {
        private string text;

        [FPlug.Scripting.ScriptMember(Scripting.ScriptTypeID.String)]
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;
                    control.Text = text;
                }
            }
        }

        public SLabel()
        {
            control.Text = "Label";
        }
    }
}
