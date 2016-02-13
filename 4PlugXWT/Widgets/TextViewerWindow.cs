using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Formats;

namespace FPlug.Widgets
{
    public class TextViewerWindow : Window
    {
        public TextViewerWindow(string path)
        {
            Icon = App.Icon;

            Title = path + " (ESC to close)";

            bool isMd = path.ToUpper().EndsWith(".MD");

            var rtv = new RichTextView();
            var scroll = new ScrollView(rtv);
            Content = scroll;

            try
            {
                rtv.LoadFile(path, isMd ? TextFormat.Markdown : ProperPlainTextFormat.Proper);
            }
            catch { }

            rtv.KeyPressed += (s, e) => { if (e.Key == Key.Escape) Close(); };

            Width = 800;
            Height = 550;
        }
    }
}
