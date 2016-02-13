using FPlug.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace FPlug
{
    public class MainWindow : Window
    {
        ScrollView scrollView;
        PluginWidgetLayout flowControl;

        public MainWindow()
        {
            var w = System.Diagnostics.Stopwatch.StartNew();
            Title = "4Plug v2 beta";

            Width = 870;
            Height = 550;

            Padding = new WidgetSpacing();

            //Menu
            //Menu mainMenu = new Menu();
            //var m = new MenuItem("Close");
            //Menu subMenu = new Menu();
            //subMenu.Items.Add(new MenuItem("HAH"));
            //m.SubMenu = subMenu;
            //mainMenu.Items.Add(m);
            //this.MainMenu = mainMenu;

            //Flow Control
            flowControl = new PluginWidgetLayout();
            flowControl.Padding = new WidgetSpacing(16, 0, 8, 16);

            flowControl.AddChild(new PluginTitleWidget(PluginType.Hud));
            flowControl.AddChild(new PluginTitleWidget(PluginType.Unknown));
            flowControl.AddChild(new PluginTitleWidget(PluginType.Vpk));
            flowControl.AddChild(new PluginTitleWidget(PluginType.Hitsound));
            flowControl.AddChild(new PluginTitleWidget(PluginType.Addon));

            //flowControl.AddChild(new AddPluginWidget(PluginType.Hud));
            ////flowControl.AddChild(new AddPluginWidget(PluginType.Unknown));
            //flowControl.AddChild(new AddPluginWidget(PluginType.Vpk));
            //flowControl.AddChild(new AddPluginWidget(PluginType.Hitsound));
            //flowControl.AddChild(new AddPluginWidget(PluginType.Addon));

            scrollView = new ScrollView();
            scrollView.Content = flowControl;
            flowControl.BackgroundColor = Color.FromBytes(240, 240, 240);
            Content = scrollView;

            flowControl.layout = false;
            LoadPlugins();
            flowControl.layout = true;
            flowControl.Layout();

            //new Popover() { Content = new Label("eyyy leute") }.Show(Popover.Position.Top, scrollView);

            //Finish
            CloseRequested += HandleCloseRequested;
            w.Stop();
            Console.WriteLine("MainWindow..ctor(): " + w.ElapsedMilliseconds + "ms");
        }

        public void AddDirectoryPlugin(string path, bool installed)
        {
            var p = new DirectoryPluginWidget(path);
            p.MainWindow = this;
            p.Installed = installed;
            flowControl.AddChild(p);
        }

        public void RemovePlugin(PluginWidget plugin)
        {
            flowControl.RemoveChild(plugin);
        }

        public void LoadPlugins()
        {
            var w = System.Diagnostics.Stopwatch.StartNew();
            int pluginCount = 0;
            foreach (string s in Directory.GetDirectories(App.PathCustom))
            {
                AddDirectoryPlugin(s, true);
                pluginCount++;
            }

            foreach (string s in Directory.GetDirectories(App.PathCustom_))
            {
                AddDirectoryPlugin(s, false);
                pluginCount++;
            }
            w.Stop();
            Console.WriteLine("Loaded " + pluginCount + " Plugins in " + w.ElapsedMilliseconds + "ms");
        }

        void HandleCloseRequested(object sender, CloseRequestedEventArgs args)
        {
            args.AllowClose = true;
            if (args.AllowClose)
                Application.Exit();
        }
    }
}
