using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Scripting
{
    public partial class ScriptType
    {
        // Metadata
        public string Name { get; private set; }

        public ScriptType BaseType { get; private set; }

        public bool ExtendsOrIs(ScriptType type)
        {
            ScriptType t1 = this;
            do
            {
                if (t1 == type)
                    return true;
                t1 = t1.BaseType;
            }
            while (t1 != null);
            return false;
        }

        // Ctor
        private ScriptType (string name)
	    {
            Name = name;
	    }

        public ScriptType (string name, Method[] methods, Property[] properties)
            : this (name)
        {
            Methods = methods;
            Properties = properties;
        }

        public override string ToString()
        {
            return Name ?? "_static";
        }

        // Methods
        public Method[] Methods { get; private set; }

        public Method GetMethod(string name, ScriptType[] args)
        {
            ScriptType type = this;
            int id;
            if (!Member.IDs.TryGetValue(name, out id))
                return null;

            do
            {
                Method[] methods = type.Methods;

                for (int i = 0; i < methods.Length; i++)
                {
                    if (methods[i].ID == id && ((args == null && methods[i].Args == null) || ValidateArgs(methods[i].Args, args)))
                        return methods[i];
                }

                type = type.BaseType;
            }
            while (type != null);

            return null;
        }

        public static bool ValidateArgs(ScriptType[] requiredArgs, ScriptType[] givenArgs)
        {
            if (requiredArgs.Length != givenArgs.Length)
                return false;
            for (int i = 0; i < requiredArgs.Length; i++)
            {
                if (!givenArgs[i].ExtendsOrIs(requiredArgs[i]))
                    return false;
            }
            return true;
        }

        // Properties
        public Property[] Properties { get; private set; }

        public Property GetProperty(string name)
        {
            ScriptType type = this;
            
            int id;
            if (!Member.IDs.TryGetValue(name, out id))
                return null;

            do
            {
                Property[] properties = type.Properties;

                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].ID == id)
                        return properties[i];
                }

                type = type.BaseType;
            }
            while (type != null);

            return null;
        }

        // Lists
        //static Dictionary<ScriptType, ScriptType> listTypes = new Dictionary<ScriptType, ScriptType>();
        
        //static Method[] listMethods = new Method[]
        //{
        //    //new Method("Get", ScriptType.Number, new ParamLambda((v, o) => v.C
        //};
        //
        //static Property[] listProperies = new Property[]
        //{
        //    //new Property("Count", 
        //};
        //
        //public ScriptType CreateListType()
        //{
        //    ScriptType type;
        //    if (listTypes.TryGetValue(this, out type))
        //    {
        //        return type;
        //    }
        //    else
        //    {
        //        type = new ScriptType(Name + "[]",
        //            new Method[]
        //            {
        //                
        //            },
        //            new Property[]
        //            {
        //
        //            });
        //        listTypes.Add(this, type);
        //        return type;
        //    }
        //}
        //
        //public bool IsList { get; private set; }
        //public ScriptType ListType { get; private set; }
    }
}
