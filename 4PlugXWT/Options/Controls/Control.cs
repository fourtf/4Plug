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
        public string ID { get; set; }

        private double widthPercent = 0;

        [ScriptMember("width")]
        public double WidthPercent
        {
            get { return widthPercent; }
            set { widthPercent = Math.Max(Math.Min(value, 1), 0); }
        }

        public SettingsWindow Window { get; set; }

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

            ctx.SetColor(Colors.Red);
            
            ctx.Rectangle(0, 0, Bounds.Width, Bounds.Height);
            
            ctx.Stroke();
        }

        public virtual void PerformLayout(double width)
        {
            Width = width;
        }
    }
}
