using FPlug.Options.Controls;
using FPlug.Options.IO;
using FPlug.Options.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xwt;

namespace FPlug.Options
{
    public class SettingsWindow : Window
    {
        public string WorkingDirectory { get; private set; }
        public string XmlPath { get; private set; }

        public bool EditMode { get; private set; }

        public bool UseTabs { get; private set; } = false;

        public bool DrawRedDebugOutline { get; set; } = false;

        public FolderCache Folder { get; private set; }

        //  Errors
        DataField<int> errorIndex = new DataField<int>();
        DataField<string> errorField = new DataField<string>();
        DataField<string> errorLevelField = new DataField<string>();

        ListStore errorListStore;
        ListView errorListView;

        int currentError = 1;

        double containerWidth = 800;
        double containerHeight = 550;

        public void LogError(string text, ErrorType level)
        {
            var index = errorListStore.AddRow();
            errorListStore.SetValue(index, errorIndex, currentError++);
            errorListStore.SetValue(index, errorField, text);
            errorListStore.SetValue(index, errorLevelField, level.ToString());
        }

        public void LogError(string text, Coordinate coords, ErrorType level)
        {
            var index = errorListStore.AddRow();
            errorListStore.SetValue(index, errorIndex, currentError++); //.ToString().PadLeft(3, '0')
            errorListStore.SetValue(index, errorField, "Line " + coords.Line + ", Char " + coords.Char + Environment.NewLine + text);
            errorListStore.SetValue(index, errorLevelField, level.ToString());
        }

        public void ClearLogs()
        {
            errorListStore.Clear();
            currentError = 0;
        }


        //  Window
        VBox ContainerBox;
        IContainer ControlsContainer;


        //  Controls
        Dictionary<string, IControl> ControlsByID = new Dictionary<string, IControl>();
        List<IControl> AllControls = new List<IControl>();

        public IControl GetControlByID(string id)
        {
            IControl c;
            if (ControlsByID.TryGetValue(id, out c))
                return c;
            else
                return null;
        }


        //  CTOR
        public SettingsWindow(string xmlPath, string workingDirectory, bool editMode)
        {
            Script.InitializeOptionsTask?.Wait();


            WorkingDirectory = workingDirectory;
            XmlPath = xmlPath;
            EditMode = editMode;

            Folder = new FolderCache(workingDirectory);


            // Errors
            errorListStore = new ListStore(errorIndex, errorLevelField, errorField /*, xField, yField*/);
            errorListView = new ListView(errorListStore);

            errorListView.Columns.Add("#", errorIndex);
            errorListView.Columns.Add("Type", errorLevelField);
            errorListView.Columns.Add("Message", errorField);

            errorListView.HeightRequest = 200;


            // Window
            Size = new Size(930, 200);

            // buttons
            HBox buttonsBox = new HBox();
            Button applyButton = new Button(" Apply ");
            applyButton.Clicked += (s, e) =>
            {

            };

            buttonsBox.PackEnd(applyButton);
            Button reloadButton = new Button(" Reload ");
            reloadButton.Clicked += (s, e) => { Reload(); };
            buttonsBox.PackEnd(reloadButton);

            //CheckBox autoReloadCheck = new CheckBox("Autoreload xml on change");
            //autoReloadCheck.Toggled += (s, e) => {  };

            CheckBox outlineCheck = new CheckBox("Outline")
            {
                Active = DrawRedDebugOutline
            };
            outlineCheck.Toggled += (s, e) => { DrawRedDebugOutline = outlineCheck.Active; AllControls.Do(k => (k as Control)?.QueueDraw()); };
            buttonsBox.PackStart(outlineCheck);

            // container
            ContainerBox = new VBox();
            ContainerBox.HeightRequest = containerHeight;

            // content
            VBox content = new VBox();
            content.PackStart(ContainerBox);
            content.PackStart(buttonsBox);
            if (EditMode)
                content.PackEnd(errorListView);
            Content = content;

            Reload();
        }


