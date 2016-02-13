using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Options
{
    [ScriptClass("Group")]
    public class SGroup : SContainer
    {
        private string text;

        [ScriptMember(ScriptTypeID.String)]
        public string Text
        {
            get { return text; }
            set { text = value; QueueDraw(); }
        }

        protected double paddingLeft = 8;
        protected double paddingRight = 8;
        protected double paddingTop = 18;
        protected double paddingBottom = 9;

        public IEnumerable<SContainer> GetContainers()
        {
            foreach (SChild c in children)
                if (c is SContainer)
                    yield return (SContainer)c;
        }

        protected GroupType _GroupType;

        protected enum GroupType
        {
            None, Group
        }

        public SGroup(Color bgColor)
            : this()
        {
            this.BackgroundColor = bgColor;
        }

        public SGroup()
        {
            text = "Group";

            children = new List<SChild>();

            //Default Size
            Height = 100;
            Width = 500;

            _GroupType = GroupType.Group;
        }

        public override void Layout(double width)
        {
            base.Layout(width);

            double _w = Math.Max(width - paddingLeft - paddingRight, 20); // max width

            double x = 0;
            double y = paddingTop;
            double _h = 0; // this lines max height

            foreach (SChild c in children)
            {
                // Auto Width
                if (c.WidthPercentage == 0)
                {
                    if (x + 20 <= _w)
                    {
                        double w = _w - x;
                        c.Layout(w);
                        SetChildBounds(c, new Rectangle(x + paddingLeft, y, c.Width, c.Height));
                        y += Math.Max(_h, c.Height);
                    }
                    else
                    {
                        y += _h;
                        c.Layout(_w);
                        SetChildBounds(c, new Rectangle(paddingLeft, y, c.Width, c.Height));
                        y += c.Height;
                    }
                    _h = 0;
                    x = 0;
                }
                // Width %
                else
                {
                    int w = (int)(_w * c.WidthPercentage);
                    c.Layout(w);
                    if (x + w <= _w)
                    {
                        SetChildBounds(c, new Rectangle(x + paddingLeft, y, c.Width, c.Height));
                        x += w;
                        _h = Math.Max(_h, c.Height);
                    }
                    else
                    {
                        y += _h;
                        x = w;
                        SetChildBounds(c, new Rectangle(paddingLeft, y, c.Width, c.Height));
                        _h = c.Height;
                    }
                }
            }
            y += paddingBottom;

            Height = Math.Max(y + _h, 30);
        }

        protected override void OnAddChild(SChild child)
        {
            AddChild((Widget)child);
        }

        protected override void OnRemoveChild(SChild child)
        {
            RemoveChild((Widget)child);
        }

        protected override void OnMoveChild(SChild child, int index)
        {
            children.Remove(child);
            children.Insert(index, child);
        }

        //public void Remove()
        //{
        //    if (children.Count > 0)
        //        foreach (SChild b in children)
        //            b.Parent.RemoveChild(b);
        //    Parent.RemoveChild(this);
        //}

        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            //base.OnDraw(ctx, dirtyRect);

            if (_GroupType == GroupType.Group)
            {
                ctx.SetColor(Colors.Gray);
                ctx.SetLineWidth(1);

                ctx.RoundRectangle(0, paddingTop / 2 - .5, Width - 4.5, Height - paddingBottom * 1.5, 3);
                ctx.Stroke();

                var tl = new TextLayout(this) { Text = text, Width = this.Width - paddingLeft - paddingRight / 2 };

                ctx.SetColor(Colors.White);
                ctx.Rectangle(paddingLeft, 0, tl.GetSize().Width, paddingTop);
                ctx.Fill();

                ctx.SetColor(Colors.Black);
                ctx.DrawTextLayout(tl, paddingLeft, 0);
            }
        }
    }

    [ScriptClass("Box")]
    public class SBox : SGroup
    {
        public SBox()
        {
            _GroupType = GroupType.None;
            paddingLeft = paddingRight = paddingTop = paddingBottom = 0;
        }
    }
}
