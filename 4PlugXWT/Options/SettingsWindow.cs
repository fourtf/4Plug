using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Options
{
    public partial class SettingsWindow : Dialog
    {
        // Variables
        //ScrollView ScrollContainer;
        public new SGroup Container;
        //SettingsWidgetTreeview treeview;

        public Script Script { get; set; }

        public string XmlPath { get; private set; }
        public string Folder { get; private set; }
        public bool EditMode { get; private set; }

        public Timer xmlUpdateTimer;
        public DateTime lastChangedTime;

        public bool LegacyMode { get; private set; }

        public FolderCache FolderCache { get; set; }

        bool initialized = false;

        DataField<int> errorIndex = new DataField<int>();
        DataField<string> errorField = new DataField<string>();
        DataField<string> errorLevelField = new DataField<string>();

        ListStore errorListStore;
        ListView errorListView;

        int currentindex = 0;

        public void LogError(string text, ErrorType level)
        {
            var index = errorListStore.AddRow();
            errorListStore.SetValue(index, errorIndex, currentindex++);
            errorListStore.SetValue(index, errorField, text);
            errorListStore.SetValue(index, errorLevelField, level.ToString());
        }

        public void LogError(string text, int line, int column, ErrorType level)
        {
            var index = errorListStore.AddRow();
            errorListStore.SetValue(index, errorIndex, currentindex++); //.ToString().PadLeft(3, '0')
            errorListStore.SetValue(index, errorField, "Line " + line + ", Char " + column + Environment.NewLine + text);
            errorListStore.SetValue(index, errorLevelField, level.ToString());
        }

        public void ClearLogs()
        {
            errorListStore.Clear();
            currentindex = 0;
        }


        // CTOR
        public SettingsWindow(string xmlPath, string folder, bool editMode)
        {
            App.InitializeOptionsTask.Wait();

            Icon = App.Icon;

            Title = folder;

            XmlPath = xmlPath;
            Folder = folder;
            EditMode = editMode;

            FolderCache = new FolderCache(folder);

            Size = new Size(930, /*editMode ? 750 : */200);

            // Errors
            errorListStore = new ListStore(errorIndex, errorLevelField, errorField /*, xField, yField*/);
            errorListView = new ListView(errorListStore);

            errorListView.Columns.Add("#", errorIndex);
            errorListView.Columns.Add("Type", errorLevelField);
            errorListView.Columns.Add("Message", errorField);

            errorListView.HeightRequest = 200;
            // End Errors

            //TempSources = new Dictionary<int, object>();
            Resources = new Dictionary<string, Tuple<SourceType, object>>();
            //DisposeResource = new Dictionary<SourceType, Action<object>>();

            //ErrorWindow = new Scripting.ErrorWindow();

            if (editMode)
            {
                //try
                //{
                //    Point? p;
                //    if (XSettings.SettingsLastPosition != null && App.TryParsePoint(XSettings.SettingsLastPosition.Value, out p) && p != null)
                //    {
                //        this.InitialLocation = WindowLocation.Manual;
                //        this.Location = p.Value;
                //    }
                //    if (XSettings.SettingsLastErrorPosition != null && App.TryParsePoint(XSettings.SettingsLastErrorPosition.Value, out p) && p != null)
                //    {
                //        ErrorWindow.InitialLocation = WindowLocation.Manual;
                //        ErrorWindow.Location = p.Value;
                //    }
                //}
                //catch { }
            }

            VBox vbox = new VBox();
            //Content = vbox;

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

                execApply = (c) => c.children.Do((w) => { w.RenewSources(); if (w is SContainer) execApply((SContainer)w); });
                execApply(Container);

#warning Register source types
                DisposeControlResources();
            };

            hbox.PackEnd(applyButton);
            vbox.PackStart(Container = new SBox());
            Container.Window = this;
            vbox.PackEnd(hbox);
            vbox.HeightRequest = 450;

            Button reloadButton = new Button(" Reload ");
            reloadButton.Clicked += (s, e) => { Reload(); };
            hbox.PackEnd(reloadButton);

            Reload();

            lastChangedTime = new FileInfo(xmlPath).LastWriteTime;

            xmlUpdateTimer = new Timer(1000);
            xmlUpdateTimer.Elapsed += (s, e) =>
            {
                try
                {
                    DateTime time = new FileInfo(XmlPath).LastWriteTime;
                    if (File.Exists(XmlPath))
                    {
                        if (lastChangedTime != time)
                        {
                            Application.Invoke(() => { Reload(); });
                        }
                    }
                    Application.Invoke(() => { lastChangedTime = time; });
                }
                catch
                {

                }
            };
            xmlUpdateTimer.Enabled = true;

            initialized = true;

            VBox box = new VBox();
            box.PackStart(vbox);
            if (EditMode)
                box.PackEnd(errorListView);
            Content = box;

            Resizable = false;
        }


        // Events
        protected override void OnClosed()
        {
            //if (!LegacyMode)
            //{
            //    if (XSettings.SettingsLastPosition == null)
            //        XSettings.SettingsLastPosition = XSettings.Add(XSettings.Settings, "lastPosition");
            //    XSettings.SettingsLastPosition.Value = Location.X + "," + Location.Y;
            //
            //    if (XSettings.SettingsLastErrorPosition == null)
            //        XSettings.SettingsLastErrorPosition = XSettings.Add(XSettings.Settings, "lastErrorPosition");
            //    XSettings.SettingsLastErrorPosition.Value = ErrorWindow.Location.X + "," + ErrorWindow.Location.Y;
            //}

            base.OnClosed();
        }

        public void Reload()
        {
            Container.Clear();

            Script = null;
            ClearLogs();

            string scriptApply = null;
            Coordinate textOffset = Coordinate.Empty;

            // Xml
            try
            {
                string s = File.ReadAllText(XmlPath);

                StringReader reader = new StringReader(s);
                XDocument doc = XDocument.Load(reader, LoadOptions.SetLineInfo);
                var settings = doc.Element("mod").Element("options");

                // Window
                var xwin = settings.Element("window");
                if (xwin == null)
                    Console.WriteLine("-- xml does not contain Plugin/Settings/Window");
                else
                {
                    LoadXmlNode(xwin, Container);
                    Action<SContainer> compileAll = null;
                    compileAll = (c) => c.children.Do((w) =>
                    {
                        w.CompileScripts();
                        if (w is SContainer) compileAll((SContainer)w);
                    });
                    compileAll(Container);
                }

                if (!initialized)
                {
                    //XSettings
                }

                // Script
                var xapply = settings.Element("script");
                if (xapply != null)
                {
                    if (((IXmlLineInfo)xapply).HasLineInfo())
                        textOffset = new Coordinate(((IXmlLineInfo)xapply).LinePosition, ((IXmlLineInfo)xapply).LineNumber);
                    else
                        textOffset = new Coordinate(0, 0);
                    scriptApply = (string)xapply;
                }
            }
            catch (XmlException exc)
            {
                LogError("Malformed xml at line " + exc.LineNumber + ", column " + exc.LinePosition + ":" + Environment.NewLine + exc.Message, ErrorType.Xml);
            }

            // Script
            if (scriptApply != null)
            {
                Script = new Script(this, scriptApply, Folder, textOffset);
            }

            Layout();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);
        }


        public void LoadXmlNode(XElement xwin, SChild currentChild, ControlDesc currentDesc = null)
        {
            SContainer container = currentChild as SContainer;

            foreach (XElement x in xwin.Elements())
            {
                PropertyInfo propInfo;
                if (currentDesc != null && currentDesc.Events.TryGetValue(x.Name.LocalName, out propInfo))
                {
                    Coordinate textOffset;
                    if (((IXmlLineInfo)x).HasLineInfo())
                        textOffset = new Coordinate(((IXmlLineInfo)x).LinePosition, ((IXmlLineInfo)x).LineNumber);
                    else
                        textOffset = new Coordinate(0, 0);
                    Script script = new Scripting.Script(this, x.Value, Folder, textOffset, false);

                    script.RegisterVariable("this", currentChild, currentChild.ScriptType);
                    propInfo.SetValue(currentChild, script, null);
                }
                else if (container != null)
                {
                    var controlDesc = App.Controls.FirstOrDefault(c => c.Name == x.Name);
                    if (controlDesc == null)
                    {
                        Console.WriteLine("-- unknown control " + x.Name);
                        continue;
                    }

                    SChild child = null;
                    child = (SChild)Activator.CreateInstance(controlDesc.Type);

                    if (container.CanAddChild(child) == null)
                        container.AddChild(child);

                    foreach (XAttribute attribute in x.Attributes())
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(attribute.Value))
                            {
                                var name = attribute.Name.LocalName;

                                var prop = controlDesc.Properties.FirstOrDefault(desc => desc.Name == name);
                                if (prop != null && prop.Set != null)
                                {
                                    if (prop.ScriptType == ScriptType.String)
                                        prop.Set(child, (string)attribute);
                                    else if (prop.ScriptType == ScriptType.Number)
                                    {
                                        double d;
                                        if (Double.TryParse(attribute.Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                                            prop.Set(child, d);
                                        else
                                            LogError("Attribute \"" + attribute.Value + "\" is invalid at <" + x.Name + "." + name + ">", ErrorType.Xml);
                                    }
                                    else if (prop.ScriptType == ScriptType.Bool)
                                    {
                                        bool b;
                                        if (bool.TryParse(attribute.Value, out b))
                                            prop.Set(child, b);
                                        else
                                            LogError("Attribute \"" + attribute.Value + "\" is invalid at <" + x.Name + "." + name + ">", ErrorType.Xml);
                                    }
                                    else if (prop.ScriptType == ScriptType.Color)
                                    {
                                        Color c;
                                        if (App.TryParseColor((string)attribute, out c))
                                            prop.Set(child, c);
                                        else
                                            LogError("Attribute \"" + attribute.Value + "\" is invalid at <" + x.Name + "." + name + ">", ErrorType.Xml);
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                    LoadXmlNode(x, child, controlDesc);
                }

                //if (child is SContainer)
            }
        }

        public SChild GetChildByID(string ID)
        {
            return Container.GetChildByID(ID);
        }

        public void Layout()
        {
            Container.Layout(900);
            Container.MinHeight = Container.Height;
        }

        void DisposeControlResources()
        {
            foreach (var t in DisposeResource)
            {
                foreach (var kvp in Resources)
                {
                    if (kvp.Value.Item1 == t.Item1)
                    {
                        t.Item2(kvp.Value.Item2);
                    }
                }
            }
        }

        public Dictionary<string, Tuple<SourceType, object>> Resources { get; private set; }
        public static List<Tuple<SourceType, Action<object>>> DisposeResource
        { get; }
        =
            new List<Tuple<SourceType, Action<object>>> {
                Tuple.Create<SourceType, Action<object>>(SourceType.Source1Res, (res) => ((Source1ResourceFile)res).Save())
            };

        public List<SourceType> PendingActionTypes = new List<SourceType> { SourceType.SwitchFile, SourceType.Source1Res };
        public List<Tuple<SourceType, Action>> PendingActions = new List<Tuple<SourceType, Action>>();

        public void ExecPendingActions()
        {
            foreach (var type in PendingActionTypes)
            {
                foreach (var action in PendingActions)
                {
                    if (action.Item1 == type)
                    {
                        action.Item2();
                    }
                }
            }
            PendingActions.Clear();
        }

        //public Dictionary<int, object> TempSources { get; private set; }
    }

    public enum ErrorType
    {
        Compiler,
        Execution,
        Xml,
        Log,
    }

    /*
    <?xml version="1.0" encoding="UTF-8"?>
    <Plugin>
        <Meta>
            <Author>Sevin</Author>
            <Version>4.02</Version>
            <SteamID>76561198056804798</SteamID>
            <Links>
                <Link>https://github.com/Sevin7/7HUD</Link>
                <Link>http://steamcommunity.com/groups/7HUD</Link>
                <Link>http://teamfortress.tv/forum/thread/12745</Link>
                <Link>http://etf2l.org/forum/huds/topic-27677/</Link>
            </Links>
        </Meta>
        <!--<Settings>
            here would be the dialog code
        </Settings>-->
    </Plugin>
     * */
}
