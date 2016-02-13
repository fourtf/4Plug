using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Widgets
{
    public class PluginTitleWidget : PluginLayoutWidget
    {
        Label label;

        public PluginTitleWidget(PluginType type)
        {
            PluginType = type;

            label = new Label()
            {
                Text = type.GetName(),
                TextColor = type.GetColor(),
                Font = Xwt.Drawing.Font.FromName("Segoe UI Light 30"), // Font.WithScaledSize(2d),
                HeightRequest = 40,
                WidthRequest = 400,
            };

            MarginTop = 8;

            MarginBottom = -8;

            HeightRequest = 40;
            WidthRequest = 400;

            AddChild(label, 0, 0);
        }
    }
}
