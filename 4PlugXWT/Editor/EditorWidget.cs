using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Editor
{
    public class EditorWidget : Canvas
    {
        static Color[] colors = new Color[] { Colors.Black, Colors.Blue, Colors.Blue, Colors.CadetBlue };

        public TextSource Source = new TextSource();

        Font font = Font.FromName("Consolas 12");
        HScrollbar HScroll;
        VScrollbar VScroll;

        public EditorWidget()
        {
            CanGetFocus = true;

            //Source.InsertText(String.Join("\n", Enumerable.Range('a', 26).Select(i => new String((char)i, 40))), 0, 0);
            Source.InsertText(System.IO.File.ReadAllText(@"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2\tf\cfg\script.4cfg"));
            Source.TextChanged += (s, e) => { QueueDraw(); };

            HScroll = new HScrollbar();
            AddChild(HScroll);
            VScroll = new VScrollbar();
            AddChild(VScroll);
            VScroll.ValueChanged += (s, e) =>
            {
                Source.ViewOffset = new PointD { X = Source.ViewOffset.X, Y = (int)(VScroll.Value) * CharHeight };
            };

            Source.CaretPosition = new Loc(10, 10);
            Source.Selection = new Range(5, 5, 10, 10, SelectionType.Rectangular);
        }

        protected override void OnBoundsChanged()
        {
            SetChildBounds(HScroll, new Rectangle(0, Size.Height - HScroll.Size.Height, Size.Width - VScroll.Size.Width, HScroll.Size.Height));
            SetChildBounds(VScroll, new Rectangle(Size.Width - VScroll.Size.Width, 0, VScroll.Size.Width, Size.Height - HScroll.Size.Height));
            base.OnBoundsChanged();
        }

        void updateScrollBars()
        {
            VScroll.LowerValue = 0;
            VScroll.UpperValue = Source.Count - 1;
            VScroll.PageSize = Size.Height / CharHeight;
            VScroll.Value = Source.ViewOffset.Y / CharHeight;
            //HScroll.Visible = false;
        }

        public double CharWidth { get; private set; }
        public double CharHeight { get; private set; }

        public double TextXOffset { get; private set; }

        protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
        {
            var w = System.Diagnostics.Stopwatch.StartNew();
            base.OnDraw(ctx, dirtyRect);

            // char size
            var charSize = new TextLayout(this) { Text = "X", Font = font }.GetSize();
            double charWidth = CharWidth = charSize.Width;
            double charHeight = CharHeight = charSize.Height;

            // scroll bars
            updateScrollBars();

            // calculations
            int start = Math.Max(0, (int)(Source.ViewOffset.Y / charHeight));
            int end = Math.Min(Source.Count, (int)((Source.ViewOffset.Y + Size.Height) / charHeight));

            double lineNrWidth = ((int)Math.Log10(Source.Count) + 2) * CharWidth;
            TextXOffset = lineNrWidth - Source.ViewOffset.X;

            // selection
            Range selection = Source.Selection;
            bool hasSelection = !selection.IsEmpty;
            bool isRectSelection = selection.Type == SelectionType.Rectangular;

            // text
            for (int i = start; i < end; i++)
            {
                Line line = Source[i];
                string text = line.GetString();

                if (hasSelection)
                {
                    if (i >= selection.Start.Y && i <= selection.End.Y)
                    {
                        ctx.SetColor(Colors.LightSkyBlue);
                        if (isRectSelection)
                        {
                            ctx.Rectangle(selection.Start.X * charWidth + lineNrWidth - Source.ViewOffset.X,
                                (i * charHeight) - Source.ViewOffset.Y,
                                Math.Abs(selection.End.X - selection.Start.X) * charWidth,
                                charHeight);
                            ctx.Fill();
                        }
                        else
                        {
                            if (selection.Start.Y == selection.End.Y)
                            {
                                ctx.Rectangle(selection.Start.X * charWidth + TextXOffset, (i * charHeight) - Source.ViewOffset.Y, (selection.End.X - selection.Start.X) * charWidth + 2, charHeight);
                            }
                            else
                            {
                                if (i == selection.Start.Y)
                                    ctx.Rectangle(selection.Start.X * charWidth + TextXOffset, (i * charHeight) - Source.ViewOffset.Y, (Source[i].Count - selection.Start.X) * charWidth + 2, charHeight);
                                else if (i == selection.End.Y)
                                    ctx.Rectangle(TextXOffset, (i * charHeight) - Source.ViewOffset.Y, selection.End.X * charWidth + 2, charHeight);
                                else
                                    ctx.Rectangle(TextXOffset, (i * charHeight) - Source.ViewOffset.Y, Source[i].Count * charWidth + 2, charHeight);
                                ctx.Fill();
                            }
                        }
                    }
                }

                //ctx.SetColor(Colors.Black);
                //var layout = new TextLayout(this)
                //{
                //    Text = text,
                //    Font = font,
                //};
                //ctx.DrawTextLayout(layout, -Source.ViewOffset.X + lineNrWidth, (i * charHeight) - Source.ViewOffset.Y);
            }

            for (int ic = 0; ic < colors.Length; ic++)
            {
                ctx.SetColor(colors[ic]);
                CharpStyle style = (CharpStyle)ic;
                StringBuilder builder = new StringBuilder(1024);
                bool draw = false;

                for (int y = start; y < end; y++)
                {
                    Line line = Source[y];
                    for (int x = 0; x < line.Count; x++)
                    {
                        char c = line[x].Char;
                        if (c != ' ' && (line[x].Style & CharpStyle.ColorAll) == style)
                        {
                            if (c == '\t')
                                builder.Append(' ');
                            else
                            {
                                builder.Append(line[x].Char);
                                draw = true;
                            }
                        }
                        else
                            builder.Append(' ');
                    }
                    builder.AppendLine(" ");
                }

                TextLayout layout = new TextLayout(this)
                {
                    Text = builder.ToString(),
                    Font = font,
                };
                if (draw)
                    ctx.DrawTextLayout(layout, TextXOffset, 0);
            }

            // line left
            ctx.SetColor(Colors.LightGray);
            ctx.Rectangle(0, 0, lineNrWidth, Size.Height);
            ctx.Fill();

            // line numbers, markers
            for (int i = start; i < end; i++)
            {
                ctx.SetColor(Colors.Black);
                ctx.DrawTextLayout(new TextLayout(this) { Text = (i + 1).ToString(), Font = font }, 0, (i * charHeight) - Source.ViewOffset.Y);
                if ((Source[i].LineStyle & LineStyle.Unsaved) == LineStyle.Unsaved)
                {
                    ctx.SetColor(Colors.Red);
                    ctx.Rectangle(TextXOffset - charWidth / 2 - 1, (i * charHeight) - Source.ViewOffset.Y, 2, charHeight);
                    ctx.Fill();
                }
            }

            // caret
            Loc caret = Source.GetSanitizedLoc(Source.CaretPosition);
            ctx.Rectangle((int)(charWidth * caret.X - Source.ViewOffset.X + lineNrWidth), (int)(charHeight * caret.Y - Source.ViewOffset.Y), 2, charHeight);
            ctx.SetColor(Colors.Red);
            ctx.Fill();

            // stopwatch
            w.Stop();
            //Console.WriteLine("Drawing: " + w.Elapsed.TotalMilliseconds);
        }

        protected override void OnPreviewTextInput(PreviewTextInputEventArgs e)
        {
            e.Handled = true;

            if (e.Text != "\b")
                Source.InsertText(e.Text);
        }

        protected override void OnKeyPressed(KeyEventArgs e)
        {
            base.OnKeyReleased(e);

            switch (e.Key)
            {
                case Key.Left:
                    {
                        Source.MoveCaret(Direction.Left, (e.Modifiers & ModifierKeys.Control) == ModifierKeys.Control, (e.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift);
                    }
                    break;
                case Key.Right:
                    {
                        Source.MoveCaret(Direction.Right, (e.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                    }
                    break;
                default:
                    if (e.Modifiers == ModifierKeys.None)
                    {
                        switch (e.Key)
                        {
                            case Key.Return:
                                {
                                    Source.InsertText("\n\n");
                                }
                                break;
                            case Key.Up:
                                {
                                    Source.MoveCaret(Direction.Up);
                                }
                                break;
                            case Key.Down:
                                {
                                    Source.MoveCaret(Direction.Down);
                                }
                                break;
                            case Key.BackSpace:
                                {
                                    Source.RemoveCaretLeft((e.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                                }
                                break;
                            case Key.Delete:
                                {
                                    Source.RemoveCaretRight((e.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                                }
                                break;
                        }
                    }
                    else if (e.Modifiers == ModifierKeys.Control)
                    {
                        switch (e.Key)
                        {
                            case Key.BackSpace:
                                {
                                    Source.RemoveCaretLeft((e.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                                }
                                break;
                            case Key.Delete:
                                {
                                    Source.RemoveCaretRight((e.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
