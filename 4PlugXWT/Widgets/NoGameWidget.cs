using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class NoGameWidget : PluginLayoutWidget
    {
        static Image imgError = Resources.GetImage("error.png");

        public NoGameWidget()
        {
            HasBorder = false;

            ImageView view = new ImageView(imgError);

            AddChild(view, 15, 15);

            AddChild(new Label("No games where found"), 46, 15);
            var link = new Button(" Select Games ");
            link.Clicked += (s, e) =>
            {
                App.MainWindow.ShowGameSelector();
            };

            AddChild(link, WidthRequest - 115, 12);
        }
    }
}
