using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options
{
    [ScriptClass("Check")]
    public class SCheckBox : SSingleWidget<CheckBox>
    {
        [ScriptMember(Scripting.ScriptTypeID.String)]
        public string Text
        {
            get
            {
                return control.Label;
            }
            set
            {
                control.Label = value;
            }
        }

        [ScriptMember(Scripting.ScriptTypeID.Boolean)]
        public bool Checked
        {
            get
            {
                return control.Active;
            }
            set
            {
                if (control.Active != value)
                    control.Active = value;
            }
        }

        [ScriptEvent]
        public Script OnValueChanged { get; set; }

        //private int myVar;

        public int Int
        {
            get
            {
                return 5;
            }
            set
            {
                //myVar = value;
            }
        }

        public override void CompileScripts()
        {
            base.CompileScripts();

            if (OnValueChanged != null)
                OnValueChanged.Compile();
        }

        public SCheckBox()
        {
            //Height = 16;
            //Height += 1;
            PaddingBottom = 1;

            control.Label = "Radio";

            control.Toggled += (s, e) => { if (OnValueChanged != null) OnValueChanged.Execute(); };
        }
    }
}
