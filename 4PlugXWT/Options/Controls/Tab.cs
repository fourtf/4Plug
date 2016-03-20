using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options.Controls
{
    public class Tab : Container
    {
        private string title = "tab";

        [ScriptMember("title")]
        public string Title
        {
            get { return title; }
            set { title = value; if (RealTab != null) RealTab.Label = value; }
        }

        public NotebookTab RealTab { get; set; }

        public Tab()
        {
            Padding = new WidgetSpacing();
        }

        public override void PerformLayout(double width)
        {
            base.PerformLayout(width);

            HeightRequest = Height;
            MinHeight = Height;
        }
    }
}
