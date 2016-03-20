using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Options.Controls
{
    public class GroupContainer : Container
    {
        private string title = "";

        [ScriptMember("title")]
        public string Title
        {
            get { return title; }
            set { title = value; QueueDraw(); }
        }

        public GroupContainer()
        {
            Padding = new WidgetSpacing(8, 18, 8, 10);
        }

        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            ctx.SetColor(Colors.Gray);
            ctx.SetLineWidth(1);

            ctx.RoundRectangle(0, Padding.Top / 2 - .5, Width - 4.5, Height - Padding.Bottom * 1.5, 3);
            ctx.Stroke();

            var tl = new TextLayout(this) { Text = title, Width = Width - Padding.Left - Padding.Right / 2 };

            ctx.SetColor(Colors.White);
            ctx.Rectangle(Padding.Left, 0, tl.GetSize().Width, Padding.Top);
            ctx.Fill();

            ctx.SetColor(Colors.Black);
            ctx.DrawTextLayout(tl, Padding.Left, 0);
        }
    }
}
