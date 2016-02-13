using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug
{
    public class AddPluginWidget : Canvas, IHasPluginType, IHasCanvasBackend
    {
        public PluginType PluginType
        {
            get;
            set;
        }

        public AddPluginWidget(PluginType type)
        {
            PluginType = type;
            HeightRequest = PluginWidget.Size.Height;
            WidthRequest = PluginWidget.Size.Width;

            Margin = PluginWidget.Margin;

            Cursor = CursorType.Hand;

            Label l = new Label("Add new " + type.GetName());
            //l.TextColor = type.GetColor();

            l.TextAlignment = Alignment.Center;

            AddChild(l, new Rectangle(0, 0, PluginWidget.Size.Width, PluginWidget.Size.Height));

            BackgroundColor = type.GetBGColor();

            if (App.InitDropShadow != null)
                App.InitDropShadow(this);

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

        protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            //ctx.SetColor(PluginType.GetColor());
            ctx.SetLineDash(0, 4d, 4d);
            ctx.Rectangle(0, 0, PluginWidget.Size.Width, PluginWidget.Size.Height);
            ctx.Stroke();
        }
    }
}
