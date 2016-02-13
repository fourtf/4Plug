using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class ImageButton : Canvas
    {
        public event EventHandler Click;

        private Image image;

        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                QueueDraw();
            }
        }

        protected bool mOver = false;
        protected bool mDown = false;

        static Color mOverColor = new Color(1, 1, 1, .3d);
        static Color bg = new Color(1, 1, 1, .5d);

        public ImageButton(double width = 16, double height = 16, Image image = null)
        {
            HeightRequest = height;
            WidthRequest = width;
            Cursor = CursorType.Hand;
            this.image = image;
        }

        protected override void OnMouseEntered(EventArgs args)
        {
            base.OnMouseEntered(args);

            mOver = true;
            QueueDraw();
        }

        protected override void OnMouseExited(EventArgs args)
        {
            base.OnMouseExited(args);

            mOver = false;
            QueueDraw();
        }

        protected override void OnButtonPressed(ButtonEventArgs args)
        {
            base.OnButtonPressed(args);

            if (args.Button == PointerButton.Left)
                mDown = true;

            QueueDraw();
        }

        protected override void OnButtonReleased(ButtonEventArgs args)
        {
            base.OnButtonReleased(args);

            if (args.Button == PointerButton.Left)
            {
                if (Click != null)
                    Click(this, EventArgs.Empty);

                mDown = false;
                QueueDraw();
            }
        }

        protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
        {
            if (!Sensitive)
            {
                ctx.GlobalAlpha = .5d;
            }
            if (image == null)
            {
                ctx.SetColor(bg);
                ctx.Rectangle(dirtyRect);
                ctx.Fill();
            }
            else
                ctx.DrawImage(image, new Rectangle(0, 0, WidthRequest, HeightRequest));

            
            if (mOver && Sensitive)
            {
                ctx.SetColor(mOverColor);
                ctx.Rectangle(dirtyRect);
                ctx.Fill();
            }

            if (mDown)
            {
                ctx.SetColor(mOverColor);
                ctx.Rectangle(dirtyRect);
                ctx.Fill();
            }

            //ctx.SetColor(Colors.Red);
            //ctx.Rectangle(0, 0, WidthRequest, HeightRequest);
            //ctx.Stroke();
        }
    }

    public class ImageLinkButton : ImageButton
    {
        public string Url { get; set; }

        public ImageLinkButton()
            : base()
        {
            Click += ImageLinkButton_Click;
        }

        void ImageLinkButton_Click(object sender, EventArgs e)
        {
            Desktop.OpenUrl(Url);
        }
    }
}
