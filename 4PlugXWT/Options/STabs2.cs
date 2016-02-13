using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Tab = System.Tuple<FPlug.Options.STabPage, Xwt.Button>;

namespace FPlug.Options
{
    //[ScriptClass("Tabs2")]
    public class STabs2 : SContainer
    {
        List<Tab> tabs = new List<Tab>();
        Tab lastTab = null;

        public STabs2()
        {
            Height = 500;
        }

        protected override void OnAddChild(SChild child)
        {
            child.Visible = false;
            tabs.Add(new Tab((STabPage)child, new Button("Tab")));
            AddChild(tabs[tabs.Count - 1].Item2, 0, 0);
            tabs[tabs.Count - 1].Item2.Clicked += (s, e) =>
            {
                if (lastTab != null)
                    lastTab.Item1.Visible = false;
                var tab = tabs[tabs.FindIndex((t) => t.Item2.Equals(s))];
                tab.Item1.Visible = true;
                AddChild(tab.Item1, 0, 32);
                lastTab = tab;
            };
        }

        protected override void OnMoveChild(SChild child, int index)
        {

        }

        public override string CanAddChild(SChild child)
        {
            return child is STabPage ? null : "Children of a tab control can only be tabpages.";
        }

        public override void Layout(double width)
        {
            base.Layout(width);

            double x = 0;
            double y = 0;

            foreach (var t in tabs)
            {
                SetChildBounds(t.Item2, new Rectangle(x, y, 128, 24));
                x += 128;
            }

            double maxHeight = 32;
            foreach (var t in tabs)
            {
                t.Item1.Layout(width - 10);
                maxHeight = Math.Max(maxHeight, t.Item1.Height);
            }
            
            Height = 32 + maxHeight + 30;
            
            //SetChildBounds(tabBox, new Rectangle(0, 0, Width, -1));
        }

        protected override void OnRemoveChild(SChild child)
        {
            
        }
    }
}
