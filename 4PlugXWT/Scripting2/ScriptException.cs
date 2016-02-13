using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Scripting2
{
    [Serializable]
    public class ScriptException : Exception
    {
        public Coordinate Position { get; set; }

        public ScriptException() { }

        public ScriptException(string message) : base(message)
        {

        }

        public ScriptException(string message, Coordinate position)
        {
            Position = position;
        }

        public static void ThrowNullVarException(string varName = null)
        {
            throw new ScriptException($"The variable \"{varName ?? "_unknown"} does not have a value.");
        }

        public static void ThrowTypeIncorrectException(VariableType type)
        {
            throw new ScriptException($"The variable must be convertible to one of the following types: {type.ToString()}");
        }
    }

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
