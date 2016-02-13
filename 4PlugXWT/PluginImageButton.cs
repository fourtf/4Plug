using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug
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

        public PluginImageButton()
            : base(80, 40)
        {

        }
    }
}
