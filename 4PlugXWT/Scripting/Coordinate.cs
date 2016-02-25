using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Scripting
{
    public struct Coordinate
    {
        public int Line { get; private set; }
        public int Char { get; private set; }

        public Coordinate(int line, int _char)
        {
            Line = line;
            Char = _char;
        }

        public static Coordinate Empty
        {
            get
            {
                return new Coordinate(0, int.MinValue);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Char == int.MinValue;
            }
        }
    }
}
