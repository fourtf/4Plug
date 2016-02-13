using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Options
{
    public class SControlCollection : SChild
    {
        protected SGroup box;

        public SControlCollection()
        {
            box = new SGroup();
            AddChild(box);
        }

        public override void Layout(double width)
        {
            base.Layout(width);
            box.Layout(width);
            Height = box.Height;
            SetChildBounds(box, new Xwt.Rectangle(0, 0, width, Height));
        }
    }
}
