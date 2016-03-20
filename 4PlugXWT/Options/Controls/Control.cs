using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Options.Controls
{
    public partial class Control : Canvas, IControl
    {
        [ScriptMember("id")]
        public string ID { get; set; } = null;

        private double widthPercent = 0;

        [ScriptMember("width")]
        public double WidthPercent
        {
            get { return widthPercent; }
            set { widthPercent = Math.Max(Math.Min(value, 1), 0); }
        }

        private SettingsWindow window;

        public SettingsWindow Window
        {
            get { return window; }
            set { window = value; OnWindowSet(); }
        }

        public double Height { get; set; } = 25;
        public double Width { get; set; } = 100;

        //  ctor
        public Control()
        {
            WidthRequest = 200;
            HeightRequest = 30;
        }

        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            if (Window?.DrawRedDebugOutline ?? false)
            {
                ctx.SetColor(Colors.Red);
                ctx.Rectangle(0, 0, Bounds.Width, Bounds.Height);
                ctx.Stroke();
                ctx.SetColor(Colors.Black);
            }
        }

        public virtual void PerformLayout(double width)
        {
            Width = width;
        }

        protected virtual void OnWindowSet()
        {

        }
    }
}
