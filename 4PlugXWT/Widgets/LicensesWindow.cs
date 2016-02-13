using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class LicensesWindow : Window
    {
        public LicensesWindow()
        {
            Icon = App.Icon;

            var vbox = new VBox();
            var scroll = new ScrollView(vbox);
            Content = scroll;

            Width = 600;
            Height = 500;

            Title = "Licenses";

            Font labelFont;
            RichTextView rtv;

            // XWT FRAMEWORK
            var lbl = new LinkLabel() { Text = "Xwt Framework", Uri = new Uri("https://github.com/mono/xwt/") };
            labelFont = lbl.Font = lbl.Font.WithScaledSize(2);
            vbox.PackStart(lbl);
            vbox.PackStart(rtv = new RichTextView() { MarginLeft = 8 });
            rtv.LoadText(@"The MIT License (MIT)

Copyright (c) 2014 Xamarin, Inc

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the ""Software""), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.", ProperPlainTextFormat.Proper);

            //vbox.PackStart(new LinkLabel() { Font = labelFont, Text = "Splashy Icons", Uri = new Uri("http://splashyfish.com/icons/") });
            //vbox.PackStart(rtv = new RichTextView() { MarginLeft = 8 });
            //rtv.LoadText(@"It's free as in free.", ProperPlainTextFormat.Proper);

//            vbox.PackStart(new LinkLabel() { Font = labelFont, Text = "DefaultIcon", Uri = new Uri("http://www.defaulticon.com/") });
//            vbox.PackStart(rtv = new RichTextView() { MarginLeft = 8 });
//            rtv.LoadText(@"Released under ""Creative Commons Attribution-No Derivative Works 3.0.""
//No changes where made to the images.", ProperPlainTextFormat.Proper);
//
//            vbox.PackStart(new LinkLabel(){ MarginLeft = 14, Text="http://creativecommons.org/licenses/by-nd/3.0/", Uri = new Uri("http://creativecommons.org/licenses/by-nd/3.0/")});
//            vbox.PackStart(new LinkLabel() { MarginLeft = 14, Text = "http://creativecommons.org/licenses/by-nd/3.0/legalcode", Uri = new Uri("http://creativecommons.org/licenses/by-nd/3.0/legalcode") });

            vbox.PackStart(new LinkLabel() { Font = labelFont, Text = "ZipStorer", Uri = new Uri("http://zipstorer.codeplex.com/") });
            vbox.PackStart(rtv = new RichTextView() { MarginLeft = 8 });
            rtv.LoadText(@"The code/library supplied is totally free for either personal or commercial use, with no warranties. 
Please use the ""Powered by ZipStorer"" logo (below) wherever is possible, pointing to this site.", ProperPlainTextFormat.Proper);
            ImageView imgView;
            vbox.PackStart(imgView = new ImageView(Resources.GetImage("zipstorer.png")));
        }
    }
}
