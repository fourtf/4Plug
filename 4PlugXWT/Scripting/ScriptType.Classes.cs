
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XColor = Xwt.Drawing.Color;

namespace FPlug.Scripting
{
    public partial class ScriptType
    {
        public static readonly ScriptType Bool = new ScriptType(
            "Bool",
            new Method[]
            {
                new Method("ToString", ScriptType.String, new Lambda(v => v.CurrentObject = ((bool)v.CurrentObject) ? "True" : "False")),
                new Method("Log", null, new Lambda(v => v.Script.SettingsWindow.LogError(v.CurrentObject.ToString(), Options.ErrorType.Log))),
            },
            new Property[]
            {
                
            });

        static char[] nlChars = new[] { '/', '\\' };

        public static readonly ScriptType String = new ScriptType(
            "String",
            new Method[]
            {
                new Method("ToUpper", ScriptType.String, new Lambda(v => v.CurrentObject = ((string)v.CurrentObject).ToUpper())),
                new Method("ToLower", ScriptType.String, new Lambda(v => v.CurrentObject = ((string)v.CurrentObject).ToLower())),
                new Method("EndsWith", ScriptType.Bool, new ParamLambda((v, p) => v.CurrentObject = ((string)v.CurrentObject).EndsWith((string)p)), ScriptType.String),
                new Method("StartsWith", ScriptType.Bool, new ParamLambda((v, p) => v.CurrentObject = ((string)v.CurrentObject).StartsWith((string)p)), ScriptType.String),
                new Method("Contains", ScriptType.Bool, new ParamLambda((v, p) => v.CurrentObject = ((string)v.CurrentObject).Contains((string)p)), ScriptType.String),
                new Method("Log", null, new Lambda(v => v.Script.SettingsWindow.LogError((string)v.CurrentObject, Options.ErrorType.Log))),

                //new Method("GetExtension", ScriptType.String, new Lambda(v => { var s = (string)v.CurrentObject; int index = s.LastIndexOf('.'); v.CurrentObject = s.LastIndexOfAny(nlChars) > index ? "" : (index == -1 ? "" : s.Substring(index)); })),
                new Method("GetExtension", ScriptType.String, new Lambda(v => { try{ v.CurrentObject = Path.GetExtension((string)v.CurrentObject);} catch { v.CurrentObject = ""; } })),
                new Method("GetFileName", ScriptType.String, new Lambda(v => { try{ v.CurrentObject = Path.GetFileName((string)v.CurrentObject);} catch { v.CurrentObject = ""; } })),

                new Method("Remove", ScriptType.String, new ParamLambda((v, o) => { var s = (string)v.CurrentObject; int i = (int)Math.Round((double)o); v.CurrentObject = i < 0 ? "" : (s.Length > i ? s.Remove(i) : s); }), ScriptType.Number),
                //new Method("Remove", ScriptType.String, new ParamsLambda((v, o) => { var s = (string)v.CurrentObject; int i1 = (int)Math.Round((double)o[0]); int i2 = (int)Math.Round((double)o[1]); v.CurrentObject = (i1 < 0 || i2 >= 0) ? "" : (s.Length > i2 ? s.Remove(i1, i2) : s); }), ScriptType.Number, ScriptType.Number),
                new Method("Substring", ScriptType.String, new ParamLambda((v, o) => { var s = (string)v.CurrentObject; int i = (int)Math.Round((double)o); v.CurrentObject = i < 0 ? s : (s.Length > i ? s.Substring(i) : ""); }), ScriptType.Number),
                //new Method("Substring", ScriptType.String, new ParamsLambda((v, o) => { var s = (string)v.CurrentObject; int i1 = (int)Math.Round((double)o[0]); int i2 = (int)Math.Round((double)o[1]); v.CurrentObject = (i1 < 0 || i2 >= 0) ? s : (s.Length > i2 ? s.Substring(i1, i2) : ""); }), ScriptType.Number, ScriptType.Number),
            },
            new Property[]
            {
                new Property("Length", ScriptType.Number, new Lambda(v => v.CurrentObject = ((string)v.CurrentObject).Length), null),
            });

        public static readonly ScriptType Number = new ScriptType(
            "Number",
            new Method[]
            {
                new Method("ToString", ScriptType.String, new Lambda(v => v.CurrentObject = v.CurrentObject.ToString())),
                new Method("Log", null, new Lambda(v => v.Script.SettingsWindow.LogError(v.CurrentObject.ToString(), Options.ErrorType.Log))),
            },
            new Property[]
            {
                
            });

