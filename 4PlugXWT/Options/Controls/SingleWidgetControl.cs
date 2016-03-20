using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options.Controls
{
    public class SingleWidgetControl<T> : Control
        where T : Widget, new()
    {
        protected T control;
        protected double PaddingBottom = 2;

        public SingleWidgetControl()
        {
            control = new T();
            AddChild(control, 0, 0);
            Height = 26;
        }

        public SingleWidgetControl(double height)
            : this()
        {
            Height = height;
        }

        public override void PerformLayout(double width)
        {
            base.PerformLayout(width);

            SetChildBounds(control, new Rectangle(0, 0, width - 5, Height - PaddingBottom));
        }
    }
}
