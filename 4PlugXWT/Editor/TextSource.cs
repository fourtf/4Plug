using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public class TextSource : PrimitiveList<Line>
    {
        public event EventHandler TextChanged;

        public void QueueDraw()
        {
            if (TextChanged != null)
                TextChanged(this, EventArgs.Empty);
        }

        private Syntax syntax = null;

        public Syntax Syntax
        {
            get
            {
                return syntax;
            }
            set
            {
                if (syntax != value)
                {
                    syntax = value;
                    value?.TextAdded(All);
                }
            }
        }


        // Constructor
        public TextSource()
        {
            Add(new Line());
            Syntax = new CfgSyntax(this);
        }

        public TextSource(string text)
            : this()
        {
            InsertText(text, 0, 0);
        }


        // Strings
        public static string SanitizeString(string s, int tabPosition = 0)
        {
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c == '\n' || c == '\r')
                {
                    tabPosition = 0;
                }
                else if (c < ' ' || (c >= '\u007f' && c <= '\u009f'))
                {
                    chars[i] = ' ';
                }
                //else if ()
            }
            return new string(chars);
        }


        // Actions
        static Dictionary<TextAction, Action<TextSource>> actions = new Dictionary<TextAction, Action<TextSource>>
        {
            // caret
            [TextAction.MoveCaretLeft] = (s) => s.MoveCaret(Direction.Left),
            [TextAction.MoveCaretLeftControl] = (s) => s.MoveCaret(Direction.Left, true),
            [TextAction.MoveCaretRight] = (s) => s.MoveCaret(Direction.Right),
            [TextAction.MoveCaretRightControl] = (s) => s.MoveCaret(Direction.Right, true),

            [TextAction.MoveCaretUp] = (s) => s.MoveCaret(Direction.Up),
            [TextAction.MoveCaretDown] = (s) => s.MoveCaret(Direction.Down),
        };

        public void ExecuteAction(TextAction action)
        {
            Action<TextSource> item;
            if (actions.TryGetValue(action, out item))
                item?.Invoke(this);
        }


        // Point
        private PointD viewOffset;
        public PointD ViewOffset
        {
            get
            {
                return viewOffset;
            }
            set
            {
                viewOffset = value;
                QueueDraw();
            }
        }


        // Properties
        private NewlineType newLineType;
        public NewlineType NewLineType
        {
            get
            {
                return newLineType;
            }
            set
            {
                if (newLineType != value)
                {
                    newLineType = value;

                }
            }
        }

        public static NewlineType GetNewLineType(string text)
        {
            if (text.IndexOf('\r') == -1 && text.IndexOf('\n') != -1)
                return NewlineType.N;
            return NewlineType.RN;
        }


        // Loc
        private Loc caretPosition = new Loc(0, 0);
        public Loc CaretPosition
        {
            get
            {
                return caretPosition;
            }
            set
            {
                caretPosition = value;
                selection = value.ToSelection();
                QueueDraw();
            }
        }

        private Loc? selectionStart = null;
        public Loc? SelectionStart
        {
            get
            {
                return selectionStart;
            }
            set
            {
                if (selectionStart != value)
                {
                    selectionStart = value;

                }
            }
        }

        public Loc EndLoc
        {
            get
            {
                return new Loc(Count - 1, Last.Count);
            }
        }


        // Selection
        private Range selection;
        public Range Selection
        {
            get
            {
                return selection;
            }
            set
            {
                if (selection != value)
                {
                    selection = value;
                    SelectionStart = value.Start;
                    QueueDraw();
                }
            }
        }

        public Range All
        {
            get { return new Range(Loc.Empty, EndLoc); }
        }

        public Range RangeAtLine(int y)
        {
            return new Range(y, 0, y, arr[y].Count);
        }


        // IO
        private string filename;
        public string Filename
        {
            get
            {
                return filename;
            }
            set
            {
                if (filename != value)
                {
                    filename = value;
                }
            }
        }


        // Markers
        public void RemoveMarkers()
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i].LineStyle = LineStyle.None;
            }
        }


        // Functions
        public static bool IsStoppingChar(char c)
        {
            if (c <= ' ' ||
                (c >= '!' && c <= '/') ||
                (c >= ':' && c <= '@') ||
                (c >= '[' && c <= '`') ||
                (c >= '{' && c <= '~'))
                return true;
            return false;
        }

        public static bool IsWhiteSpace(char c)
        {
            return c <= ' ';
        }

        public Loc GetNextLocLeft(Loc loc, bool control)
        {
            var l = GetSanitizedLoc(loc);
            //if (l != loc)
            //    return l;
            if (control)
            {
                int x = l.X - 2, y = l.Y;
                bool inSpaces = true;

                for (; y >= 0; y--)
                {
                    x = y == l.Y ? x : arr[y].Count - 1;
                    for (; x >= 0; x--)
                    {
                        if (inSpaces && IsWhiteSpace(arr[y][x].Char))
                            continue;
                        inSpaces = false;
                        if (IsStoppingChar(arr[y][x].Char))
                            return new Loc(y, x + 1);
                    }
                    if (!inSpaces)
                        return new Loc(y, 0);
                }
                return Loc.Empty;
            }
            else
            {
                if (l.X == 0)
                {
                    if (l.Y == 0)
                        return Loc.Empty;
                    return new Loc(l.Y - 1, arr[loc.Y - 1].Count);
                }
                else
                    return loc.WithOffset(0, -1);
            }
        }

        public Loc GetNextLocRight(Loc loc, bool control)
        {
            var l = GetSanitizedLoc(loc);
            if (l != loc)
            {
                if (loc.Y == Count - 1)
                    return EndLoc;
                return new Loc(loc.Y + 1, 0);
            }
            if (control)
            {
                int x = l.X + 1, y = l.Y;
                bool inSpaces = false, moved = false;

                for (; y < Count - 1; y++)
                {
                    x = y == l.Y ? x : 0;
                    for (; x < arr[y].Count; x++)
                    {
                        if (!inSpaces && IsWhiteSpace(arr[y][x].Char))
                            inSpaces = true;
                        if (inSpaces && !IsWhiteSpace(arr[y][x].Char))
                            return new Loc(y, x);
                        if (!IsWhiteSpace(arr[y][x].Char) && IsStoppingChar(arr[y][x].Char))
                            return new Loc(y, x);
                        if (!IsWhiteSpace(arr[y][x].Char))
                            moved = true;
                    }
                    if (moved)  //!inSpaces)
                        return new Loc(y, x);
                    inSpaces = true;
                }
                return EndLoc;
            }
            else
            {
                if (l.X >= arr[l.Y].Count)
                {
                    if (l.Y == Count - 1)
                        return EndLoc;
                    return new Loc(l.Y + 1, 0);
                }
                else
                    return loc.WithOffset(0, 1);
            }
        }

        public void MoveCaret(Direction direction, bool control = false, bool shift = false)
        {
            if (shift)
            {
                throw new Exception("shift");
            }
            else
            {
                if (direction == Direction.Left)
                {
                    CaretPosition = GetNextLocLeft(CaretPosition, control);
                }
                else if (direction == Direction.Right)
                {
                    CaretPosition = GetNextLocRight(CaretPosition, control);
                }
                else if (direction == Direction.Down)
                {
                    if (caretPosition.Y < Count - 1)
                        CaretPosition = caretPosition.WithOffset(1, 0);
                }
                else if (direction == Direction.Up)
                {
                    if (caretPosition.Y > 0)
                        CaretPosition = caretPosition.WithOffset(-1, 0);
                }
            }
        }

        public void OffsetCaret(int y, int x)
        {
            if (x == 0 && y == 0) return;
            if (x == 0)
            {
                if (caretPosition.Y + y >= 0 && caretPosition.Y + y < Count)
                    CaretPosition = caretPosition.WithOffset(y, 0);
            }
            else
            {
                CaretPosition = GetSanitizedLoc(caretPosition.WithOffset(y, x));
            }
        }

        public void RemoveCaretLeft(bool control)
        {
            RemoveLeft(caretPosition, control);
        }

        public void RemoveLeft(Loc loc, bool control)
        {
            RemoveRange(new Range(GetNextLocLeft(loc, control), loc));
        }

        public void RemoveCaretRight(bool control)
        {
            RemoveRight(caretPosition, control);
        }

        public void RemoveRight(Loc loc, bool control)
        {
            RemoveRange(new Range(loc, GetNextLocRight(loc, control)));
        }

        public void RemoveSelection()
        {
            RemoveRange(selection);
            Selection = caretPosition.ToSelection();
            selectionStart = null;
        }


        // Style
        public void SetRangeStyle(Range range, CharpStyle style)
        {
            if (range.IsRectangularSelection)
            {
                Loc from = range.Start;
                Loc to = range.End;

                // Rectangular
                int x1 = Math.Min(from.X, to.X);
                int x2 = Math.Max(from.X, to.X);
                if (x1 == x2)
                    return;
                for (int i = from.Y; i <= to.Y; i++)
                {
                    Line l = arr[i];
                    if (l.Count < x1) // out of range
                        continue;
                    else if (l.Count < x2)
                    {
                        for (int x = x1; x < arr.Length; x++)
                            l[x] = l[x].WithStyle(style);
                    }
                    else
                    {
                        for (int x = x1; x < x2; x++)
                            l[x] = l[x].WithStyle(style);
                    }
                }
                QueueDraw();
            }
            else
            {
                Loc from = GetSanitizedLoc(range.Start);
                Loc to = GetSanitizedLoc(range.End);
                if (from == to)
                    return;
                // Normal
                if (range.IsSingleLine)
                {
                    Line l = arr[from.Y];
                    for (int x = from.X; x < to.X; x++)
                        l[x] = l[x].WithStyle(style);
                }
                else
                {
                    Line l = arr[from.Y];
                    for (int x = 0; x < l.Count; x++)
                        l[x] = l[x].WithStyle(style);

                    if (from.Y != to.Y - 1)
                        for (int i = from.Y; i < to.Y; i++)
                        {
                            l = arr[from.Y];
                            for (int x = 0; x < arr[i].Count; x++)
                                l[x] = l[x].WithStyle(style);
                        }

                    l = arr[to.Y];
                    for (int x = 0; x < l.Count; x++)
                        l[x] = l[x].WithStyle(style);
                }
                QueueDraw();
            }
        }

        public void SetRangeColor(Range range, int color)
        {
            SetRangeStyle(range, (CharpStyle)color);
        }


        // Api
        public void GetSanitizedLoc(ref int y, ref int x)
        {
            y = y < 0 ? 0 : (y >= Count ? Count - 1 : y);
            x = x < 0 ? 0 : (x > arr[y].Count ? arr[y].Count : x);
        }

        public Loc GetSanitizedLoc(Loc loc)
        {
            int y = loc.Y;
            int x = loc.X;
            GetSanitizedLoc(ref y, ref x);
            return new Loc(y, x);
        }

        public void InsertText(string text)
        {
            RemoveSelection();
            InsertText(text, caretPosition, true);
        }

        public void InsertText(string text, Loc loc, bool moveCaret = true)
        {
            InsertText(text, loc.Y, loc.X, moveCaret);
        }

        public void InsertText(string text, int y, int x, bool moveCaret = true)
        {
            Line[] lines = text.GetLines().ToArray();
            if (lines.Length == 0)
                return;

            InsertLines(lines, y, x, moveCaret);
        }

        public void InsertLines(Line[] lines, int y, int x, bool moveCaret = true, bool suppressEvents = false)
        {
            GetSanitizedLoc(ref y, ref x);

            if (lines.Length == 1)
            {
                // insert
                arr[y].InsertRange(lines[0], x);

                // caret
                if (moveCaret)
                    caretPosition = new Loc(y, x + lines[0].Count);

                // events
                if (!suppressEvents)
                    Syntax?.TextAdded(new Range(y, x, y, x + lines[0].Count));
            }
            else
            {
                // insert
                int start = x + lines[0].Count;
                arr[y].InsertRange(lines[0], x);
                InsertRange(lines, y + 1, 1);
                arr[y + lines.Length - 1].AddFromTo(arr[y], start, arr[y].Count);
                arr[y].RemoveFrom(start);

                // caret
                if (moveCaret)
                    caretPosition = new Loc(y + lines.Length - 1, 0);

                // events
                if (!suppressEvents)
                    Syntax?.TextAdded(new Range(y, x, y + lines.Length - 1, lines[lines.Length - 1].Count));
            }

            QueueDraw();
        }

        public void RemoveRange(Range range, bool moveCaret = true, bool isSelection = false, bool suppressEvents = false)
        {
            if (range.IsRectangularSelection)
            {
                Loc from = range.Start;
                Loc to = range.End;

                // Rectangular
                int x1 = Math.Min(from.X, to.X);
                int x2 = Math.Max(from.X, to.X);
                if (x1 == x2)
                    return;
                for (int i = from.Y; i < to.Y; i++)
                {
                    Line l = arr[i];
                    if (l.Count < x1) // out of range
                        continue;
                    else if (l.Count < x2)
                        l.RemoveFrom(x1);
                    else
                        l.RemoveFromTo(x1, x2);
                    l.LineStyle &= LineStyle.Unsaved;
                }

                // caret
                if (moveCaret)
                    caretPosition = from;

                // events
                if (!suppressEvents)
                    Syntax?.TextRemoved(new Range(from, to, SelectionType.Rectangular));

                QueueDraw();
            }
            else
            {
                Loc from = GetSanitizedLoc(range.Start);
                Loc to = GetSanitizedLoc(range.End);
                if (from == to)
                    return;
                // Normal
                if (range.IsSingleLine)
                {
                    arr[from.Y].RemoveFromTo(from.X, to.X);
                }
                else
                {
                    arr[from.Y].RemoveFrom(from.X);
                    if (from.Y != to.Y - 1)
                        RemoveFromTo(from.Y + 1, to.Y - 1);
                    arr[from.Y].AddFromTo(arr[from.Y + 1], 0, to.X);
                    RemoveAt(from.Y + 1);
                }
                if (moveCaret)
                    CaretPosition = from;

                // line style
                arr[from.Y].LineStyle |= LineStyle.Unsaved;

                // caret
                if (moveCaret)
                    caretPosition = from;

                // events
                if (!suppressEvents)
                    Syntax?.TextRemoved(new Range(from, to));

                QueueDraw();
            }
        }
    }
}
