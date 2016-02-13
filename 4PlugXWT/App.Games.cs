using FPlug.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace FPlug
{
    public static partial class App
    {
        static Game NoGame = new Game()
        {
            Enabled = true,
            Exists = true,
            Type = new GameType()
            {
                LibraryName = "No games found",
                InitGame = () =>
                {
                    MainWindow.Layout.AddChild(new NoGameWidget());
                }
            }
        };

        public static List<GameType> GameTypes = new List<GameType>()
        {
            #region TF2
            new GameType
            {
                ID = GameID.TeamFortress2,
                LibraryName = "Team Fortress 2",
                IsValid = (s) => Directory.Exists(Path.Combine(s, "tf", "custom")),
                InitGame = () =>
                {
                    PathCustom = Path.Combine(CurrentGame.Path, "tf", "custom");
                    PathCustom_ = Path.Combine(CurrentGame.Path, "tf", "custom_");
                    if (Directory.Exists(PathCustom_))
                        Directory.CreateDirectory(PathCustom_);

                    //MainWindow.Layout.AddChild(new DownloadHudWidget());

                    MainWindow.Layout.AddChild(new PluginTitleWidget(PluginType.Hud));
                    MainWindow.Layout.AddChild(new PluginTitleWidget(PluginType.Unknown));
                    MainWindow.Layout.AddChild(new PluginTitleWidget(PluginType.Vpk));
                    MainWindow.Layout.AddChild(new PluginTitleWidget(PluginType.Hitsound));
                    MainWindow.Layout.AddChild(new PluginTitleWidget(PluginType.Addon));

                    MainWindow.Layout.AddChild(new AddPluginWidget(PluginType.Hud, "Add Huds from Huds.TF"));
                    //flowControl.AddChild(new AddPluginWidget(PluginType.Unknown));
                    //MainWindow.Layout.AddChild(new AddPluginWidget(PluginType.Vpk));
                    //MainWindow.Layout.AddChild(new AddPluginWidget(PluginType.Hitsound));
                    //MainWindow.Layout.AddChild(new AddPluginWidget(PluginType.Addon));

                    CurrentGame.Type.LoadPlugins();
                },
                LoadPlugins = () =>
                {
                    var layout = MainWindow.Layout;
                    layout.EnableLayout = false;
                    layout.ClearPlugins();

                    var w = System.Diagnostics.Stopwatch.StartNew();
                    int pluginCount = 0;
                    foreach (string s in Directory.GetDirectories(App.PathCustom))
                    {
                        if (!Path.GetFileName(s).Equals("workshop", StringComparison.OrdinalIgnoreCase))
                        {
                            MainWindow.AddDirectoryPlugin(s, true);
                            pluginCount++;
                        }
                    }

                    foreach (string s in Directory.GetDirectories(App.PathCustom_))
                    {
                        MainWindow.AddDirectoryPlugin(s, false);
                        pluginCount++;
                    }
                    w.Stop();

                    foreach (string s in Directory.GetFiles(App.PathCustom))
                    {
                        if (s.EndsWith(".vpk", StringComparison.OrdinalIgnoreCase))
                            MainWindow.AddFilePlugin(s, true);
                    }
                    foreach (string s in Directory.GetFiles(App.PathCustom_))
                    {
                        if (s.EndsWith(".vpk", StringComparison.OrdinalIgnoreCase))
                            MainWindow.AddFilePlugin(s, false);
                    }

                    Console.WriteLine("Loaded " + pluginCount + " Plugins in " + w.ElapsedMilliseconds + "ms");
                    layout.EnableLayout = true;
                    layout.Layout(true);
                },
                TryInstallZip = (stream, name) =>
                {
                    ZipStorer zip = ZipStorer.Open(stream, FileAccess.Read);
                    var items = zip.ReadCentralDir();

                    var huds = App.FindHuds(items);
                    if (huds.Length == 0)
                    {
                        return false;
                    }

                    string hud;

                    if (huds.Length == 1)
                        hud = huds[0];
                    else
                    {
                        hud = huds[0];
                    }

                    hud += "/";

                    var pluginDirectory = App.MakePluginDirectoryUnique(Path.Combine(App.PathCustom_, name));
                    Directory.CreateDirectory(pluginDirectory);

                    foreach (var entry in items)
                    {
                        if (!entry.FilenameInZip.EndsWith("/") && entry.FilenameInZip.StartsWith(hud))
                        {
                            string filename = Path.Combine(pluginDirectory, entry.FilenameInZip.Substring(hud.Length));
                            string folder = Path.GetDirectoryName(filename);
                            if (!Directory.Exists(folder))
                                Directory.CreateDirectory(folder);

                            zip.ExtractFile(entry, filename);
                        }
                    }

                    MainWindow.AddDirectoryPlugin(pluginDirectory, false, true);

                    return true;
                },
                //GetCustomMenu = () =>
                //{
                //    Menu menu = new Menu();
                //
                //    {
                //        menu.Items.Add(new MenuItem { Label = 
                //    }
                //
                //    return menu;
                //}
            },
            #endregion
            new GameType
            {
                ID = GameID.Test,
                LibraryName = "Test",
                IsValid = (s) => true,
                InitGame = () => 
                {
                    MainWindow.Layout.AddChild(new NoGameWidget());
            
                    //PathCustom = Path.Combine(CurrentGame.Path, "custom");
                    //PathCustom_ = Path.Combine(CurrentGame.Path, "custom_");
                    //if (Directory.Exists(PathCustom_)) Directory.CreateDirectory(PathCustom_);
                }
            },
        };
    }
}
