using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class ColorPicker : Canvas
    {
        static Image resetImage = Resources.GetImage("refresh.png");
        static Image checkerboard = Resources.GetImage("transparent_graphic.png");

        private Color color = Colors.White;

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                fColor = true;
                if (color != value)
                {
                    color = value;
                    QueueDraw();
                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler ColorChanged;

        bool fColor = false;

        private Color defaultColor = Colors.White;

        public Color DefaultColor
        {
            get
            {
                return defaultColor;
            }
            set
            {
                if (!fColor)
                    Color = value;

                if (defaultColor != value)
                {
                    defaultColor = value;
                    QueueDraw();
                }
            }
        }

        public bool IsDefaultColor
        {
            get
            {
                return color == defaultColor;
            }
        }

        public ColorPicker()
        {
            Cursor = CursorType.Hand;
        }

        bool mover = false;
        double mx = 0;

        protected override void OnMouseEntered(EventArgs args)
        {
            base.OnMouseEntered(args);
            mover = true;
            QueueDraw();
        }

        protected override void OnMouseExited(EventArgs args)
        {
            base.OnMouseExited(args);
            mover = false;
            QueueDraw();
        }

        protected override void OnMouseMoved(MouseMovedEventArgs args)
        {
            base.OnMouseMoved(args);
            mx = args.X;
            QueueDraw();
        }

        protected override void OnButtonReleased(ButtonEventArgs args)
        {
            base.OnButtonReleased(args);

            if (args.Button == PointerButton.Left)
            {
                if (!IsDefaultColor && mx > Size.Width - 20)
                {
                    Color = DefaultColor;
                }
                else
                {
                    ColorPopover popover = new ColorPopover(this);
                    popover.SetColor(Color, null);
                    popover.Show(Popover.Position.Bottom, this);

                    //SelectColorDialog dialog = new SelectColorDialog();
                    //dialog.SupportsAlpha = true;
                    //dialog.Color = color;
                    //if (dialog.Run())
                    //    Color = dialog.Color;
                }
            }
        }

        protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            for (int i = 0; i < Size.Width / checkerboard.Width; i++)
            {
                for (int j = 0; j < Size.Height / checkerboard.Height; j++)
                {
                    ctx.DrawImage(checkerboard, i * checkerboard.Width, j * checkerboard.Height); 
                }
            }

            ctx.SetLineWidth(1);

            ctx.SetColor(color);
            ctx.Rectangle(0, 0, Size.Width - 20, Size.Height);
            ctx.Fill();

            ctx.SetColor(DefaultColor);
            ctx.Rectangle(Size.Width - 20, 0, 20, Size.Height);
            ctx.Fill();

            ctx.SetColor(Colors.Black);
            ctx.Rectangle(0, 0, Size.Width, Size.Height);
            ctx.Stroke();

            //ctx.SetColor(Color.Brightness > .5 ? Colors.White : Colors.Black);
            ctx.SetColor(((-1 * Color.Brightness) + 1) * 1.15 + (Color.Alpha) > 1.2 ? Colors.White : Colors.Black);
            ctx.DrawTextLayout(new TextLayout(this) { Text = Color.ToHexString().ToUpper() }, 2, 3);

            if (mover && (IsDefaultColor || mx <= Size.Width - 20))
            {
                ctx.SetColor(new Color(1, 1, 1, .33));
                ctx.Rectangle(0, 0, Size.Width - (IsDefaultColor ? 0 : 20), Size.Height);
                ctx.Fill();
            }

            if (!IsDefaultColor)
            {
                if (mover && mx > Size.Width - 20)
                {
                    ctx.SetColor(new Color(1, 1, 1, .33));
                    ctx.Rectangle(Size.Width - 20, 0, 20, Size.Height);
                    ctx.Fill();
                }

                ctx.SetColor(Colors.Black);
                ctx.MoveTo(Size.Width - 19.5, 0);
                ctx.RelLineTo(0, Size.Height);
                ctx.Stroke();

                if (mover)
                    ctx.DrawImage(resetImage, Size.Width - 18, (Size.Height - 16) / 2, 16, 16);
            }
        }
    }

    public class ColorPopover : Popover
    {
        ColorPicker picker;

        public Color Color { get; private set; }

        bool enableEvents = true;

        public void SetColor(Color c, Widget unaffectedWidget)
        {
            if (c != Color)
            {
                enableEvents = false;
                if (unaffectedWidget != rSlider)
                    rSlider.Value = (byte)(c.Red * 255);
                if (unaffectedWidget != gSlider)
                    gSlider.Value = (byte)(c.Green * 255);
                if (unaffectedWidget != bSlider)
                    bSlider.Value = (byte)(c.Blue * 255);
                if (unaffectedWidget != aSlider)
                    aSlider.Value = (byte)(c.Alpha * 255);

                if (unaffectedWidget != rText)
                    rText.Text = ((byte)(c.Red * 255)).ToString();
                if (unaffectedWidget != gText)
                    gText.Text = ((byte)(c.Green * 255)).ToString();
                if (unaffectedWidget != bText)
                    bText.Text = ((byte)(c.Blue * 255)).ToString();
                if (unaffectedWidget != aText)
                    aText.Text = ((byte)(c.Alpha * 255)).ToString();

                if (unaffectedWidget != hexText)
                    hexText.Text = c.ToHexString().ToUpper();
                if (unaffectedWidget != sourceText)
                    sourceText.Text = ((byte)(c.Red * 255)) + " " + ((byte)(c.Green * 255)) + " " + ((byte)(c.Blue * 255)) + (c.Alpha == 1 ? "" : " " + ((byte)(c.Alpha * 255)));

                picker.Color = c;

                enableEvents = true;
            }
        }

        public Color OriginalColor { get; private set; }

        Slider rSlider, gSlider, bSlider, aSlider;
        TextEntry rText, gText, bText, aText;
        TextEntry sourceText, hexText;

        public ColorPopover(ColorPicker picker)
        {
            this.picker = picker;

            OriginalColor = picker.Color;

            var h = new HBox();
            var leftBox = new VBox();
            h.PackStart(leftBox);

            Content = h;

            {
                HBox box = new HBox();
                box.PackStart(new Label("R") { WidthRequest = 50 });
                box.PackStart(rSlider = new HSlider() { WidthRequest = 100, MinimumValue = 0, MaximumValue = 255 });
                box.PackStart(rText = new TextEntry() { WidthRequest = 50 });
                leftBox.PackStart(box);
            }
            {
                HBox box = new HBox();
                box.PackStart(new Label("G") { WidthRequest = 50 });
                box.PackStart(gSlider = new HSlider() { WidthRequest = 100, MinimumValue = 0, MaximumValue = 255 });
                box.PackStart(gText = new TextEntry() { WidthRequest = 50 });
                leftBox.PackStart(box);
            }
            {
                HBox box = new HBox();
                box.PackStart(new Label("B") { WidthRequest = 50 });
                box.PackStart(bSlider = new HSlider() { WidthRequest = 100, MinimumValue = 0, MaximumValue = 255 });
                box.PackStart(bText = new TextEntry() { WidthRequest = 50 });
                leftBox.PackStart(box);
            }
            {
                HBox box = new HBox();
                box.PackStart(new Label("Alpha") { WidthRequest = 50 });
                box.PackStart(aSlider = new HSlider() { WidthRequest = 100, MinimumValue = 0, MaximumValue = 255 });
                box.PackStart(aText = new TextEntry() { WidthRequest = 50 });
                leftBox.PackStart(box);
            }

            EventHandler setSlider = (s, e) => { if (enableEvents) SetColor(Color.FromBytes((byte)rSlider.Value, (byte)gSlider.Value, (byte)bSlider.Value, (byte)aSlider.Value), (Widget)s); };

            rSlider.ValueChanged += setSlider;
            gSlider.ValueChanged += setSlider;
            bSlider.ValueChanged += setSlider;
            aSlider.ValueChanged += setSlider;

            rText.Changed += (s, e) => { if (enableEvents) { byte i; if (byte.TryParse(rText.Text, out i)) SetColor(Color.FromBytes(i, (byte)gSlider.Value, (byte)bSlider.Value, (byte)aSlider.Value), rText); } };
            gText.Changed += (s, e) => { if (enableEvents) { byte i; if (byte.TryParse(gText.Text, out i)) SetColor(Color.FromBytes((byte)rSlider.Value, i, (byte)bSlider.Value, (byte)aSlider.Value), gText); } };
            bText.Changed += (s, e) => { if (enableEvents) { byte i; if (byte.TryParse(bText.Text, out i)) SetColor(Color.FromBytes((byte)rSlider.Value, (byte)gSlider.Value, i, (byte)aSlider.Value), bText); } };
            aText.Changed += (s, e) => { if (enableEvents) { byte i; if (byte.TryParse(aText.Text, out i)) SetColor(Color.FromBytes((byte)rSlider.Value, (byte)gSlider.Value, (byte)bSlider.Value, i), aText); } };

            {
                HBox box = new HBox();
                TextEntry text;
                hexText = text = new TextEntry();
                text.Changed += (s, e) =>
                {
                    if (enableEvents)
                    {
                        Color c;
                        if (App.TryParseColor(hexText.Text, out c))
                            SetColor(c, hexText);
                    }
                };
                box.PackStart(text);

                sourceText = text = new TextEntry();
                text.Changed += (s, e) =>
                {
                    if (enableEvents)
                    {
                        Color c;
                        if (App.TryParseColor(sourceText.Text, out c))
                            SetColor(c, sourceText);
                    }
                };
                box.PackStart(text);
                leftBox.PackStart(box);
            }

            {
                HBox box = new HBox();
                Button btn;
                btn = new Button(" Reset ");
                btn.Clicked += (s, e) => { picker.Color = picker.DefaultColor; };
                box.PackEnd(btn);

                btn = new Button(" Discard Changes ");
                btn.Clicked += (s, e) => { picker.Color = OriginalColor; };
                box.PackEnd(btn);
                leftBox.PackStart(box);
            }
        }
    }
}
