using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Backends;

namespace FPlug.Widgets
{
    public class PluginLayoutWidget : Canvas
    {
        public static readonly Size DefaultSize = new Size(330, 46);
        public static readonly WidgetSpacing DefaultMargin = new WidgetSpacing(18, 12, 18);

        public bool HasBorder { get; set; }
        public bool HasBorderLineDash { get; set; }

        public virtual PluginType PluginType { get; set; }

        public IWidgetBackend GetBackend()
        {
            return BackendHost.Backend;
        }

        public PluginLayoutWidget()
        {
            this.WidthRequest = DefaultSize.Width;
            this.HeightRequest = DefaultSize.Height;

            this.Margin = DefaultMargin;

            if (App.InitDropShadow != null)
                App.InitDropShadow(BackendHost.Backend);

            if (App.AnimateOpacityIn != null)
            {
                Opacity = 0;
                App.AnimateOpacityIn(BackendHost.Backend);
            }

            if (App.InitMoveWidget != null)
                App.InitMoveWidget(BackendHost.Backend);
        }

        public virtual bool CanClose()
        {
            return true;
        }

        protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            if (HasBorder)
            {
                ctx.SetColor(PluginType.GetColor());
                ctx.SetLineWidth(1);

                if (HasBorderLineDash)
                    ctx.SetLineDash(0, 1, 3);
                    //ctx.SetLineDash(0, 4d, 4d);

                ctx.Rectangle(0, 0, WidthRequest, HeightRequest);
                ctx.Stroke();
            }
        }
    }
}
