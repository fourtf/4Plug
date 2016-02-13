using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug
{
    public class PluginTitleWidget : Canvas, IHasPluginType, IHasCanvasBackend
    {
        public PluginType PluginType { get; set; }

        Label label;

        public PluginTitleWidget(PluginType type)
        {
            PluginType = type;

            label = new Label()
            {
                Text = type.GetName(),
                TextColor = type.GetColor(),
                Font = Font.WithScaledSize(2d),
                HeightRequest = 30,
                WidthRequest = 400,
            };

            MarginTop = 8;

            HeightRequest = 30;
            WidthRequest = 400;

            AddChild(label, 0, 0);

            if (App.InitOpacityAnimation != null)
            {
                Opacity = 0;
                App.InitOpacityAnimation(this);
            }
        }

        public Xwt.Backends.ICanvasBackend GetBackend()
        {
            return (Xwt.Backends.ICanvasBackend)BackendHost.Backend;
        }
    }
}