        public static ScriptType _Source1Res = new ScriptType(
            "TF2Res",
            new Method[]
            {
                new Method("SetValue", null, new ParamsLambda((v, args) => ((Source1ResourceFile)v.CurrentObject).SetValue(new string[]{ }, (string)args[0], (string)args[1])), ScriptType.String, ScriptType.String),
                new Method("SetValue", null, new ParamsLambda((v, args) => ((Source1ResourceFile)v.CurrentObject).SetValue(new[]{ (string)args[0] }, (string)args[1], (string)args[2])), ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("SetValue", null, new ParamsLambda((v, args) => ((Source1ResourceFile)v.CurrentObject).SetValue(new[]{ (string)args[0], (string)args[1] }, (string)args[2], (string)args[3])), ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("SetValue", null, new ParamsLambda((v, args) => ((Source1ResourceFile)v.CurrentObject).SetValue(new[]{ (string)args[0], (string)args[1], (string)args[2] }, (string)args[3], (string)args[4])), ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("SetValue", null, new ParamsLambda((v, args) => ((Source1ResourceFile)v.CurrentObject).SetValue(new[]{ (string)args[0], (string)args[1], (string)args[2], (string)args[3] }, (string)args[4], (string)args[5])), ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("SetValue", null, new ParamsLambda((v, args) => ((Source1ResourceFile)v.CurrentObject).SetValue(new[]{ (string)args[0], (string)args[1], (string)args[2], (string)args[3], (string)args[4] }, (string)args[5], (string)args[6])), ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("SetValue", null, new ParamsLambda((v, args) => ((Source1ResourceFile)v.CurrentObject).SetValue(new[]{ (string)args[0], (string)args[1], (string)args[2], (string)args[3], (string)args[4], (string)args[5] }, (string)args[6], (string)args[7])), ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("Save", null, new Lambda(v => ((Source1ResourceFile)v.CurrentObject).Save())),
            },
            new Property[]
            {
                new Property("Loaded", Bool, new Lambda(v => v.CurrentObject = ((Source1ResourceFile)v.CurrentObject).Loaded), null),
            });

        public static ScriptType _TF2Scheme = new ScriptType(
            "TF2Scheme",
            new Method[]
            {
                new Method("SetColor", null, new ParamsLambda((v, args) => ((Source1SchemeFile)v.CurrentObject).SetColor((string)args[0], (string)args[1])), ScriptType.String, ScriptType.String),
                new Method("SetBaseSetting", null, new ParamsLambda((v, args) => ((Source1SchemeFile)v.CurrentObject).SetBaseSetting((string)args[0], (string)args[1])), ScriptType.String, ScriptType.String),
                new Method("SetBitmapFontFile", null, new ParamsLambda((v, args) => ((Source1SchemeFile)v.CurrentObject).SetBitmapFontFile((string)args[0], (string)args[1])), ScriptType.String, ScriptType.String),
                new Method("SetFont", null, new ParamsLambda((v, args) => ((Source1SchemeFile)v.CurrentObject).SetFont((string)args[0], (string)args[1], (string)args[2], (string)args[3])), ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("SetBorder", null, new ParamsLambda((v, args) => ((Source1SchemeFile)v.CurrentObject).SetBorder(new[]{ (string)args[0] }, (string)args[1], (string)args[2])), ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("SetBorder", null, new ParamsLambda((v, args) => ((Source1SchemeFile)v.CurrentObject).SetBorder(new[]{ (string)args[0], (string)args[1], (string)args[2] }, (string)args[3], (string)args[4])), ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("SetCustomFontFile", null, new ParamsLambda((v, args) => ((Source1SchemeFile)v.CurrentObject).SetCustomFontFile((string)args[0], (string)args[1])), ScriptType.String, ScriptType.String),
                new Method("SetCustomFontFile", null, new ParamsLambda((v, args) => ((Source1SchemeFile)v.CurrentObject).SetCustomFontFile(new[]{ (string)args[0] }, (string)args[1], (string)args[2])), ScriptType.String, ScriptType.String, ScriptType.String),
                new Method("SetCustomFontFile", null, new ParamsLambda((v, args) => ((Source1SchemeFile)v.CurrentObject).SetCustomFontFile(new[]{ (string)args[0], (string)args[1] }, (string)args[2], (string)args[3])), ScriptType.String, ScriptType.String, ScriptType.String, ScriptType.String),
            },
            new Property[]
            {

            }) { BaseType = _Source1Res };

        public static readonly ScriptType StaticFile = new ScriptType(
            "File",
            new Method[]
            {
                new Method("Exists", ScriptType.Bool, new ParamLambda((v, o) => v.CurrentObject = v.Script.FolderCache.FileExists((string)o)), ScriptType.String),
                new Method("Delete", ScriptType.Bool, new ParamLambda((v, o) => v.CurrentObject = v.Script.FolderCache.DeleteFile((string)o)), ScriptType.String),
                new Method("Move", ScriptType.Bool, new ParamsLambda((v, o) =>  v.CurrentObject = v.Script.FolderCache.TryMoveFile((string)o[0], (string)o[1])), ScriptType.String, ScriptType.String),
                new Method("Copy", ScriptType.Bool, new ParamsLambda((v, o) =>  v.CurrentObject = v.Script.FolderCache.TryCopyFile((string)o[0], (string)o[1])), ScriptType.String, ScriptType.String),
            },
            new Property[]
            {

            });

        public static readonly ScriptType StaticDirectory = new ScriptType(
            "File",
            new Method[]
            {
                new Method("Exists", ScriptType.Bool, new ParamLambda((v, o) => v.CurrentObject = v.Script.FolderCache.DirectoryExists((string)o)), ScriptType.String),
                new Method("Delete", ScriptType.Bool, new ParamLambda((v, o) => v.CurrentObject = v.Script.FolderCache.DeleteDirectory((string)o)), ScriptType.String),
                new Method("Move", ScriptType.Bool, new ParamsLambda((v, o) =>  v.CurrentObject = v.Script.FolderCache.TryMoveDirectory((string)o[0], (string)o[1])), ScriptType.String, ScriptType.String),
                new Method("Copy", ScriptType.Bool, new ParamsLambda((v, o) =>  v.CurrentObject = v.Script.FolderCache.TryCopyDirectory((string)o[0], (string)o[1])), ScriptType.String, ScriptType.String),
            },
            new Property[]
            {

            });

        public static readonly ScriptType Color = new ScriptType(
            "Bool",
            new Method[]
            {
                new Method("ToSource", ScriptType.String, new Lambda(v => { XColor c = (XColor)v.CurrentObject; v.CurrentObject = (int)(c.Red * 255) + " " + (int)(c.Green * 255) + " " + (int)(c.Blue * 255) + " " + (int)(c.Alpha * 255); } )),
                new Method("ToString", ScriptType.String, new Lambda(v => { XColor c = (XColor)v.CurrentObject; v.CurrentObject = "R:" + (int)(c.Red * 255) +" G:" + (int)(c.Green * 255)+" B:" + (int)(c.Blue * 255)+" A:" + (int)(c.Alpha * 255); } )),
                new Method("Log", null, new Lambda(v => v.Script.SettingsWindow.LogError(v.CurrentObject.ToString(), Options.ErrorType.Log))),
            },
            new Property[]
            {
                new Property("R", ScriptType.Number, new Lambda((v) => v.CurrentObject = ((XColor)v.CurrentObject).Red), null),
                new Property("G", ScriptType.Number, new Lambda((v) => v.CurrentObject = ((XColor)v.CurrentObject).Green), null),
                new Property("B", ScriptType.Number, new Lambda((v) => v.CurrentObject = ((XColor)v.CurrentObject).Blue), null),
                new Property("A", ScriptType.Number, new Lambda((v) => v.CurrentObject = ((XColor)v.CurrentObject).Alpha), null),
            });

        public static readonly ScriptType Static = new ScriptType(
            null,
            new Method[]
            {
                new Method("TF2Scheme", _TF2Scheme, (script) => new ParamActivator(script, (sc, arg) => new Source1SchemeFile(sc.FolderCache, (string)arg)), ScriptType.String),
                new Method("TF2Res", _Source1Res, (script) => new ParamActivator(script, (sc, arg) => new Source1ResourceFile(sc.FolderCache, (string)arg)), ScriptType.String),
                new Method("Color", _Source1Res, (script) => new ParamActivator(script, (sc, arg) =>
                {
                    XColor c;
                    if (App.TryParseColor((string)arg, out c))
                        return c;
                    return Xwt.Drawing.Colors.White;
                }), ScriptType.String),
                new Method("Color", _Source1Res, (script) => new ParamsActivator(script, (sc, args) => XColor.FromBytes((byte)Math.Max(0, Math.Min(255, ((double)args[0]))), (byte)Math.Max(0, Math.Min(255, ((double)args[1]))), (byte)Math.Max(0, Math.Min(255, ((double)args[2]))))), ScriptType.Number, ScriptType.Number, ScriptType.Number),
                new Method("Color", _Source1Res, (script) => new ParamsActivator(script, (sc, args) => XColor.FromBytes((byte)Math.Max(0, Math.Min(255, ((double)args[0]))), (byte)Math.Max(0, Math.Min(255, ((double)args[1]))), (byte)Math.Max(0, Math.Min(255, ((double)args[2]))), (byte)Math.Max(0, Math.Min(255, ((double)args[3]))))), ScriptType.Number, ScriptType.Number, ScriptType.Number, ScriptType.Number),
            },
            new Property[]
            {
                new Property("File", ScriptType.StaticFile, Function.Empty, null),
                new Property("Directory", ScriptType.StaticDirectory, Function.Empty, null),
            });

        //public static readonly ScriptType IfState = new ScriptType(
        //    null,
        //    null,
        //    null);

        public static readonly Dictionary<ScriptTypeID, ScriptType> DefaultTypes = new Dictionary<ScriptTypeID, ScriptType>()
        {
            { ScriptTypeID.Boolean, ScriptType.Bool },
            { ScriptTypeID.String, ScriptType.String },
            { ScriptTypeID.Number, ScriptType.Number },
            { ScriptTypeID.Color, ScriptType.Color },
        };
    }
}
