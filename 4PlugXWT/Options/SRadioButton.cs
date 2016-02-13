using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options
{
    //[SChild("Radio")]
    public class SRadioButton : SSingleWidget<RadioButton>
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

        static RadioButtonGroup group = new RadioButtonGroup();

        public SRadioButton()
        {
            //Height = 16;
            //Height += 1;
            PaddingBottom = 1;

            control.Active = false;
            control.Group = group;
            control.Label = "CheckBox";

            control.ActiveChanged += (s, e) => { if (OnValueChanged != null) OnValueChanged.Execute(); };
        }

        public override void CompileScripts()
        {
            base.CompileScripts();

            if (OnValueChanged != null)
                OnValueChanged.Compile();
        }
    }
}
