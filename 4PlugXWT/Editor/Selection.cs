using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public struct Range
    {
        // Fields
        private SelectionType type;

        public SelectionType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        private Loc start;

        public Loc Start
        {
            get
            {
                return start;
            }
        }

        private Loc end;

        public Loc End
        {
            get
            {
                return end;
            }
        }

        // Constructor
        public Range(int startY, int startX, int endY, int endX, SelectionType type = SelectionType.Normal)
        {
            start = new Loc(startY, startX);
            end = new Loc(endY, endX);
            this.type = type;
        }

        public Range(Loc start, Loc end, SelectionType type = SelectionType.Normal)
        {
            if (start < end)
            {
                this.start = start;
                this.end = end;
            }
            else
            {
                this.start = end;
                this.end = start;
            }
            this.type = type;
        }

        // Checks
        public bool IsEmpty
        {
            get
            {
                return start == end;
            }
        }

        public bool IsSingleLine
        {
            get { return Start.Y == End.Y; }
        }

        public bool Contains(Loc l)
        {
            if (IsRectangularSelection)
                return l.X >= (Start.X < End.X ? Start.X : End.X) && l.X < (Start.X > End.X ? Start.X : End.X) && l.Y >= Start.Y && l.Y <= End.Y;
            else
                return (IsSingleLine ? (Start.X <= l.X && End.X >= l.X) : (Start.Y < l.Y && End.Y > l.Y) || (Start.Y == l.Y && Start.X <= l.Y) || (End.Y == l.Y && End.X >= l.Y));
        }

        public bool IsRectangularSelection
        {
            get { return Type == SelectionType.Rectangular; }
        }

        // Equals
        public static bool operator ==(Range loc1, Range loc2)
        {
            return loc1.Start == loc2.Start && loc1.End == loc2.End;
        }

        public static bool operator !=(Range loc1, Range loc2)
        {
            return loc1.Start != loc2.Start || loc1.End != loc2.End;
        }

        public override bool Equals(object obj)
        {
            if (obj is Range)
                return (Range)obj == this;
            return false;
        }

        public override int GetHashCode()
        {
            return start.X | start.Y << 8 | end.X << 16 | end.Y << 24;
        }

        // ToString()
        public override string ToString()
        {
            return "{" + Start.X + ", " + Start.Y + " | " + End.X + ", " + End.Y + "}";
        }
    }
}
