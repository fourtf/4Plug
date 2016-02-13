using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public struct Charp
    {
        // Fields
        private Char c;
        public Char Char
        {
            get { return c; }
        }

        private CharpStyle style;
        public CharpStyle Style
        {
            get { return style; }
        }

        // Constructors
        public Charp(char c)
        {
            this.c = c;
            style = 0;
        }

        public Charp(char c, CharpStyle style)
        {
            this.c = c;
            this.style = style;
        }

        public Charp WithStyle(CharpStyle style)
        {
            return new Charp(c, style);
        }

        public Charp WithChar(CharpStyle style)
        {
            return new Charp(c, style);
        }

        public override string ToString()
        {
            return c.ToString();
        }
    }

    [Flags]
    public enum CharpStyle : ushort
    {
        None = 0,
        //Bold = 0x0010,
        //Italic = 0x0020,
        //Underline = 0x0040,
        //Striked = 0x0080,
        Color1 = 0x0001,
        Color2 = 0x0002,
        Color3 = 0x0004,
        Color4 = 0x0008,
        ColorAll = Color1 | Color2 | Color3 | Color4,
        //Selected    = 0x0100,
        //Snippet     = 0x0200,
    }

    public static class StyledCharExtensions
    {
        public static Charp ToStyledChar(this char c)
        {
            return new Charp(c);
        }

        public static Charp ToStyledChar(this char c, CharpStyle style)
        {
            return new Charp(c, style);
        }

        public static List<Line> GetLines(this string s, int tabPosition = 0)
        {
            List<Line> L = new List<Line>();
            using (StringReader reader = new StringReader(s))
            {
                Line line = new Line();
                string l;
                while ((l = reader.ReadLine()) != null)
                {
                    L.Add(new Line(l));
                }
            }
            return L;
        }
    }
}
