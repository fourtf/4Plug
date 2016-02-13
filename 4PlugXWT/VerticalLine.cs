using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug
{
    public class VerticalLine : Canvas
    {
        protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            ctx.SetColor(Colors.DarkGray);
            ctx.SetLineWidth(1);
            ctx.MoveTo(0, 0);
            ctx.LineTo(0, Bounds.Height);
            ctx.Stroke();
        }
    }
}
