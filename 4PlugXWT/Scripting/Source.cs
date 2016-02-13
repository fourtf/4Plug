using FPlug.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xwt.Drawing;

namespace FPlug.Scripting
{
    public class Source
    {
        SettingsWindow Window;

        SourceType sourceType = SourceType.None;
        string file = null;
        string path = null;
        string seperator = " - ";

        string selected = null;
        List<string> allItems = null;

        public Source(SettingsWindow window, string source)
        {
            Window = window;

            List<Tuple<string, string>> items = new List<Tuple<string, string>>();

            bool inEscape = false;
            bool inName = true;
            string name = "";
            string v = "";

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == '\'')
                {
                    if (inEscape)
                        if (inName)
                            name += source[i];
                        else
                            v += source[i];
                    inEscape = !inEscape;
                }
                else if (!inEscape && source[i] == ';')
                {
                    if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(v))
                        items.Add(Tuple.Create(name.Trim(), v.Trim()));
                    name = "";
                    v = "";
                    inName = true;
                }
                else if (!inEscape && source[i] == ':' && inName)
                {
                    inName = false;
                }
                else
                {
                    if (inName)
                        name += source[i];
                    else
                        v += source[i];
                    inEscape = false;
                }
            }
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(v))
                items.Add(Tuple.Create(name.Trim(), v.Trim()));

            var type = items.FirstOrDefault(t => t.Item1 == "Type");
            if (type != null)
            {
                if (type.Item2 == "Res")
                {
#warning items
                    var fileitem = items.FirstOrDefault(t => t.Item1 == "File");
                    file = fileitem == null ? "resource/clientscheme.res" : fileitem.Item2;

                    var pathitem = items.FirstOrDefault(t => t.Item1 == "Path");
                    if (pathitem == null)
                    {
                        window.LogError("unknown Color Source Type \"" + type.Item2 + "\"", ErrorType.Xml);
                        return;
                    }
                    path = pathitem.Item2;
                    sourceType = SourceType.Source1Res;
                }
                else if (type.Item2 == "SwitchFile")
                {
                    var fileitem = items.FirstOrDefault(t => t.Item1 == "File");
                    if (fileitem == null)
                    {
                        window.LogError("SwitchFile source has no \"File:\" parameter!", ErrorType.Xml);
                        return;
                    }
                    file = fileitem.Item2;

                    List<string> list = new List<string>();

                    var cache = Window.FolderCache.GetRootFromFile(file);
                    var extension = Path.GetExtension(file);
                    var filename = file.GetFileName().RemoveFromRight(extension.Length);
                    var filenameWithSeperator = filename + seperator;

                    string selected = null;
                    list.AddRange(cache.Files.Where((s) => s.StartsWith(filenameWithSeperator, StringComparison.OrdinalIgnoreCase) && (extension.Length == 0 || s.EndsWith(extension, StringComparison.OrdinalIgnoreCase))).Select(s => s.Substring(filenameWithSeperator.Length).RemoveFromRight(extension.Length)).Select(s => s.EndsWith(" (selected)", StringComparison.OrdinalIgnoreCase) ? s.RemoveFromRight(" (selected)".Length) : s));
#warning selected
                    this.selected = selected;

                    allItems = list;

                    sourceType = SourceType.SwitchFile;
                }
                else
                {
                    window.LogError($"unknown Source Type \"{type.Item2}\"", ErrorType.Xml);
                }
            }
        }

        public string GetValue()
        {
            if (sourceType == SourceType.Source1Res)
            {
                var res = getScheme(file);
                if (res != null && res.Loaded)
                {
                    string[] S = path.Split('/');
                    if (S.Length == 0)
                    {

                    }
                    else if (S.Length == 1)
                    {
                        return res.GetValue(new string[0], S[0]);
                    }
                    else
                    {
                        string[] SPath = new string[S.Length - 1];
                        string SName = S[S.Length - 1];
                        Array.Copy(S, SPath, S.Length - 1);

                        return res.GetValue(SPath, SName);
                    }
                }
            }
            else if (sourceType == SourceType.SwitchFile)
            {
                return allItems.FirstOrDefault(s => s.EndsWith(" (selected)", StringComparison.OrdinalIgnoreCase))?.RemoveFromRight(" (selected)".Length);
            }
            return null;
        }

        public List<string> GetValues()
        {
            return allItems;
        }

        public Color? GetColor()
        {
            if (sourceType == SourceType.Source1Res)
            {
                string s = GetValue();

                if (s == null)
                    return null;

                Color c;
                if (App.TryParseColor(s, out c))
                    return c;

                return Colors.White;
            }
            return null;
        }

        public bool GetBool()
        {
            if (sourceType == SourceType.Source1Res)
            {

            }
            return false;
        }

        public void SetValue(string value)
        {
            if (sourceType == SourceType.Source1Res)
            {
                string[] S = path.Split('/');
                if (S.Length == 0)
                {

                }
                else if (S.Length == 1)
                {
                    var res = getScheme(file);
                    if (res != null && res.Loaded)
                        res.SetValue(new string[0], S[0], value);
                }
                else
                {
                    string[] SPath = new string[S.Length - 1];
                    string SName = S[S.Length - 1];
                    Array.Copy(S, SPath, S.Length - 1);

                    var res = getScheme(file);
                    if (res != null && res.Loaded)
                        res.SetValue(SPath, SName, value);
                }
            }
            else if (sourceType == SourceType.SwitchFile)
            {
#warning finish this
            }
        }

        public void SetColor(Color color)
        {
            if (sourceType == SourceType.Source1Res)
            {
                SetValue((int)(color.Red * 255) + " " + (int)(color.Green * 255) + " " + (int)(color.Blue * 255) + (color.Alpha == 1 ? "" : " " + (int)(color.Alpha * 255)));
            }
        }

        public void SetBool(bool b)
        {

        }

        public static readonly int ResID = App.GetUniqueID();
        public static readonly int FilesID = App.GetUniqueID();

        T getResource<T>(string path, SourceType id, Func<T> createNew)
        {
            Tuple<SourceType, object> obj;
            if (Window.Resources.TryGetValue(path, out obj))
            {
                if (obj.Item1 != id)
                {
                    Window.LogError($"Every file can only be used for one type of Source. In this case {id} and {obj.Item1} can not both be used.", ErrorType.Xml);
                    return default(T);
                }
                else
                {
                    return (T)obj.Item2;
                }
            }
            else
            {
                var scheme = createNew();
                Window.Resources[path] = Tuple.Create(id, (object)scheme);
                return scheme;
            }
        }

        Source1ResourceFile getScheme(string path)
        {
            return getResource(path, SourceType.Source1Res, () => new Source1ResourceFile(Window.FolderCache, path));
        }

        //Source1ResourceFile getScheme(string path)
        //{
        //    object obj;
        //    if (Window.TempSources.TryGetValue(ResID, out obj))
        //    {
        //        var dic = (Dictionary<string, Source1ResourceFile>)obj;
        //
        //        Source1ResourceFile scheme;
        //        if (dic.TryGetValue(path, out scheme))
        //            return scheme;
        //        else
        //        {
        //            scheme = new Source1ResourceFile(Window.FolderCache, path);
        //            dic[path] = scheme;
        //            return scheme;
        //        }
        //    }
        //    else
        //    {
        //        var dic = new Dictionary<string, Source1ResourceFile>();
        //        var scheme = new Source1ResourceFile(Window.FolderCache, path);
        //        dic[path] = scheme;
        //        Window.TempSources[ResID] = dic;
        //        Window.DisposeTempSource[ResID] = (o) => ((Dictionary<string, Source1ResourceFile>)o).Values.Do((res) => res.Save());
        //        return scheme;
        //    }
        //}
    }

    public enum SourceType
    {
        None,
        Source1Res,
        SwitchFile,
        LegacyCfg
    }
}
