using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FPlug.Scripting
{
    public abstract class Member
    {
        public ScriptType ReturnType { get; private set; }

        public string Name { get; private set; }
        public int ID { get; private set; }

        static int cID = 0;

        public static Dictionary<string, int> IDs = new Dictionary<string, int>();

        public Member(string name, ScriptType returnType)
        {
            Name = name;
            int id;
            if (IDs.TryGetValue(name, out id))
                ID = id;
            else
            {
                ID = cID++;
            IDs.Add(name, ID);
                }
            ReturnType = returnType;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Method : Member
    {
        public ScriptType[] Args { get; private set; }

        private Function _function = null;
        private Func<Script, Function> _func = null;

        private bool hasArgs;
        public bool HasArgs { get { return hasArgs; } }

        public Function CreateFunction(Script script)
        {
            return _function ?? _func(script);
        }

        public Method(string name, ScriptType returnType, Function function, params ScriptType[] argTypes)
            : base (name, returnType)
        {
            _function = function;
            Args = argTypes.Length > 0 ? argTypes : null;
            hasArgs = Args != null;
        }

        public Method(string name, ScriptType returnType, Func<Script, Function> function, params ScriptType[] argTypes)
            : base(name, returnType)
        {
            _func = function;
            Args = argTypes.Length > 0 ? argTypes : null;
            hasArgs = Args != null;
        }
    }

    public class Property : Member
    {
        private Function _getFunction;
        public Function GetGetFunction()
        {
            return _getFunction;
        }

        public bool HasGet { get { return _getFunction != null; } }

        public Func<object, object> GetFunc { get; private set; }

        private Func<Function> _setFunction;
        public Function GetSetFunction()
        {   
            return _setFunction();
        }

        public bool HasSet { get { return _setFunction != null; } }

        public Action<object, object> SetAction { get; private set; }

        public Property(string name, ScriptType type, Function getFunction, Func<Function> setFunction)
            : base(name, type)
        {
            _getFunction = getFunction;
            _setFunction = setFunction;
        }

        public Property(string name, ScriptType type, PropertyInfo propertyInfo, bool canGet = true, bool canSet = true)
            : base(name, type)
        {
            if (canGet && propertyInfo.CanRead)
            {
                var method = propertyInfo.GetGetMethod();

                var parameter = Expression.Parameter(typeof(object));

                var func = Expression.Lambda<Func<object, object>>(
                    Expression.Convert(
                        Expression.Call(Expression.Convert(parameter, method.DeclaringType), method),
                            typeof(object)),
                    parameter).Compile();

                GetFunc = func;
                _getFunction = new GetProperty(func);
            }
            if (canSet && propertyInfo.CanWrite)
            {
                var method = propertyInfo.GetSetMethod();

                var parameter = Expression.Parameter(typeof(object));
                var value = Expression.Parameter(typeof(object));
                
                var action = Expression.Lambda<Action<object, object>>(
                    Expression.Call(
                        Expression.Convert(parameter, method.DeclaringType),
                        method,
                        Expression.Convert(value, method.GetParameters()[0].ParameterType)),
                    parameter,
                    value).Compile();

                SetAction = action;
                _setFunction = () => new SetProperty(action);
            }
        }
    }
}
