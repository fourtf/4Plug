using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Widgets
{
    public class PagedViewer : Canvas
    {
        List<Widget> pages = new List<Widget>();

        public IEnumerable<Widget> Widgets
        {
            get
            {
                return pages;
            }
        }

        private Widget selectedWidget;

        public Widget SelectedWidget
        {
            get
            {
                return selectedWidget;
            }
            set
            {
                if (selectedWidget != value)
                {
                    if (!pages.Contains(value))
                        throw new ArgumentException("The widget is not a child of this PagedViewer");
                    selectedWidget = value;
                }
            }
        }

        public int Count { get { return pages.Count; } }

        public void Add(Widget widget)
        {
            widget.Visible = false;
        }

        protected override void OnBoundsChanged()
        {
            base.OnBoundsChanged();


        }
    }
}
