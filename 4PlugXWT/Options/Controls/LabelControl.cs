using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Options.Controls
{
    public class LabelControl : Control
    {
        [ScriptMember("text")]
        public string Text { get; set; }

        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            ctx.DrawTextLayout(new TextLayout() { Text = Text }, 0, 0);
        }
    }
}
