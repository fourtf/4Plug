using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Widgets
{
    public class WidgetLayout<T> : Canvas
        where T : Widget
    {
        public WidgetSpacing Padding { get; set; }
        public double ItemWidth { get; set; }
        public WidgetSpacing ItemMargin { get; set; }

        public bool EnableCustomItemMove { get; set; }

        public WidgetLayout()
        {
            this.VerticalPlacement = WidgetPlacement.Fill;
            this.HorizontalPlacement = WidgetPlacement.Fill;

            ItemWidth = 300;
            ItemMargin = new WidgetSpacing(8, 8, 8, 8);
            EnableLayout = true;

            BoundsChanged += (s, e) => { Layout(); };
        }

        protected class Pair<TPair>
        {
            //public bool Initialized = false;
            public Point Location = Point.Zero;
            public T Widget;
        }

        protected List<Pair<T>> children = new List<Pair<T>>();

        public IEnumerable<T> Items
        {
            get
            {
                foreach (var pair in children)
                    yield return pair.Widget;
            }
        }


        public Func<T, T, bool> ControlComparer { get; set; }

        public bool CanSort
        {
            get { return ControlComparer != null; }
        }

        // Children
        public void AddChild(T widget)
        {
            int addIndex = 0;
            if (ControlComparer != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (ControlComparer(widget, children[i].Widget))
                        break;
                    addIndex = i + 1;
                }
            }
            else
            {
                addIndex = children.Count;
            }
            children.Insert(addIndex, new Pair<T> { Widget = widget });
            base.AddChild(widget, new Rectangle(0, 0, widget.WidthRequest, widget.HeightRequest));
            Layout();
        }

        public new void RemoveChild(Widget widget)
        {
            if (widget == null)
                return;
            children.Remove(children.First(p => p.Widget == widget));
            base.RemoveChild(widget);
            Layout();
        }

        public void ClearAll()
        {
            children.Clear();
            base.Clear();
        }

        // Layout
        public event EventHandler DidLayout;

        public bool EnableLayout { get; set; }

        public void Layout()
        {
            Layout(false);
        }

        public void Layout(bool animate)
        {
            if (children.Count > 0 && EnableLayout)
            {
                var margin = ItemMargin;    //children[0].Widget.Margin;
                var width = ItemWidth;     //children[0].Widget.WidthRequest;

                double _w = Bounds.Width - Padding.Right; // maximum width
                double padLeft =
                    ((_w - Padding.Left) / (width + margin.Left + margin.Right)
                    - (int)((_w - Padding.Left) / (width + margin.Left + margin.Right)))
                    * (width + margin.Left + margin.Right)
                    / 2
                    + Padding.Left;

                double x = padLeft;
                double y = Padding.Top;
                double _h = 0; // this lines max height

                foreach (Pair<T> p in children)
                {
                    var c = p.Widget;
                    if (c is PluginTitleWidget)
                    {
                        y += _h;
                        setBounds(p, new Rectangle(new Point(c.Margin.Left + padLeft, y + c.Margin.Top), c.Size), animate);
                        y += c.Margin.Top + c.HeightRequest + c.Margin.Bottom;
                        x = padLeft;
                        _h = 0;
                        //_h = c.Margin.Top + c.HeightRequest + c.Margin.Bottom;
                    }
                    else if (x + c.Margin.Left + c.WidthRequest < _w)
                    {
                        setBounds(p, new Rectangle(new Point(x + c.Margin.Left, y + c.Margin.Top), c.Size), animate);
                        x += c.Margin.Left + c.WidthRequest + c.Margin.Right;
                        _h = Math.Max(_h, c.Margin.Top + c.HeightRequest + c.Margin.Bottom);
                    }
                    else
                    {
                        y += _h;
                        x = padLeft + c.Margin.Left + c.WidthRequest + c.Margin.Right;
                        setBounds(p, new Rectangle(new Point(c.Margin.Left + padLeft, y + c.Margin.Top), c.Size), animate);

                        _h = c.Margin.Top + c.HeightRequest + c.Margin.Bottom;
                    }
                }
                this.MinHeight = this.HeightRequest = y + _h + Padding.Bottom;

                if (DidLayout != null)
                    DidLayout(this, EventArgs.Empty);
            }
        }

        void setBounds(Pair<T> pair, Rectangle rect, bool animate)
        {
            if (pair.Location.X != rect.X || pair.Location.Y != rect.Y)
            {
                if (!EnableCustomItemMove || App.MoveWidget == null)
                    SetChildBounds(pair.Widget, rect);
                else
                {
                    App.MoveWidget((Xwt.Backends.IWidgetBackend)Xwt.Toolkit.GetBackend(pair.Widget), pair.Location.X, pair.Location.Y, rect.X, rect.Y, animate);
                    pair.Location = new Point(rect.X, rect.Y);
                }
            }
        }

        // Sort
        public void Sort(bool animate)
        {
            for (int i = children.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (ControlComparer(children[j + 1].Widget, children[j].Widget))
                        children.Swap(j, j + 1);
                }
            }
            Layout(animate);
        }

        protected class AlphanumComparatorFast : System.Collections.IComparer
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
    }
}
