using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Options.Controls
{
    public class EmptyControl : Control
    {
        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            if (Window?.DrawRedDebugOutline ?? false)
            {
                ctx.SetColor(Colors.Blue);
                ctx.Rectangle(0, 0, Bounds.Width, Bounds.Height);
                ctx.Stroke();
                ctx.SetColor(Colors.Black);
            }
        }
    }
}
