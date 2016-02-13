using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Scripting
{
    public class ScriptException : Exception
    {
        public int Position { get; private set; }
        public StringParser Parser { get; private set; }

        public ScriptException(StringParser parser)
        {
            Position = parser.Position;
            Parser = parser;
        }

        public ScriptException(StringParser parser, string message)
            : base(message)
        {
            Position = parser.Position;
            Parser = parser;
        }

        public ScriptException(StringParser parser, int position)
        {
            Position = position;
            Parser = parser;
        }

        public ScriptException(StringParser parser, int position, string message)
            : base(message)
        {
            Position = position;
            Parser = parser;
        }
    }

    public class UnexpectedCharException : ScriptException
    {
        public UnexpectedCharException(int position, StringParser parser)
            : base(parser, position, string.Format("{0} is an unexpected character!", parser.Text[position]))
        {
            
        }
    }

    public class UnexpectedEndException : ScriptException
    {
        public UnexpectedEndException(int position, StringParser parser)
            : base(parser, position, "Unexpected End")
        {
            
        }
    }
}
