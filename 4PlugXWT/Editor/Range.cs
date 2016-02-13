using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public struct Range
    {
        private int startY;

        public int StartY
        {
            get { return startY; }
            //set { y = value; }
        }

        private int startX;

        public int StartX
        {
            get { return startX; }
            //set { x = value; }
        }

        private int endY;

        public int EndY
        {
            get { return endY; }
            //set { width = value; }
        }

        private int endX;

        public int EndX
        {
            get { return endX; }
            //set { height = value; }
        }

        public Loc Start { get { return new Loc(startY, startX); } }

        public Loc End { get { return new Loc(endY, endX); } }

        public Range(int startY, int startX, int endX, int endY)
        {
            this.startY = startY;
            this.startX = startX;
            this.endY = endX;
            this.endX = endY;
        }

        public Range(Loc start, Loc end)
        {
            if (start < end)
            {
                this.startY = start.Y;
                this.startX = start.X;
                this.endY = end.Y;
                this.endX = end.X;
            }
            else
            {
                this.endY = start.Y;
                this.endX = start.X;
                this.startY = end.Y;
                this.startX = end.X;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return startY == endY && startX == endX;
            }
        }

        //public Loc WithOffset(int y, int x)
        //{
        //    return new Loc(this.y + y, this.x + x);
        //}

        public static bool operator ==(Range loc1, Range loc2)
        {
            return loc1.startX == loc2.startX && loc1.startY == loc2.startY
                && loc1.endY == loc2.endY && loc1.endX == loc2.endX;
        }

        public static bool operator !=(Range loc1, Range loc2)
        {
            return loc1.startX != loc2.startX || loc1.startY != loc2.startY
                || loc1.endY != loc2.endY || loc1.endX != loc2.endX;
        }

        public override bool Equals(object obj)
        {
            if (obj is Range)
                return (Range)obj == this;
            return false;
        }

        public override int GetHashCode()
        {
            return startY >> 16 + startX;
        }
    }
}
