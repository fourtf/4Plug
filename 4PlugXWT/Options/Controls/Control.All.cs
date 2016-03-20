using FPlug.Options.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace FPlug.Options.Controls
{
    public partial class Control
    {
        public static List<ControlType> AllControls = new List<ControlType>();

        public static void InitializeControls()
        {
            try
            {
                var control = typeof(Control);
                getString = control.GetMethod("_getString", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                getDouble = control.GetMethod("_getDouble", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                getInt    = control.GetMethod("_getInt", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                var variable = typeof(Variable);
                getStringFromVar = variable.GetMethod("GetString");
                getDoubleFromVar = variable.GetMethod("GetDouble");
                getIntFromVar = variable.GetMethod("GetInt");

                LoadControl("empty", typeof(EmptyControl));
                LoadControl("label", typeof(LabelControl));
                LoadControl("button", typeof(ButtonControl));
                LoadControl("image", typeof(ImageControl));

                LoadControl("dropdown", typeof(DropDownControl));
                LoadControl("color", typeof(ColorControl));

                LoadControl("group", typeof(GroupContainer));
                LoadControl("box", typeof(Container));
                LoadControl("tab", typeof(Tab));
            }
            catch (Exception exc)
            {
                exc.Message.Log();
            }
        }

        static MethodInfo getString;
        static Variable _getString(string val)
        {
            var var = new Variable();
            var.SetValue(val);
            return var;
        }
        static MethodInfo getDouble;
        static Variable _getDouble(double val)
        {
            var var = new Variable();
            var.SetValue(val);
            return var;
        }
        static MethodInfo getInt;
        static Variable _getInt(int val)
        {
            var var = new Variable();
            var.SetValue(val);
            return var;
        }

        static MethodInfo getStringFromVar;
        static MethodInfo getDoubleFromVar;
        static MethodInfo getIntFromVar;

        static void LoadControl(string name, Type type)
        {
            ControlType c = new ControlType();

            c.Name = name;
            c.Type = type;

            AllControls.Add(c);

            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                object[] memberAttributes = propertyInfo.GetCustomAttributes(typeof(ScriptMemberAttribute), true);
                if (memberAttributes.Length > 0)
                {
                    ScriptMemberAttribute attribute = (ScriptMemberAttribute)memberAttributes[0];

                    Property property = new Property() { Name = attribute.Name };
                    Type propertyType = propertyInfo.PropertyType;

                    // get
                    {
                        var method = propertyInfo.GetGetMethod();
                        var parameter = Expression.Parameter(typeof(object));

                        MethodInfo variableMaker = null;
                        if (propertyType == typeof(string))
                            variableMaker = getString;
                        else if (propertyType == typeof(int))
                            variableMaker = getInt;
                        else if (propertyType == typeof(double))
                            variableMaker = getDouble;
                        
                        try
                        {
                            var func = Expression.Lambda<Func<object, Variable>>(
                                Expression.Call(variableMaker,
                                    Expression.Call(Expression.Convert(parameter, propertyInfo.DeclaringType), method)
                                    ),
                                parameter).Compile();

                            property.GetVariable = func;
                        }
                        catch (Exception exc)
                        {
                            exc.Message.Log();
                        }
                    }

                    // set
                    {
                        var method = propertyInfo.GetSetMethod();

                        var parameter = Expression.Parameter(typeof(object));
                        var value = Expression.Parameter(typeof(Variable));

                        MethodInfo getValFromVar = null;
                        if (propertyType == typeof(string))
                            getValFromVar = getStringFromVar;
                        else if (propertyType == typeof(int))
                            getValFromVar = getIntFromVar;
                        else if (propertyType == typeof(double))
                            getValFromVar = getDoubleFromVar;
                        
                        try
                        {
                            var action = Expression.Lambda<Action<object, Variable>>(
                                Expression.Call(Expression.Convert(parameter, method.DeclaringType),
                                method,
                                Expression.Call(value, getValFromVar)),
                            parameter,
                            value).Compile();

                            property.SetVariable = action;
                        }
                        catch (Exception exc)
                        {
                            exc.Message.Log();
                        }
                    }
                    c.Properties[property.Name] = property;



                    //if (canSet && propertyInfo.CanWrite)
                    //{
                    //    var method = propertyInfo.GetSetMethod();
                    //
                    //    var parameter = Expression.Parameter(typeof(object));
                    //    var value = Expression.Parameter(typeof(object));
                    //
                    //    var action = Expression.Lambda<Action<object, object>>(
                    //        Expression.Call(
                    //            Expression.Convert(parameter, method.DeclaringType),
                    //            method,
                    //            Expression.Convert(value, method.GetParameters()[0].ParameterType)),
                    //        parameter,
                    //        value).Compile();
                    //
                    //    SetAction = action;
                    //    _setFunction = () => new SetProperty(action);
                    //}
                    //if (canGet && propertyInfo.CanRead)
                    //{
                    //    var method = propertyInfo.GetGetMethod();
                    //
                    //    var parameter = Expression.Parameter(typeof(object));
                    //
                    //    var func = Expression.Lambda<Func<object, object>>(
                    //        Expression.Convert(
                    //            Expression.Call(Expression.Convert(parameter, method.DeclaringType), method),
                    //                typeof(object)),
                    //        parameter).Compile();
                    //
                    //    GetFunc = func;
                    //    _getFunction = new GetProperty(func);
                    //}
                }
            }

            //ScriptClassAttribute typeAttribute = (ScriptClassAttribute)typeAttributes[0];
            //
            //List<Method> methods = new List<Method>();
            //List<Property> properties = new List<Property>();
            //List<PropertyDesc> propDescs = new List<PropertyDesc>();
            //Dictionary<string, PropertyInfo> events = new Dictionary<string, PropertyInfo>();
            //
            //t.GetProperties().Do((p) =>
            //{
            //    object[] mAttributes;
            //    if ((mAttributes = p.GetCustomAttributes(typeof(ScriptMemberAttribute), true)).Length > 0)
            //    {
            //        ScriptMemberAttribute memberAttribute = (ScriptMemberAttribute)mAttributes[0];

            //        Property pr0 = new Property(memberAttribute.Name ?? p.Name, ScriptType.DefaultTypes[memberAttribute.ReturnType], p);
            //        properties.Add(pr0);

            //        PropertyDesc propDesc = new PropertyDesc()
            //        {
            //            Name = pr0.Name,
            //            ScriptType = ScriptType.DefaultTypes[memberAttribute.ReturnType],
            //            Get = pr0.GetFunc,
            //            Set = pr0.SetAction
            //        };
            //        propDescs.Add(propDesc);
            //    }
            //    else if ((mAttributes = p.GetCustomAttributes(typeof(ScriptEventAttribute), true)).Length > 0)
            //    {
            //        ScriptEventAttribute eventAttribute = (ScriptEventAttribute)mAttributes[0];

            //        events.Add(p.Name, p);
            //    }
            //});
            //
            //ScriptType scriptType = new ScriptType(typeAttribute.Name, methods.ToArray(), properties.ToArray());
            //
            //ControlDesc controlDesc = new ControlDesc()
            //{
            //    Name = typeAttribute.Name,
            //    ScriptType = scriptType,
            //    Properties = propDescs,
            //    Type = t,
            //    Events = events
            //};
            //Controls.Add(controlDesc);
            //
            //ScriptTypeByType.Add(t, scriptType);
        }
    }
}
