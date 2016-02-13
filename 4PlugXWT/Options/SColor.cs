using FPlug.Scripting;
using FPlug.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt.Drawing;

namespace FPlug.Options
{
    [Scripting.ScriptClass("Color")]
    public class SColor : SSingleWidget<ColorPicker>
    {
        public SColor()
        {
            PaddingBottom = 2;
            SaveSource = true;

            control.ColorChanged += (s, e) => { Color = control.Color; };
        }

        [ScriptMember(ScriptTypeID.Color)]
        public Color Color
        {
            get
            {
                return control.Color;
            }
            set
            {
                control.Color = value;
                if (OnValueChanged != null)
                    OnValueChanged.Execute();
            }
        }

        [ScriptMember(ScriptTypeID.Color)]
        public Color Default
        {
            get
            {
                return control.DefaultColor;
            }
            set
            {
                control.DefaultColor = value;
            }
        }

        [ScriptEvent]
        public Script OnValueChanged { get; set; }

        private string source;

        Source _source = null;

        [ScriptMember(ScriptTypeID.String)]
        public string Source
        {
            get
            {
                return source;
            }
            set
            {
                if (source != value)
                {
                    source = value;

                    _source = new Source(Window, source);
                    var c =_source.GetColor();
                    if (c != null)
                        Color = c.Value;
                }
            }
        }

        [ScriptMember(ScriptTypeID.Boolean)]
        public bool SaveSource { get; set; }

        public override void CompileScripts()
        {
            base.CompileScripts();

            if (OnValueChanged != null)
                OnValueChanged.Compile();
        }

        public override void Apply()
        {
            base.Apply();

            if (SaveSource && _source != null)
                _source.SetColor(Color);
        }
    }
}
