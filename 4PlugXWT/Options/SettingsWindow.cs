using FPlug.Options.Controls;
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


        //  Errors
        DataField<int> errorIndex = new DataField<int>();
        DataField<string> errorField = new DataField<string>();
        DataField<string> errorLevelField = new DataField<string>();

        ListStore errorListStore;
        ListView errorListView;

        int currentindex = 0;

        double containerWidth = 600;
        double containerHeight = 450;

        public void LogError(string text, ErrorType level)
        {
            var index = errorListStore.AddRow();
            errorListStore.SetValue(index, errorIndex, currentindex++);
            errorListStore.SetValue(index, errorField, text);
            errorListStore.SetValue(index, errorLevelField, level.ToString());
        }

        public void LogError(string text, Coordinate coords, ErrorType level)
        {
            var index = errorListStore.AddRow();
            errorListStore.SetValue(index, errorIndex, currentindex++); //.ToString().PadLeft(3, '0')
            errorListStore.SetValue(index, errorField, "Line " + coords.Line + ", Char " + coords.Char + Environment.NewLine + text);
            errorListStore.SetValue(index, errorLevelField, level.ToString());
        }

        public void ClearLogs()
        {
            errorListStore.Clear();
            currentindex = 0;
        }


        //  Window
        VBox ContainerBox;
        IContainer Container;


        //  Controls
        Dictionary<string, Control> AllControls = new Dictionary<string, Control>();

        public Control GetControlByID(string id)
        {
            Control c;
            if (AllControls.TryGetValue(id, out c))
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

            // container
            ContainerBox = new VBox();
            ContainerBox.HeightRequest = 450;

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
                            ContainerBox.PackStart((TabContainer)(Container = new TabContainer() { HeightRequest = containerHeight }));
                        else
                            ContainerBox.PackStart((Container)(Container = new Container()));
                        //Container.MinHeight = containerHeight;
                        //Container.MinWidth = containerWidth;
                        Container.Window = this;

                        ReadControls(window, Container, tabRequired: UseTabs);
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



                if (control is Container)
                {
                    ReadControls(e, (Container)control);
                }
                else // children of control
                {
                    e.Elements().FirstOrDefault().Process(_e => LogError($"Warning: \"{name}\" is not a container control. All children will be ignored. ", ErrorType.Xml));
                }

                container.AddControl(control);
            });
        }


        //  process layout for container
        public void PerformLayout()
        {
            Container.PerformLayout(containerWidth);
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
