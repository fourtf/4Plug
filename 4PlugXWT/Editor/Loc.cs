using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public struct Loc
    {
        // Static
        public static readonly Loc Empty = new Loc(0, 0);

        // Properties
        private int y;

        public int Y
        {
            get { return y; }
            //set { y = value; }
        }

        private int x;

        public int X
        {
            get { return x; }
            //set { x = value; }
        }

        // Constructor
        public Loc(int y, int x)
        {
            this.y = y;
            this.x = x;
        }

        // To()
        public Loc WithOffset(int y, int x)
        {
            return new Loc(this.y + y, this.x + x);
        }

        public Loc WithOffset(Loc offset)
        {
            return new Loc(y + offset.y, x + offset.x);
        }

        public Range ToSelection(SelectionType type = SelectionType.Normal)
        {
            return new Range(this, this, type);
        }

        // Equals
        public static bool operator ==(Loc loc1, Loc loc2)
        {
            return loc1.x == loc2.x && loc1.y == loc2.y;
        }

        public static bool operator !=(Loc loc1, Loc loc2)
        {
            return loc1.x != loc2.x || loc1.y != loc2.y;
        }

        public static bool operator >(Loc loc1, Loc loc2)
        {
            return loc1.y > loc2.y || (loc1.y == loc2.y && loc1.x > loc2.x);
        }

        public static bool operator <(Loc loc1, Loc loc2)
        {
            return loc1.y < loc2.y || (loc1.y == loc2.y && loc1.x < loc2.x);
        }

        public override bool Equals(object obj)
        {
            if (obj is Loc)
                return (Loc)obj == this;
            return false;
        }

        public override int GetHashCode()
        {
            return y >> 16 + x;
        }

        // ToString()
        public override string ToString()
        {
            return "{" + X.ToString() + ", " + Y.ToString() + "}";
        }
    }
}
