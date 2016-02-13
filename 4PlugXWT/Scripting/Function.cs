using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FPlug.Scripting
{
    public class Variable
    {
        public object[] Variables;
        public object CurrentObject = null;
        public Script Script;

        public Variable(object[] variables, Script script)
        {
            Variables = variables;
            Script = script;
        }
    }


    public abstract class Function : System.Collections.IEnumerable
    {
        public FunctionStack[] Args = null;

        public abstract void Execute(Variable param);

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (Args != null)
                return Args.GetEnumerator();
            return empty.GetEnumerator();
        }

        static int[] empty = new int[0];
        
        public override string ToString()
        {
            return GetType().Name + " (" + Args + ") ";
        }

        public class EmptyFunction : Function
        {
            public override void Execute(Variable param)
            {
                
            }
        }

        public static Function Empty = new EmptyFunction();
    }

    public class FunctionStack : Function, System.Collections.IEnumerable
    {
        public Function[] Functions;

        public ScriptType ReturnType { get; set; }

        public FunctionStack()
        {

        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Functions.GetEnumerator();
        }

        public override string ToString()
        {
            return "FunctionStack (" + ReturnType + ")";
        }

        public override void Execute(Variable param)
        {
            if (Functions.Length > 0)
            {
                for (int i = 0; i < Functions.Length; i++)
                    Functions[i].Execute(param);
            }
            //else
            //    return null;
        }

        public object ExecuteByVarClone(Variable variable)
        {
            if (Functions.Length > 0)
            {
                var var = new Variable(variable.Variables, variable.Script);

                for (int i = 0; i < Functions.Length; i++)
                    Functions[i].Execute(var);

                return var.CurrentObject;
            }
            else
                return null;
        }
    }


    public class Constant : Function
    {
        public object Variable { get; set; }

        public Constant(object constant)
        {
            Variable = constant;
        }

        public override void Execute(Variable param)
        {
            param.CurrentObject = Variable;
        }

        public override string ToString()
        {
            return "Constant " + Variable;
        }
    }

    public class GetVariable : Function
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public Script Script { get; set; }

        public GetVariable(Script script, string name, int index)
        {
            Script = script;
            Name = name;
            Index = index;
        }

        public override string ToString()
        {
            return "get \"" + Name + "\" at " + Index;
        }

        public override void Execute(Variable param)
        {
            param.CurrentObject = param.Variables[Index];
        }
    }

    public class SetVariable : Function
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public ScriptType Type { get; set; }

        public Script Script { get; set; }

        public SetVariable(string variableName, Script script, ScriptType type)
        {
            int index;
            if ((index = script.VariableNames.IndexOf(variableName)) == -1)
            {
                Index = script.VariableNames.Count;
                script.VariableNames.Add(variableName);
                script.VariableTypes.Add(type);
            }
            else
            {
                Index = index;
            }
            Name = variableName;
            Type = type;
        }

        public override string ToString()
        {
            return ">> " + Name + " at " + Index;
        }

        public override void Execute(Variable param)
        {
            param.Variables[Index] = param.CurrentObject;
        }
    }


    public class SetProperty : Function
    {
        public Action<object, object> Action { get; set; }

        public SetProperty(Action<object, object> action)
        {
            Action = action;
        }

        public override void Execute(Variable param)
        {
            Action(param.CurrentObject, Args[0].ExecuteByVarClone(param));
        }
    }

    public class GetProperty : Function
    {
        public Func<object, object> Action { get; set; }
    
        public GetProperty(Func<object, object> action)
        {
            Action = action;
        }
    
        public override void Execute(Variable param)
        {
            param.CurrentObject = Action(param.CurrentObject);
        }
    }


    public class IfFunction : Function
    {
        public FunctionStack Condition = null;

        public override void Execute(Variable param)
        {
            if ((bool)Condition.ExecuteByVarClone(param))
            {
                Args[0].ExecuteByVarClone(param);
                param.CurrentObject = false;
            }
            else
                param.CurrentObject = true;
        }
    }

    public class ElseFunction : Function
    {
        public FunctionStack Condition = null;

        public override void Execute(Variable param)
        {
            if ((bool)param.CurrentObject)
            {
                if (Condition == null || (bool)Condition.ExecuteByVarClone(param))
                {
                    Args[0].ExecuteByVarClone(param);
                    param.CurrentObject = false;
                }
            }
        }
    }


    public class Lambda : Function
    {
        public Action<Variable> Function;

        public Lambda(Action<Variable> function)
        {
            this.Function = function;
        }

        public override void Execute(Variable param)
        {
            Function(param);
        }
    }

    public class ParamLambda : Function
    {
        public Action<Variable, object> Function;

        public ParamLambda(Action<Variable, object> function, params FunctionStack[] args)
        {
            this.Function = function;
            Args = args;
        }

        public override void Execute(Variable param)
        {
            Function(param, Args[0].ExecuteByVarClone(param));
        }
    }

    public class ParamsLambda : Function
    {
        public Action<Variable, object[]> Function;

        public ParamsLambda(Action<Variable, object[]> function, params FunctionStack[] args)
        {
            this.Function = function;
            Args = args;
        }

        public override void Execute(Variable param)
        {
            object[] args = new object[Args.Length];
            for (int i = 0; i < Args.Length; i++)
                args[i] = Args[i].ExecuteByVarClone(param);

            Function(param, args);
        }
    }

    public class ShortCircuitAnd : Function
    {
        public ShortCircuitAnd(FunctionStack stack)
        {
            Args = new FunctionStack[] { stack };
        }

        public override void Execute(Variable param)
        {
            if ((bool)param.CurrentObject)
                param.CurrentObject = Args[0].ExecuteByVarClone(param);
        }
    }

    public class ShortCircuitOr : Function
    {
        public ShortCircuitOr(FunctionStack stack)
        {
            Args = new FunctionStack[] { stack };
        }

        public override void Execute(Variable param)
        {
            if (!(bool)param.CurrentObject)
                param.CurrentObject = Args[0].ExecuteByVarClone(param);
        }
    }

    public class ParamActivator : Function
    {
        Func<Script, object, object> _initializer;
        Script _script;

        public ParamActivator(Script script, Func<Script, object, object> initializer)
        {
            _initializer = initializer;
            _script = script;
        }

        public override void Execute(Variable param)
        {
            param.CurrentObject = _initializer(_script, Args[0].ExecuteByVarClone(param));
        }
    }

    public class ParamsActivator : Function
    {
        Func<Script, object[], object> _initializer;
        Script _script;

        public ParamsActivator(Script script, Func<Script, object[], object> initializer)
        {
            _initializer = initializer;
            _script = script;
        }

        public override void Execute(Variable param)
        {
            object[] args = new object[Args.Length];
            for (int i = 0; i < Args.Length; i++)
                args[i] = Args[i].ExecuteByVarClone(param);

            param.CurrentObject = _initializer(_script, args);
        }
    }

    /*
    public class DoFunction : Function
    {
        public override object Execute(object param)
        {
            if ((bool)param)
            {
                Parameter.Execute();
            }
            return param;
        }
    }

    public class DontFunction : Function
    {
        public override object Execute(object param)
        {
            if (!(bool)param)
            {
                Parameter.Execute();
            }
            return param;
        }
    }
    */


    //File Function
    /*
    public class FileFunction : Function
    {
        public enum FileOperation
        {
            MoveTo,
            CopyTo,
            Create,
            Append,
        }

        FileOperation Operation;

        public FileFunction(FileOperation operation)
        {
            Operation = operation;
        }

        public override string ToString()
        {
            return "File: " + Operation + ", " + Parameter;
        }

        public override void Execute(Variable param)
        {
            try
            {
                switch (Operation)
                {
                    case FileOperation.MoveTo:
                        File.Move(Path.Combine(Root.BaseFolder, (string)param), Path.Combine(Root.BaseFolder, (string)Parameter.Execute()));
                        param.CurrentObject = 
                        break;
                    case FileOperation.CopyTo:
                        File.Copy(Path.Combine(Root.BaseFolder, (string)param), Path.Combine(Root.BaseFolder, (string)Parameter.Execute()), true);
                        break;
                    case FileOperation.Create:
                        File.WriteAllText(Path.Combine(Root.BaseFolder, (string)param), (string)Parameter.Execute());
                        break;
                    case FileOperation.Append:
                        using (var w = File.AppendText(Path.Combine(Root.BaseFolder, (string)param)))
                            w.Write(Parameter);
                        break;
                }
            }
            catch { }
        }
    }
    */
}