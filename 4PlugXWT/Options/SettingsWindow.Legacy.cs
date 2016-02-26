using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options
{
    public partial class SettingsWindow
    {
        public static SettingsWindow CreateLegacySettings(string folder, string[] code)
        {
            SettingsWindow window = new SettingsWindow(folder, code);

            return window;
        }

        ctrlStyle CtrlStyle = new ctrlStyle();

        // Legacy Mode
        private SettingsWindow(string folder, string[] code)
        {
            // 479px x 513px
            LegacyMode = true;

            Resizable = false;

            Icon = App.Icon;

            Title = folder;

            FolderCache = new FolderCache(folder);

            App.InitializeOptionsTask.Wait();

            // Create Window
            VBox vbox = new VBox();
            this.Content = vbox;

            HBox hbox = new HBox();
            Button applyButton = new Button(" Apply ");
            applyButton.Clicked += (s, e) =>
            {
                FolderCache = new FolderCache(folder);

                if (Script != null) Script.Execute();

                Action<SContainer> execApply = null;
                execApply = (c) => c.children.Do((w) => { w.ExecApplyScript(); if (w is SContainer) execApply((SContainer)w); });
                execApply(Container);

                execApply = (c) => c.children.Do((w) => { w.Apply(); if (w is SContainer) execApply((SContainer)w); });
                execApply(Container);

                DisposeControlResources();
            };

            hbox.PackEnd(applyButton);
            Container = new SBox();
            ScrollView scroll = new ScrollView(Container);
            scroll.MinWidth = 479;
            scroll.MinHeight = 513;
            vbox.PackStart(scroll);
            Container.Window = this;
            vbox.PackEnd(hbox);

            // Start Legacy Code
            StringBuilder script = new StringBuilder(1024);
            string[] lines = code;
            int LastControlID = 0;

            for (int i = 0; i < lines.Length; ++i)
            {
                if (lines[i].Trim().StartsWith("["))
                {
                    CtrlStyle = parseCtrlStyle(lines[i], CtrlStyle);
                }
                else if (lines[i].Trim().StartsWith("#"))
                {
                    int layer = 0;
                    List<string> S = new List<string>();
                    while (lines.Length > i)
                    {
                        S.Add(lines[i].Trim());

                        if (lines[i].Trim() == "{")
                        {
                            layer++;
                            i++; continue;
                        }
                        else if (lines[i].Trim() == "}")
                        {
                            layer--;
                            if (layer == 0)
                                break;
                            i++;

                            continue;
                        }
                        i++;
                    }
                    ConditionItem A = ParseConditionalItem(S);
                    if (A != null)
                        ifs.Add(A);
                }
                else if (lines[i].Contains("("))
                {
                    string type = lines[i].Remove(lines[i].IndexOf('(')).Trim();
                    List<string> args = new List<string>();
                    List<string> linesEx = new List<string>();
                    List<string> multilines = new List<string>();

                    string S = lines[i].Substring(lines[i].IndexOf('(') + 1).Trim();
                    string s = "";
                    bool inQuotes = false;
                    bool inArg = false;
                    bool supportMultilines = false;

                    #region parse line
                    if (!lines[i].EndsWith(";") && lines.Length - 2 > i)
                        supportMultilines = true;
                    for (int q = 0; q < S.Length; ++q)
                    {
                        if (inQuotes)
                        {
                            if (S[q] == '"')
                            {
                                inQuotes = false;
                                inArg = false;
                                args.Add(s);
                                s = "";
                            }
                            else
                            {
                                s += S[q];
                            }
                        }
                        else
                        {
                            if (inArg)
                            {
                                if (S[q] == ' ')
                                    continue;
                                else if (S[q] == ')')
                                {

                                }
                                else if (S[q] == ',')
                                {
                                    args.Add(s);
                                    s = "";
                                    inArg = false;
                                }
                                else
                                    s += S[q];
                            }
                            else
                            {
                                if (S[q] == ' ')
                                    continue;
                                else if (S[q] == ',')
                                {
                                    if (s != "")
                                        args.Add(s);
                                    s = "";
                                }
                                else if (S[q] == ')')
                                {
                                    if (S.Length - 1 != q && S[q + 1] == ';')
                                    {
                                        if (s != "")
                                            args.Add(s);
                                        break;
                                    }
                                }
                                else if (S[q] == '"')
                                    inQuotes = true;
                                else
                                    s += S[q];
                            }
                        }
                    }
                    if (supportMultilines)
                    {
                        if (lines[i + 1].Trim() != "{")
                        {
                            //log(type, @"Couldn't find ""{{"", maybe you forgot to end the line with "";""");
                        }
                        else
                        {
                            i += 2;
                            while (lines.Length > i)
                            {
                                if (lines[i].Trim() == "}")
                                    break;

                                multilines.Add(lines[i].Trim());

                                ++i;
                            }
                        }
                    }
                    #endregion parse line

                    switch (type)
                    {
                        case "Window":
                            //#region Window
                            //applyCtrlStyle(CtrlStyle, this);
                            break;
                        //#endregion
                        case "Banner":
                            #region Banner
                            {
                                if (args.Count == 0)
                                {
                                    //log("Banner", "No arguments!", 1);
                                    break;
                                }

                                //args[0] = FolderCache.TryResolvePath(args[0], false);
                                //
                                //if (!CheckFile(args[0]))
                                //    break;

                                try
                                {
                                    SImage image = new SImage();
                                    image.Window = this;
                                    image.Parent = Container;
                                    image.Path = args[0];

                                    //PictureBox p = new PictureBox();
                                    //using (Stream stream = new FileStream(Path.Combine(BaseFolder, args[0]), FileMode.Open))
                                    //{
                                    //    Image img = Image.FromStream(stream);
                                    //    p.Size = new Size(ControlWidth, args.Count > 1 ? parseHeight(args[1], img.Height) : 100);
                                    //    p.SizeMode = args.Count > 2 ? parsePicSizeMode(args[2], PictureBoxSizeMode.CenterImage) : PictureBoxSizeMode.CenterImage;
                                    //    p.Image = img;
                                    //}
                                    applyCtrlStyle(CtrlStyle, image);

                                    Container.AddChild(image);
                                }
                                catch (Exception exc)
                                {
                                    //log("Banner", exc.Message);
                                }
                            }
                            break;
                            #endregion Banner
                        case "DynamicFileSwitcher":
                            #region DynamicFileSwitcher
                            {
                                if (!(args.Count == 2 || args.Count == 3))
                                {
                                    //log("DynamicFileSwitcher", "DynamicFileSwitcher needs eighter 2 or 3 arguments!", 1);
                                    break;
                                }

                                //args[0] = Utils.ParsePath(args[0]);
                                //if (!CheckFile(args[0]))
                                //    break;

                                SComboBox combo = new SComboBox();
                                combo.Window = this;
                                combo.Parent = Container;

                                combo.Source = "Type:FileSwitch; File:" + args[0].Replace(":", "\\:").Replace(";", "\\;") + ";";

                                //DynamicFileSwitcher d;
                                //if (args.Count == 2)
                                //    d = new DynamicFileSwitcher(CtrlStyle.Name, CtrlStyle.Default, Path.Combine(BaseFolder, args[0]), args[1]);
                                //else
                                //    d = new DynamicFileSwitcher(CtrlStyle.Name, CtrlStyle.Default, Path.Combine(BaseFolder, args[0]), args[1], args[2] == "1" ? true : false);
                                //d.Width = ControlWidth;
                                //
                                //applyCtrlStyle(CtrlStyle, d);
                                //flowControl.Controls.Add(d);

                                applyCtrlStyle(CtrlStyle, combo);

                                Container.AddChild(combo);
                            }
                            break;
                            #endregion
                        case "HudMenuConnectButton":
                            #region HudGameMenuConnectButton
                            {
                                if (!(args.Count == 2))
                                {
                                    //log("HudMenuConnectButton", "HudGameMenuConnectButton needs 2 arguments!", 1);
                                    break;
                                }

                                SGroup group = new SGroup();
                                group.Parent = Container;
                                group.Window = this;

                                group.Text = args[1];
                                {
                                    LastControlID++;

                                    STextInput label = new STextInput();
                                    label.ID = "l" + LastControlID;

                                    STextInput ip = new STextInput();
                                    ip.ID = "c" + LastControlID;

                                    STextInput password = new STextInput();
                                    password.ID = "p" + LastControlID;

                                    group.AddChild(new SLabel() { Text = "Text", WidthPercentage = .3 });
                                    group.AddChild(label);
                                    group.AddChild(new SLabel() { Text = "IP", WidthPercentage = .3 });
                                    group.AddChild(ip);
                                    group.AddChild(new SLabel() { Text = "Password:", WidthPercentage = .3 });
                                    group.AddChild(password);

                                    script.AppendLine(string.Format(@"s{0} = {1}(""{2}""); s{0}.SetValue(""{3}/label"", $l{0}.Text); s{0}.SetValue(""{3}/command"", ""engine connect $c{0}.Text; password $p{0}.Text;""); $c{0} s{0}.Save();", LastControlID, ScriptType._Source1Res.Name, "resource/GameMenu.res", args[0]));
                                }

                                applyCtrlStyle(CtrlStyle, group);

                                Container.AddChild(group);
                            }
                            break;
                            #endregion
                        case "GoatControl":
                            //#region GoatControl
                            //GoatControl g = new GoatControl(BaseFolder, CtrlStyle.Name, CtrlStyle.Default);
                            //g.Width = ControlWidth;
                            //applyCtrlStyle(CtrlStyle, g);
                            //flowControl.Controls.Add(g);
                            break;
                        //#endregion
                        case "GlobalComboBox":
                            #region GlobalComboBox
                            {
                                if (args.Count != 2)
                                {
                                    //log("GlobalComboBox", "GlobalComboBox needs 2 arguments!", 1);
                                    break;
                                }
                                if (multilines.Count == 0)
                                    break;
                                //log("GlobalComboBox", "No multiline arguments", 0);

                                SGroup group = new SGroup();
                                group.Parent = Container;
                                group.Window = this;
                                group.Text = args[1];

                                SComboBox combo = new SComboBox();
                                //combo.Parent = Container;
                                //combo.Window = this;

                                combo.ID = args[0];

                                combo.Items = string.Join(",", multilines.Select(ss => ss.Replace(",", "\\,")));

                                applyCtrlStyle(CtrlStyle, combo);
                                group.AddChild(combo);
                                Container.AddChild(group);
                            }
                            break;
                            #endregion
                        case "GlobalCheckBox":
                            #region GlobalCheckBox
                            {
                                if (args.Count != 3)
                                {
                                    //log("GlobalCheckBox", "GlobalCheckBox needs 3 arguments!", 1);
                                    break;
                                }
                                SGroup group = new SGroup();
                                group.Parent = Container;
                                group.Window = this;
                                group.Text = args[1];


                                SCheckBox check = new SCheckBox();
                                //check.Parent = Container;
                                //check.Window = this;a

                                check.ID = args[0];

                                check.Text = args[2];

                                applyCtrlStyle(CtrlStyle, check);
                                group.AddChild(check);
                                Container.AddChild(group);
                            }
                            break;
                        #endregion
                    }

                    //CtrlStyle = new ctrlStyle();
                }
            }

            Container.Layout(479);
            Container.MinHeight = Container.Height;

            Script = new Scripting.Script(this, script.ToString(), folder, new Coordinate(0,0));

            Size = new Xwt.Size(530, 600);
        }


        // More legacy shit
        class ctrlStyle
        {
            public int? Portion = null;

            public string Name = null;
            public string Default = null;
        }

        string BaseFolder;
        FVersion ApiVersion = FVersion.TryParse("1.0");
        Boolean debug = false;
        Settings settings = new Settings();
        Settings GlobalSettings = new Settings();
        private string settingsPath;

        List<ConditionItem> ifs = new List<ConditionItem>();


        public ConditionItem ParseConditionalItem(List<string> lines)
        {
            string[] c = lines[0].Split(new Char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            ConditionItem C = new ConditionItem();
            bool copy = true;

            if (c[0] == "#if")
                C.ConditionType = ConditionType.If;
            else if (c[0] == "#elseif")
                C.ConditionType = ConditionType.ElseIf;
            else if (c[0] == "#else")
                C.ConditionType = ConditionType.Else;
            else
                copy = false;

            if (C.ConditionType != ConditionType.Else && c.Length != 2)
                copy = false;

            if (c.Length == 2)
                C.Condition = c[1];

            for (int i = 1; i < lines.Count; ++i)
            {
                if (lines.Count > i)
                {
                    if (lines[i].StartsWith("#"))
                    {
                        int layer = 0;
                        List<string> S = new List<string>();
                        while (lines.Count > i)
                        {
                            S.Add(lines[i].Trim());

                            if (lines[i] == "{")
                            {
                                layer++;
                                i++; continue;
                            }
                            else if (lines[i] == "}")
                            {
                                layer--;
                                if (layer == 0)
                                    break;
                                i++;

                                continue;
                            }
                            i++;
                        }
                        ConditionItem A = ParseConditionalItem(S);
                        if (A != null)
                            C.ConditionItems.Add(A);
                    }
                    else
                    {
                        C.Lines.Add(lines[i]);
                    }
                }
                else
                {
                    //log(lines[i], "Invalid syntax");
                }
            }
            if (copy)
                return C;
            else
                return null;
        }

        private bool CheckFile(string filename)
        {
            if (filename == null)
                return false;
            //if (filename.Contains(".."))
            //{
            //    //log("File", "Filename \"{0}\" conatins \"..\"", 1, filename);
            //    return false;
            //}
            return true;
        }

        private ctrlStyle parseCtrlStyle(string text, ctrlStyle c)
        {
            try
            {
                string[] S = text.Trim().TrimStart('[').TrimEnd(']').Split(',');
                foreach (string b in S)
                {
                    string s = b.Trim();
                    try
                    {
                        string v = s.Substring(s.IndexOf(':') + 1);
                        switch (s.Remove(s.IndexOf(':')))
                        {
                            //case "Border":
                            //    if (v == "1")
                            //        c.BorderStyle = BorderStyle.FixedSingle;
                            //    else if (v == "2")
                            //        c.BorderStyle = BorderStyle.Fixed3D;
                            //    else
                            //        c.BorderStyle = BorderStyle.None;
                            //    break;
                            //case "FG":
                            //    if (v.StartsWith("#"))
                            //        c.FG = ColorTranslator.FromHtml(v);
                            //    else if (v.Contains(";"))
                            //    {
                            //        string[] C = v.Split(';');
                            //        c.FG = Color.FromArgb(int.Parse(C[0]), int.Parse(C[1]), int.Parse(C[2]));
                            //    }
                            //    break;
                            //case "BG":
                            //    if (v.StartsWith("#"))
                            //        c.BG = ColorTranslator.FromHtml(v);
                            //    else if (v.Contains(";"))
                            //    {
                            //        string[] C = v.Split(';');
                            //        c.BG = Color.FromArgb(int.Parse(C[0]), int.Parse(C[1]), int.Parse(C[2]));
                            //    }
                            //    break;
                            case "Portion":
                                int p;
                                if (int.TryParse(v, out p))
                                    c.Portion = p;
                                break;
                            case "Name":
                                c.Name = v.Trim();
                                break;
                            case "Default":
                                c.Default = v.Trim();
                                break;
                            default:
                                //log("Control Style Parse", "The parameter \"{0}\" with the value \"{1}\" is unknown.", 3, s.Remove(s.IndexOf(':')), v);
                                break;
                        }
                    }
                    catch (Exception exc) { /*log("Parsing {0}", exc.Message, 1, s);*/ }
                }
            }
            catch (Exception exc) { /*log("Parsing Style Data", exc.Message, 1);*/ }
            return c;
        }

        private void applyCtrlStyle(ctrlStyle s, SChild c)
        {
            //if (s.BorderStyle != null)
            //{
            //    if (c is PictureBox)
            //        ((PictureBox)c).BorderStyle = (BorderStyle)s.BorderStyle;
            //    else if (c is UserControl)
            //        ((UserControl)c).BorderStyle = (BorderStyle)s.BorderStyle;
            //}
            //
            //if (s.FG != null)
            //    c.ForeColor = (Color)s.FG;
            //if (s.BG != null)
            //    c.BackColor = (Color)s.BG;
            if (s.Portion != null)
                c.WidthPercentage = 1d / s.Portion.Value;
            //c.ID = s.Name;
            //    c.Width = c.Width / (int)s.Portion - ((c.Margin.Left + c.Margin.Right) / (int)s.Portion);
        }

        bool ParseCondition(string s)
        {
            if (s.Contains("=="))
            {
                string[] S = s.Split(new string[] { "==" }, 2, StringSplitOptions.None);
                S[0] = S[0].Trim();
                S[1] = S[1].Trim();
                if (S[0].Contains("\""))
                    S[0] = S[0].Trim('\"');
                else
                    S[0] = GlobalSettings[S[0]];

                if (S[1].Contains("\""))
                    S[1] = S[1].Trim('\"');
                else
                    S[1] = GlobalSettings[S[1]];

                return string.Equals(S[0], S[1], StringComparison.OrdinalIgnoreCase);
            }
            else if (s.Contains("!="))
            {
                string[] S = s.Split(new string[] { "!=" }, 2, StringSplitOptions.None);
                S[0] = S[0].Trim();
                S[1] = S[1].Trim();
                if (S[0].Contains("\""))
                    S[0] = S[0].Trim('\"');
                else
                    S[0] = GlobalSettings[S[0]];

                if (S[1].Contains("\""))
                    S[1] = S[1].Trim('\"');
                else
                    S[1] = GlobalSettings[S[1]];

                return !string.Equals(S[0], S[1], StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        void ExecuteConditionalItem(List<ConditionItem> C)
        {
            bool trueBefore = false;
            for (int i = 0; i < C.Count; ++i)
            {
                if (C[i].ConditionType != ConditionType.If)
                {
                    if (trueBefore)
                        continue;
                }
                if (C[i].ConditionType == ConditionType.Else || ParseCondition(C[i].Condition))
                {
                    trueBefore = true;
                    foreach (string s in C[i].Lines)
                        ExecuteCommand(s);
                    if (C[i].ConditionItems.Count != 0)
                        ExecuteConditionalItem(C[i].ConditionItems);
                }
                else
                {
                    trueBefore = false;
                    continue;
                }
            }
        }

        void ExecuteCommand(string A)
        {
            if (A.Contains("("))
            {
                try
                {
                    string type = A.Remove(A.IndexOf('(')).Trim();
                    List<string> args = new List<string>();
                    List<string> linesEx = new List<string>();
                    List<string> multilines = new List<string>();

                    string S = A.Substring(A.IndexOf('(') + 1).Trim();
                    string s = "";
                    bool inQuotes = false;
                    bool inArg = false;

                    #region parse line
                    for (int q = 0; q < S.Length; ++q)
                    {
                        if (inQuotes)
                        {
                            if (S[q] == '"')
                            {
                                inQuotes = false;
                                inArg = false;
                                args.Add(s);
                                s = "";
                            }
                            else
                            {
                                s += S[q];
                            }
                        }
                        else
                        {
                            if (inArg)
                            {
                                if (S[q] == ' ')
                                    continue;
                                else if (S[q] == ')')
                                {

                                }
                                else if (S[q] == ',')
                                {
                                    args.Add(s);
                                    s = "";
                                    inArg = false;
                                }
                                else
                                    s += S[q];
                            }
                            else
                            {
                                if (S[q] == ' ')
                                    continue;
                                else if (S[q] == ',')
                                {
                                    if (s != "")
                                        args.Add(s);
                                    s = "";
                                }
                                else if (S[q] == ')')
                                {
                                    if (S.Length - 1 != q && S[q + 1] == ';')
                                    {
                                        if (s != "")
                                            args.Add(s);
                                        break;
                                    }
                                }
                                else if (S[q] == '"')
                                    inQuotes = true;
                                else
                                    s += S[q];
                            }
                        }
                    }
                    #endregion parse line

                    switch (type)
                    {
                        case "Copy":
                            try
                            {
                                File.Copy(Path.Combine(BaseFolder, ParsePath(args[0])), Path.Combine(BaseFolder, ParsePath(args[1])), true);
                            }
                            catch { }
                            break;
                    }
                }
                catch { }
            }
        }

        public static string ParsePath(string path)
        {
            return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }

        public class ConditionItem
        {
            public ConditionType ConditionType;
            public string Condition;
            public List<string> Lines;
            public List<ConditionItem> ConditionItems;

            public ConditionItem()
            {
                ConditionType = ConditionType.If;
                Condition = "";
                Lines = new List<string>();
                ConditionItems = new List<ConditionItem>();
            }

            public ConditionItem(ConditionType conditionType, string condition, IEnumerable<string> lines)
            {
                ConditionType = conditionType;
                Condition = condition;
                Lines = (List<string>)lines;
            }
        }

        public enum ConditionType
        {
            If, ElseIf, Else
        }

        public class Settings : IEnumerable<SettingsItem>
        {
            public delegate void SettingsChangedHandler(SettingsItem item);
            public event SettingsChangedHandler SettingChanged;

            private SettingsItem[] arr = new SettingsItem[0];

            public string SavePath = null;

            public static SettingsItem Empty = new SettingsItem(null, null);

            //LOAD
            public void LoadFrom(string path)
            {
                SavePath = path;
                if (File.Exists(path))
                {
                    Console.WriteLine("Loading settings from \"{0}\"", path);
                    Load(File.ReadAllText(path).Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                }
                else
                {
                    Console.WriteLine("Settings not loaded!");
                }
            }

            public void LoadRaw(string data)
            {
                if (data.Length < 3)
                    return;

                Load(data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            }

            public void Load(params string[] data)
            {
                if (data.Length == 0)
                    return;
                SettingsItem[] C = new SettingsItem[arr.Length + data.Length];
                int CIndex = arr.Length;

                for (int a = 0; a < data.Length; a++)
                {
                    int index = data[a].IndexOf('=', 1);
                    if (index == -1 || data[a].Length - index == 0)
                        continue;
                    string key = data[a].Remove(index);
                    string value = data[a].Substring(index + 1);

                    int hash = key.GetHashCode();
                    for (int i = 0; i < CIndex; i++)
                    {
                        if (C[i].GetHashCode() == hash && C[i].Key == key)
                        {
                            if (C[i].Value != value)
                            {
                                C[i].Value = value;
                                if (SettingChanged != null)
                                    SettingChanged(C[i]);
                            }
                            break;
                        }
                    }
                    C[CIndex] = new SettingsItem(key, value);
                    if (SettingChanged != null)
                        SettingChanged(C[CIndex]);
                    CIndex++;
                }
                if (CIndex == C.Length)
                {
                    arr = C;
                }
                else
                {
                    arr = new SettingsItem[CIndex];
                    Array.Copy(C, 0, arr, 0, CIndex);
                }
            }

            //SAVE
            public string GetRawData()
            {
                string s = "";

                for (int i = 0; i < arr.Length; i++)
                    s += arr[i].Key + "=" + arr[i].Value + "\r\n";

                return s;
            }

            public void SaveTo(string path)
            {
                File.WriteAllText(path, GetRawData());
            }

            public void Save()
            {
                SaveTo(SavePath);
            }

            //REMOVE
            public void Remove(string key)
            {
                int hash = key.GetHashCode();
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].GetHashCode() == hash && arr[i].Key == key)
                    {
                        SettingsItem[] C = new SettingsItem[arr.Length - 1];
                        Array.Copy(arr, 0, C, 0, i);
                        Array.Copy(arr, i + 1, C, i, arr.Length - i - 1);
                        arr = C;
                        return;
                    }
                }
            }

            //CLEAR
            public void Clear()
            {
                arr = new SettingsItem[0];
            }

            //EXISTS
            public bool Exists(string key)
            {
                int hash = key.GetHashCode();
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].GetHashCode() == hash && arr[i].Key == key)
                        return true;
                }
                return false;
            }

            //GET
            public SettingsItem this[string key]
            {
                get
                {
                    int hash = key.GetHashCode();
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr[i].GetHashCode() == hash && arr[i].Key == key)
                            return arr[i];
                    }
                    return SettingsItem.Empty;
                }
            }

            //public bool TryGet(string key, out SettingsItem item)
            //{
            //    int hash = key.GetHashCode();
            //    for (int i = 0; i < arr.Length; i++)
            //    {
            //        if (arr[i].GetHashCode() == hash && arr[i].Key == key)
            //        {
            //            item = arr[i];
            //            return true;
            //        }
            //    }
            //    item = SettingsItem.Empty;
            //    return false;
            //}

            public bool TryGet(string key, out SettingsItem item)
            {
                int hash = key.GetHashCode();
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].GetHashCode() == hash && arr[i].Key == key)
                    {
                        item = arr[i];
                        return true;
                    }
                }
                item = Empty;
                return false;
            }

            //SET
            private void Add(SettingsItem item)
            {
                SettingsItem[] C = new SettingsItem[arr.Length + 1];
                arr.CopyTo(C, 0);
                C[C.Length - 1] = item;
                arr = C;
                if (SettingChanged != null)
                    SettingChanged(item);
            }

            public void Set(string key, string value)
            {
                int hash = key.GetHashCode();
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i].GetHashCode() == hash && arr[i].Key == key)
                    {
                        if (arr[i].Value != value)
                        {
                            arr[i].Value = value;
                            if (SettingChanged != null)
                                SettingChanged(arr[i]);
                        }
                        return;
                    }
                }

                Add(new SettingsItem(key, value));
            }

            public void Set(string key, int value)
            {
                Set(key, value.ToString());
            }

            public void Set(string key, bool value)
            {
                Set(key, value.ToString());
            }

            public void Set(string key, IEnumerable<string> value)
            {
                string val = "";
                foreach (string s in value)
                    val += s + "|";

                Set(key, val);
            }

            public void Set(string key, DateTime value)
            {
                Set(key, value.ToString());
            }

            //public void Set(string key, Color value)
            //{
            //    Set(key, value.A + "|" + value.R + "|" + value.G + "|" + value.B);
            //}

            public IEnumerator<SettingsItem> GetEnumerator()
            {
                return ((IEnumerable<SettingsItem>)arr).GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return arr.GetEnumerator();
            }
        }

        public struct SettingsItem
        {
            private string key;
            public string Key
            {
                get { return key; }
                private set { key = value; }
            }

            public string Value;
            private int hash;

            public static implicit operator string (SettingsItem item)
            {
                return item.Value;
            }

            public SettingsItem(string key, string value)
            {
                this.key = key;
                hash = key == null ? 0 : key.GetHashCode();
                Value = value;
            }

            public static SettingsItem Empty
            {
                get
                {
                    return new SettingsItem(null, null);
                }
            }

            private static readonly string[] listSeperators = new string[] { "|" };

            public Boolean ToBool()
            {
                return Value.ToUpperInvariant() == "TRUE";
            }

            public Int32 ToInt32()
            {
                int r = 0;
                for (int i = 0; i < Value.Length; i++)
                    r = 10 * r + (Value[i] - 48);
                return r;
            }

            public Int64 ToInt64()
            {
                long r = 0;
                for (int i = 0; i < Value.Length; i++)
                    r = 10 * r + (Value[i] - 48);
                return r;
            }

            public DateTime ToDateTime()
            {
                DateTime d;
                DateTime.TryParse(Value, out d);
                return d;
            }

            public string[] ToStringArray()
            {
                return Value.Split(listSeperators, StringSplitOptions.RemoveEmptyEntries);
            }

            public List<string> ToStringList()
            {
                return new List<string>(ToStringArray());
            }

            public bool IsEmpty()
            {
                return Value == null;
            }

            public bool IsExistingFile()
            {
                return File.Exists(Value);
            }

            public bool IsExistingDirectory()
            {
                try
                {
                    return Directory.Exists(Value);
                }
                catch
                {
                    return false;
                }
            }

            //public Color ToColor()
            //{
            //    String[] S = ToStringArray();
            //    if (S.Length != 4)
            //        return Color.Black;
            //    return Color.FromArgb(cInt(S[0]), cInt(S[1]), cInt(S[2]), cInt(S[3]));
            //}
            //private Int32 cInt(string s)
            //{
            //    int r = 0;
            //    for (int i = 0; i < s.Length; i++)
            //        r = 10 * r + (s[i] - 48);
            //    return Math.Max(Math.Min(r, 255), 0);
            //}


            public override string ToString()
            {
                return Value;
            }

            public override int GetHashCode()
            {
                return hash;
            }
        }

    }
}
