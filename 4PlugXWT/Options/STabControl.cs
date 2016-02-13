using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options
{
    [ScriptClass("Tabs")]
    public class STabControl : SContainer
    {
        Notebook tabControl = new Notebook();
        //List<NotebookTab> tabPages = new List<NotebookTab>();

        public STabControl()
        {
            AddChild(tabControl, 0, 0);
            Height = 500;
        }

        public override void Clear()
        {
            tabControl.Tabs.Clear();
        }

        protected override void OnAddChild(SChild child)
        {
            ((STabPage)child).TitleChanged += STabControl_TitleChanged;
            tabControl.Add(child, ((STabPage)child).Text);
        }

        void STabControl_TitleChanged(object sender, EventArgs e)
        {
            tabControl.Tabs.First((tab) => tab.Child == sender).Label = ((STabPage)sender).Text;
        }

        protected override void OnRemoveChild(SChild child)
        {
            ((STabPage)child).TitleChanged -= STabControl_TitleChanged;

            //foreach (TabPage page in tabPages)
            //{
            //    if (page.Content == (Control)child)
            //    {
            //        tabPages.Remove(page);
            //        tabControl.Pages.Remove(page);
            //        break;
            //    }
            //}
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

            double maxHeight = 32;
            foreach (NotebookTab t in tabControl.Tabs)
            {
                ((STabPage)t.Child).Layout(width - 10);
                maxHeight = Math.Max(maxHeight, ((STabPage)t.Child).Height);
            }

            Height = maxHeight + 30;

            SetChildBounds(tabControl, new Rectangle(0, 0, Width, Height));
        }
    }
}
