using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FPlug.Options.Scripting
{
    public class Variable
    {
        public VariableType Type { get; private set; }
        public object Object { get; private set; }

        public Variable()
        {
            Type = VariableType.Null;
            Object = null;
        }

        public void SetValue(string value)
        {
            Object = value;
            Type = VariableType.String;
        }

        public void SetValue(double value)
        {
            Object = value;
            Type = VariableType.Double;
        }

        public int GetInt()
        {
            double d = GetDouble();
            if (d < int.MaxValue && d > int.MinValue)
                return (int)Math.Round(GetDouble());
            else
                throw new Exception("");
        }

        public double GetDouble()
        {
            switch (Type)
            {
                case VariableType.Double:
                    return (int)Object;
                case VariableType.String:
                    {
                        double val;
                        if (double.TryParse((string)Object, NumberStyles.Float, CultureInfo.InvariantCulture, out val))
                        {
                            return val;
                        }
                        else
                        {
                            throw new Exception();
                            //throw new ScriptException($"The variable \"_unknown\" ({(string)Object}) can not be converted into an integer.");
                        }
                    }
                case VariableType.Null:
                    throw new Exception();
                //ScriptException.ThrowNullVarException();
                //break;
                default:
                    throw new Exception();
                    //ScriptException.ThrowTypeIncorrectException(VariableType.Int, 0);
                    //break;
            }
            return 0;
        }

        public string GetString()
        {
            switch (Type)
            {
                case VariableType.String:
                    return (string)Object;
                default:
                    return Object.ToString();
            }
        }

        public override string ToString()
        {
            return $"{Type}:{Object}";
        }
    }

    [Flags]
    public enum VariableType
    {
        Null = 1,
        String = 2,
        Double = 4,
    }
}
