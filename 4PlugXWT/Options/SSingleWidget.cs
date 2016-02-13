using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options
{
    public class SSingleWidget<T> : SChild
        where T : Widget, new() 
    {
        protected T control;
        protected double PaddingBottom = 0;

        public SSingleWidget()
        {
            control = new T();
            AddChild(control, 0, 0);
            Height = 25;
        }

        public SSingleWidget(double height)
            : this()
        {
            this.Height = height;
        }

        public override void Layout(double width)
        {
            base.Layout(width);

            SetChildBounds(control, new Rectangle(0, 0, width - 5, Height - PaddingBottom));
        }
    }
}
