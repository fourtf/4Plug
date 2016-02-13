using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug
{
    public class PluginWidgetLayout : Canvas
    {
        public WidgetSpacing Padding { get; set; }

        //CHILDREN
        List<Widget> children = new List<Widget>();

        public new void AddChild(Widget widget)
        {
            if (widget == null)
                return;
                int addIndex = 0;
                for (int i = 0; i < children.Count; i++)
                {
                    if (compareControls(widget, children[i]))
                        break;
                    addIndex = i + 1;
                }
                children.Insert(addIndex, widget);
                base.AddChild(widget);
            Layout();
        }

        public new void RemoveChild(Widget widget)
        {
            if (widget == null)
                return;
            children.Remove(widget);
            base.RemoveChild(widget);
            Layout();
        }

        public void Insert(Widget widget, int index)
        {
            children.Insert(index, widget);
            base.AddChild(widget);
            Layout();
        }

        public int IndexOf(Widget widget)
        {
            return children.IndexOf(widget);
        }

        bool compareControls(object a, object b)
        {
            if (((IHasPluginType)a).PluginType > ((IHasPluginType)b).PluginType)
                return false;
            else if (((IHasPluginType)a).PluginType == ((IHasPluginType)b).PluginType)
            {
                if (a is PluginTitleWidget)
                    return true;
                if (b is PluginTitleWidget)
                    return false;

                if (a is AddPluginWidget)
                    return false;
                if (b is AddPluginWidget)
                    return true;

                //return String.Compare(((PluginWidget)a).Name, ((PluginWidget)b).Name) != 1;
                return new AlphanumComparatorFast().Compare(((PluginWidget)a).Name, ((PluginWidget)b).Name) != 1;
            }
            return true;
        }

        class AlphanumComparatorFast : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                string s1 = x as string;
                if (s1 == null)
                {
                    return 0;
                }
                string s2 = y as string;
                if (s2 == null)
                {
                    return 0;
                }

                int len1 = s1.Length;
                int len2 = s2.Length;
                int marker1 = 0;
                int marker2 = 0;

                // Walk through two the strings with two markers.
                while (marker1 < len1 && marker2 < len2)
                {
                    char ch1 = s1[marker1];
                    char ch2 = s2[marker2];

                    // Some buffers we can build up characters in for each chunk.
                    char[] space1 = new char[len1];
                    int loc1 = 0;
                    char[] space2 = new char[len2];
                    int loc2 = 0;

                    // Walk through all following characters that are digits or
                    // characters in BOTH strings starting at the appropriate marker.
                    // Collect char arrays.
                    do
                    {
                        space1[loc1++] = ch1;
                        marker1++;

                        if (marker1 < len1)
                        {
                            ch1 = s1[marker1];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                    do
                    {
                        space2[loc2++] = ch2;
                        marker2++;

                        if (marker2 < len2)
                        {
                            ch2 = s2[marker2];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                    // If we have collected numbers, compare them numerically.
                    // Otherwise, if we have strings, compare them alphabetically.
                    string str1 = new string(space1);
                    string str2 = new string(space2);

                    int result;

                    if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                    {
                        int thisNumericChunk = int.Parse(str1);
                        int thatNumericChunk = int.Parse(str2);
                        result = thisNumericChunk.CompareTo(thatNumericChunk);
                    }
                    else
                    {
                        result = str1.CompareTo(str2);
                    }

                    if (result != 0)
                    {
                        return result;
                    }
                }
                return len1 - len2;
            }
        }

        //CTOR
        public PluginWidgetLayout()
        {
            this.VerticalPlacement = WidgetPlacement.Fill;
            this.HorizontalPlacement = WidgetPlacement.Fill;
            BoundsChanged += FlowLayoutControl_BoundsChanged;
        }

        void FlowLayoutControl_BoundsChanged(object sender, EventArgs e)
        {
            Layout();
        }

        EmptyElement emptyElement = null;

        public void EnableEmptyElement()
        {
            emptyElement = new EmptyElement();
            base.AddChild(emptyElement);
        }

        class EmptyElement : Canvas
        {
            Xwt.Drawing.Color bg = new Xwt.Drawing.Color(1, 1, 1, 0);
            protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
            {
                ctx.SetColor(bg);
                ctx.Rectangle(dirtyRect);
                ctx.Fill();
            }
        }

        //LAYOUT
        public bool layout = true;
        public void PauseLayout()
        {
            layout = false;
        }

        public void ResumeLayout()
        {
            layout = true;
            Layout();
        }

        public event EventHandler DidLayout;

        public void Layout()
        {
            if (layout)
            {
                double _w = Bounds.Width - Padding.Right; // maximum width
                double padLeft = 
                    ((_w - Padding.Left) / (PluginWidget.Size.Width + PluginWidget.Margin.Left + PluginWidget.Margin.Right)
                    - (int)((_w - Padding.Left) / (PluginWidget.Size.Width + PluginWidget.Margin.Left + PluginWidget.Margin.Right)))
                    * (PluginWidget.Size.Width + PluginWidget.Margin.Left + PluginWidget.Margin.Right)
                    / 2
                    + Padding.Left;

                double x = padLeft;
                double y = Padding.Top;
                double _h = 0; // this lines max height

                foreach (Widget c in children)
                {
                    if (c is PluginTitleWidget)
                    {
                        y += _h;
                        SetChildBounds(c, new Rectangle(new Point(c.Margin.Left + padLeft, y + c.Margin.Top), c.Size));
                        y += c.Margin.Top + c.HeightRequest + c.Margin.Bottom;
                        x = padLeft;
                        _h = 0;
                        //_h = c.Margin.Top + c.HeightRequest + c.Margin.Bottom;
                    }
                    else if (x + c.Margin.Left + c.WidthRequest < _w)
                    {
                        SetChildBounds(c, new Rectangle(new Point(x + c.Margin.Left, y + c.Margin.Top), c.Size));
                        x += c.Margin.Left + c.WidthRequest + c.Margin.Right;
                        _h = Math.Max(_h, c.Margin.Top + c.HeightRequest + c.Margin.Bottom);
                    }
                    else
                    {
                        y += _h;
                        x = padLeft + c.Margin.Left + c.WidthRequest + c.Margin.Right;
                        SetChildBounds(c, new Rectangle(new Point(c.Margin.Left + padLeft, y + c.Margin.Top), c.Size));
                        
                        _h = c.Margin.Top + c.HeightRequest + c.Margin.Bottom;
                    }
                }
                this.MinHeight = this.HeightRequest = y + _h + Padding.Bottom;
                //if (emptyElement != null)
                //    SetChildBounds(emptyElement, new Rectangle(0, 0, Bounds.Width, Bounds.Height));

                if (DidLayout != null)
                    DidLayout(this, EventArgs.Empty);
            }
        }
    }

    public interface IHasPluginType
    {
        PluginType PluginType { get; set; }
    }
}
