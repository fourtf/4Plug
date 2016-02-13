using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public class Syntax
    {
        public TextSource Source { get; private set; }

        public Syntax(TextSource source)
        {
            Source = source;
        }

        public virtual void TextRemoved(Range range)
        {

        }

        public virtual void TextAdded(Range range)
        {

        }
    }
}
