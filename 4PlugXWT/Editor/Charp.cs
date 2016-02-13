using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public struct StyledChar
    {
        // Fields
        private Char c;
        public Char Char
        {
            get { return c; }
        }

        private short style;
        public short Style
        {
            get { return style; }
        }

        // Constructors
        public StyledChar(char c)
        {
            this.c = c;
            style = 0;
        }

        public StyledChar(char c, short style)
        {
            this.c = c;
            this.style = style;
        }

        public StyledChar WithStyle(short style)
        {
            return new StyledChar(c, style);
        }

        public StyledChar WithChar(short style)
        {
            return new StyledChar(c, style);
        }
    }
}