        //  (re)load xml
        public void Reload()
        {
            AllControls.Clear();
            ControlsByID.Clear();
            ContainerBox.Clear();
            errorListStore.Clear();
            currentError = 1;

            GC.Collect();

            UseTabs = false;

            // Load XML
            XDocument xml = XDocument.Load(XmlPath, LoadOptions.SetLineInfo);

            // root
            xml.Element("mod").Process(mod =>
            {
                mod.Element("options").Process(options =>
                {
                    // window
                    options.Element("window").Process(window =>
                    {
                        window.Attribute("tabs").Process(tabs =>
                        {
                            if (tabs.Value.ToUpper() == "TRUE")
                                UseTabs = true;
                        });

                        // add container
                        if (UseTabs)
                            ContainerBox.PackStart((TabContainer)(ControlsContainer = new TabContainer() { HeightRequest = containerHeight }));
                        else
                            ContainerBox.PackStart(new ScrollView() { HeightRequest = containerHeight, Content = (Container)(ControlsContainer = new Container()) });

                        AllControls.Add((IControl)ControlsContainer);
                        ControlsByID["window"] = (IControl)ControlsContainer;

                        ControlsContainer.Window = this;

                        ReadControls(window, ControlsContainer, tabRequired: UseTabs);
                        PerformLayout();
                    });

                    // script
                    options.Element("script").Process(window =>
                    {

                    });
                });
            });
        }

        public void ReadControls(XElement element, IContainer container, bool tabRequired = false)
        {
            element.Elements().Do(e =>
            {
                // get name
                var name = e.Name.LocalName;

                // check if tab required
                if (tabRequired && name.ToUpper() != "TAB")
                {
                    LogError($"All controls inside <window> need to be <tab> when \"UseTabs\" is true", getCoordinate(e), ErrorType.Xml);
                    return;
                }

                // get text offset
                Coordinate textOffset;

                // try to find control type
                ControlType type = Control.AllControls.FirstOrDefault(c => c.Name == name);

                if (type == null) // control unknown
                {
                    LogError($"Invalid control type \"{name}\".", getCoordinate(e), ErrorType.Xml);
                    return;
                }

                Control control = type.CreateInstance();

                // Set Properties
                e.Attributes().Do(attribute =>
                {
                    Property prop;
                    if (type.Properties.TryGetValue(attribute.Name.LocalName, out prop))
                    {
                        Variable v = new Variable();
                        v.SetValue(attribute.Value);
                        prop.SetVariable(control, v);
                    }
                    else
                    {
                        LogError($"Unknown property \"{attribute.Name}\" in <{name}>.", getCoordinate(e), ErrorType.Xml);
                    }
                });

                container.AddControl(control);

                // container
                if (control is Container)
                {
                    ReadControls(e, (Container)control);
                }
                else // children of control
                {
                    e.Elements().FirstOrDefault().Process(_e => LogError($"Warning: \"{name}\" is not a container control. All children will be ignored. ", ErrorType.Xml));
                }

                // id
                if (control.ID != null)
                    if (ControlsByID.ContainsKey(control.ID))
                    {
                        LogError($"Warning: There already is a control with the id \"{control.ID}\".", getCoordinate(e), ErrorType.Xml);
                    }
                    else
                    {
                        ControlsByID[control.ID] = control;
                    }
                AllControls.Add(control);
            });
        }


        //  process layout for container
        public void PerformLayout()
        {
            ControlsContainer.PerformLayout(containerWidth);
        }


        //  helpers
        Coordinate getCoordinate(XElement element)
        {
            if (((IXmlLineInfo)element).HasLineInfo())
                return new Coordinate(((IXmlLineInfo)element).LinePosition, ((IXmlLineInfo)element).LineNumber);
            else
                return new Coordinate(0, 0);
        }
    }
}
