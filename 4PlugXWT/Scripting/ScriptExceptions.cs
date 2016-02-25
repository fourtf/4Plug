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

        public static void ThrowInvalidTokenException(StringParser parser, int position, string token)
        {
            throw new ScriptException(parser, position, $"Invalid token \"{token}\".");
        }

        public static void ThrowUnexpectedEndOfLineException(StringParser parser, int position)
        {
            throw new ScriptException(parser, position, $"Unexpected end of line.");
        }

        public static void ThrowNullVarException(StringParser parser, int position, string varName = null)
        {
            throw new ScriptException(parser, position, $"The variable \"{varName ?? "_unknown"} does not have a value.");
        }

        public static void ThrowTypeIncorrectException(StringParser parser, int position, VariableType type)
        {
            throw new ScriptException(parser, position, $"The variable must be convertible to one of the following types: {type.ToString()}");
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
