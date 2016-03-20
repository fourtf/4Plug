using FPlug.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt.Drawing;

namespace FPlug.Options.Controls
{
    public class ColorControl : SingleWidgetControl<ColorPicker>
    {
        public ColorControl()
        {
            //PaddingBottom = 2;

            control.ColorChanged += (s, e) => { Color = control.Color; };
        }

        [ScriptMember("color")]
        public Color Color
        {
            get
            {
                return control.Color;
            }
            set
            {
                control.Color = value;
            }
        }

        [ScriptMember("default")]
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
    }
}
