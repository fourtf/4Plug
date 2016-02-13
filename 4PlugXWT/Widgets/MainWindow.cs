using FPlug.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class MainWindow : Window
    {
        ScrollView scrollView;
        PluginWidgetLayout layout;

        public PluginWidgetLayout Layout
        {
            get { return layout; }
        }

        public static Image imgSteam = Resources.GetImage("steam.png");
        public static Image imgTFTV = Resources.GetImage("tftv.png");
        public static Image imgRefresh = Resources.GetImage("refresh.png");
        public static Image imgGear = Resources.GetImage("gear.png");

        Menu gamesMenu;

        public List<Window> OpenWindows = new List<Window>();

        public bool CanClose
        {
            get
            {
                return Layout.Items.All(w => w.CanClose());
            }
        }

        public MenuItem CustomGameMenu { get; private set; }

        public MainWindow()
        {
            var w = System.Diagnostics.Stopwatch.StartNew();
            Title = App.WindowTitle;

            this.Icon = App.Icon;

            Width = 870;
            Height = 550;

            Padding = new WidgetSpacing();

            new Task(() => CheckUpdates()).Start();

            //Menu
            Menu mainMenu = new Menu();
            {
                {
                    var sub = new MenuItem("4Plug");

                    Menu subMenu = new Menu();
                    {
                        MenuItem m;

                        subMenu.Items.Add(m = new MenuItem("Reload Plugins") { Image = imgRefresh });
                        m.Clicked += (s, e) => { LoadPlugins(); };

                        subMenu.Items.Add(m = new SeparatorMenuItem());

                        subMenu.Items.Add(m = new MenuItem("Submit Feedback"));
                        m.Clicked += (s, e) => { new SubmitFeedbackWindow("via Main Window").Run(this); };

                        subMenu.Items.Add(m = new MenuItem("Program Folder"));
                        m.Clicked += (s, e) => { Xwt.Desktop.OpenFolder(Environment.CurrentDirectory); };

                        subMenu.Items.Add(m = new MenuItem("Exit"));
                        m.Clicked += (s, e) => { Close(); };
                    }

                    sub.SubMenu = subMenu;
                    mainMenu.Items.Add(sub);
                }

                {
                    var sub = new MenuItem("Games");

                    Menu subMenu = gamesMenu = new Menu();
                    {
                        MenuItem m;
                        subMenu.Items.Add(m = new MenuItem("Manage Games") { Image = imgGear });
                        m.Clicked += (s, e) => { ShowGameSelector(); };

                        subMenu.Items.Add(new SeparatorMenuItem());

                        //subMenu.Items.Add(m = new MenuItem("Team Fortress 2"));
                        //m.Clicked += (s, e) => { Xwt.Desktop.OpenUrl("http://steamcommunity.com/groups/4stuff"); };
                    }

                    sub.SubMenu = subMenu;
                    mainMenu.Items.Add(sub);
                }

                {
                    //var sub = new MenuItem("Current Game");
                    //
                    //CustomGameMenu = sub;
                    //
                    //mainMenu.Items.Add(sub);
                }

                {
                    var sub = new MenuItem("About");

                    Menu subMenu = new Menu();
                    {
                        MenuItem m;
                        subMenu.Items.Add(m = new MenuItem("Steam Group") { Image = imgSteam });
                        m.Clicked += (s, e) => { Xwt.Desktop.OpenUrl(App.SteamGroup); };
                        subMenu.Items.Add(m = new MenuItem("TeamFortress.TV Thread") { Image = imgTFTV });
                        m.Clicked += (s, e) => { Xwt.Desktop.OpenUrl(App.TfTvThread); };

                        subMenu.Items.Add(new SeparatorMenuItem());

                        subMenu.Items.Add(m = new MenuItem("Licenses"));
                        m.Clicked += (s, e) => { new LicensesWindow().Show(); };
                    }

                    sub.SubMenu = subMenu;
                    mainMenu.Items.Add(sub);
                }
            }
            this.MainMenu = mainMenu;

            //if (BackendHost.ToolkitEngine.Type == ToolkitType.Wpf)
            //    mainMenu.Items.Do(item => item.Label = item.Label.ToUpper());

            //Flow Control
            layout = new PluginWidgetLayout();
            layout.Padding = new WidgetSpacing(16, 0, 8, 16);

            scrollView = new ScrollView();
            scrollView.Content = layout;
            //layout.BackgroundColor = Color.FromBytes(240, 240, 240);
            layout.BackgroundColor = Colors.White;
            //layout.BackgroundColor = Color.FromBytes(64, 64, 64);
            Content = scrollView;

            //LoadPlugins();
            //ReloadGames();

            //new Popover() { Content = new Label("eyyy leute") }.Show(Popover.Position.Top, scrollView);

            //Finish
            //CloseRequested += HandleCloseRequested;
            w.Stop();
            Console.WriteLine("MainWindow..ctor(): " + w.ElapsedMilliseconds + "ms");

            if (App.BeforeWindowShown != null)
                App.BeforeWindowShown(BackendHost.Backend);
        }

        public void ScrollToTop()
        {
            scrollView.VerticalScrollControl.Value = 0;
        }

        private void CheckUpdates()
        {
            var client = new WebClient();
            client.Proxy = null;
            try
            {
                string s = client.DownloadString(new Uri(App.WinVersionUrl));

                FVersion version = FVersion.TryParse(s);
                if (version == null)
                    return;

                if (version > App.Version)
                {
                    //if (DialogResult.Yes == MessageBox.Show("Unfortunally autoupdates are not implemented for linux/mac yet.\r\n\r\nDo you want to open the tf.tv page in a browser?", "An update is ready!", MessageBoxButtons.YesNo))
                    //    Process.Start("http://teamfortress.tv/forum/thread/13401");

                    Application.Invoke(() =>
                        {
                            UpdateAvailableWidget w = new UpdateAvailableWidget(version);
                            App.MainWindow.Layout.AddChild(w);
                        });

                    //c.Restart += new UpdateControl.OnRestart(c_Restart);
                    //
                    //client.DownloadFile(new Uri(App.WinUpdateUrl), "_4Plug.zip");
                    //
                    //for (int i = 0; i < flowContainer.Controls.Count; ++i)
                    //{
                    //    if (flowContainer.Controls[i] is UpdateControl)
                    //    {
                    //        flowContainer.Invoke(new MethodInvoker(delegate
                    //        {
                    //            ((UpdateControl)flowContainer.Controls[i]).SetRestart(false);
                    //        }));
                    //        update = true;
                    //    }
                    //}
                }
            }
            catch { }
        }

        protected override void OnShown()
        {
            base.OnShown();

            if (App.InitMainWindow != null)
                App.InitMainWindow(this.BackendHost.Backend);
        }

        public void LoadPlugins()
        {
            if (CanClose)
            {
                if (App.CurrentGame.Type.LoadPlugins != null)
                    App.CurrentGame.Type.LoadPlugins();
            }
            else
                OpenWindows[0].Present();
        }

        public void ReloadGames()
        {
            gamesMenu.Items.Skip(2).ToList().ForEach(i => gamesMenu.Items.Remove(i));

            foreach (var g in App.Games)
            {
                var menuitem = new GameMenuItem(g) { Label = g.Type.LibraryName, Checked = g == App.CurrentGame };
                menuitem.Clicked += (s, e) =>
                {
                    if (((GameMenuItem)s).Game == App.CurrentGame)
                    {
                        ((GameMenuItem)s).Checked = true;
                    }
                    else
                    {
                        App.SetCurrentGame(((GameMenuItem)s).Game);
                        XSettings.Games.SetAttributeValue("selectedindex", gamesMenu.Items.IndexOf((MenuItem)s) - 2);
                        gamesMenu.Items.Skip(2).Cast<GameMenuItem>().Do(item => item.Checked = false);
                        ((GameMenuItem)s).Checked = true;
                    }
                };
                gamesMenu.Items.Add(menuitem);
            }
        }

        public void ShowGameSelector()
        {
            new AddGamesWindow().Run(this);
        }

        //protected override void OnBoundsChanged(BoundsChangedEventArgs a)
        //{
        //    base.OnBoundsChanged(a);
        //}

        public void AddDirectoryPlugin(string path, bool installed, bool animate = false)
        {
            var p = new DirectoryPluginWidget(path);
            p.MainWindow = this;
            p.Installed = installed;
            if (animate)
            {
                layout.EnableLayout = false;
                layout.AddChild(p);
                layout.EnableLayout = true;
                layout.Layout(true);
            }
            else
                layout.AddChild(p);
        }

        public void AddFilePlugin(string path, bool installed, bool animate = false)
        {
            var p = new FilePluginWidget(path);
            p.MainWindow = this;
            p.Installed = installed;
            if (animate)
            {
                layout.EnableLayout = false;
                layout.AddChild(p);
                layout.EnableLayout = true;
                layout.Layout(true);
            }
            else
                layout.AddChild(p);
        }

        public void RemovePlugin(PluginLayoutWidget plugin)
        {
            layout.EnableLayout = false;
            layout.RemoveChild(plugin);
            layout.EnableLayout = true;
            layout.Layout(true);
        }

        //void HandleCloseRequested(object sender, CloseRequestedEventArgs args)
        //{
        //    args.AllowClose = true;
        //    if (args.AllowClose)
        //        Application.Exit();
        //}
    }
}
