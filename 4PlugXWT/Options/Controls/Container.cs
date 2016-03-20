using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options.Controls
{
    public class Container : Control, IContainer
    {
        // controls
        protected List<Control> controls { get; set; } = new List<Control>();
        public IList<Control> Controls { get; private set; }

        protected WidgetSpacing Padding { get; set; }


        // ctor
        public Container()
        {
            Controls = controls.AsReadOnly();
        }

        public void AddControl(Control c)
        {
            c.Window = Window;
            controls.Add(c);
            AddChild(c);
            //SetChildBounds(c, new Rectangle(0, 0, 50, 50));
        }

        public override void PerformLayout(double width)
        {
            base.PerformLayout(width);
            
            double _w = Math.Max(width - Padding.Left - Padding.Right, 20); // max width
            
            double x = 0;
            double y = Padding.Top;
            double _h = 0; // this lines max height
            
            foreach (Control c in controls)
            {
                // Auto Width
                if (c.WidthPercent == 0)
                {
                    if (x + 20 <= _w)
                    {
                        double w = _w - x;
                        c.PerformLayout(w);
                        SetChildBounds(c, new Rectangle(x + Padding.Left, y, c.Width, c.Height));
                        y += Math.Max(_h, c.Height);
                    }
                    else
                    {
                        y += _h;
                        c.PerformLayout(_w);
                        SetChildBounds(c, new Rectangle(Padding.Left, y, c.Width, c.Height));
                        y += c.Height;
                    }
                    _h = 0;
                    x = 0;
                }
                // Width %
                else
                {
                    int w = (int)(_w * c.WidthPercent);
                    c.PerformLayout(w);
                    if (x + w <= _w)
                    {
                        SetChildBounds(c, new Rectangle(x + Padding.Left, y, c.Width, c.Height));
                        x += w;
                        _h = Math.Max(_h, c.Height);
                    }
                    else
                    {
                        y += _h;
                        x = w;
                        SetChildBounds(c, new Rectangle(Padding.Left, y, c.Width, c.Height));
                        _h = c.Height;
                    }
                }
            }
            y += Padding.Bottom;

            MinHeight = Height = Math.Max(y + _h, 30);
        }
    }
}
