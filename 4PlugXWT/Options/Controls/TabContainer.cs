using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options.Controls
{
    public class TabContainer : Notebook, IContainer, IControl
    {
        public SettingsWindow Window { get; set; }

        public double Height { get; set; } = 25;
        public double Width { get; set; } = 100;

        List<NotebookTab> tabs = new List<NotebookTab>();

        protected List<Control> controls { get; set; } = new List<Control>();

        public IList<Control> Controls { get; private set; }

        public TabContainer()
        {
            Controls = controls.AsReadOnly();
        }

        public void AddControl(Control c)
        {
            Tab tab = c as Tab;
            if (tab == null)
                throw new ArgumentException("\"c\" needs to be a Tab");

            ScrollView scroll = new ScrollView();

            c.Window = Window;
            controls.Add(c);

            scroll.Content = c;

            Add(scroll, "tab");
            tab.RealTab = Tabs.First(t => ReferenceEquals(((ScrollView)t.Child).Content, tab));
        }

        public void PerformLayout(double width)
        {
            //SetChildBounds(notebook, new Rectangle(0, 25, width, 400));

            controls.Do(c => c.PerformLayout(width - 8));
        }
    }
}
