using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class PluginImageButton : ImageButton
    {
        private PluginType pluginType;

        public PluginType PluginType
        {
            get
            {
                return pluginType;
            }
            set
            {
                if (pluginType != value)
                {
                    pluginType = value;
                    if (Image == null)
                        QueueDraw();
                }
            }
        }

        private bool showInstall;

        public bool ShowInstall
        {
            get
            {
                return showInstall;
            }
            set
            {
                if (showInstall != value)
                {
                    showInstall = value;
                    lbl.Visible = value;
                    QueueDraw();
                }
            }
        }

        private bool installed;

        public bool Installed
        {
            get
            {
                return installed;
            }
            set
            {
                if (installed != value)
                {
                    installed = value;
                    lbl.Text = installed ? "uninstall" : "install";
                    QueueDraw();
                }
            }
        }

        Label lbl = new Label() { Text = "install", TextAlignment = Alignment.Center, VerticalPlacement = WidgetPlacement.Center, TextColor = Colors.Black, Visible = false };

        protected override void OnMouseMoved(MouseMovedEventArgs args)
        {
            base.OnMouseMoved(args);

            //if (mOver != args.X > 16)
            //{
            //    if (args.X > 16)
            //    {
            //        mOver = true;
            //        QueueDraw();
            //    }
            //    else
            //    {
            //        mOver = false;
            //        QueueDraw();
            //    }
            //}
        }

        protected override void OnButtonPressed(ButtonEventArgs args)
        {
            if (args.X > 16)
                base.OnButtonPressed(args);

            if (args.Button == PointerButton.Left)
            {
                mDown = true;
                QueueDraw();
            }
        }

        protected override void OnButtonReleased(ButtonEventArgs args)
        {
            if (args.X > 16)
                base.OnButtonReleased(args);

            if (args.Button == PointerButton.Left)
            {
                mDown = false;
                QueueDraw();
            }
        }

        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);
            if (ShowInstall)
            {
                ctx.Rectangle(dirtyRect);
                ctx.SetColor(Colors.White.WithAlpha(.8));
                ctx.Fill();
            }
        }

        public PluginImageButton()
            : base(80, 40)
        {
            AddChild(lbl, new Rectangle(0, 0, 80, 40));
        }
    }
}
