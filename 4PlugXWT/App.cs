using FPlug.Options;
using FPlug.Scripting;
using FPlug.Widgets;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO.Compression;
using Xwt;
using Xwt.Backends;
using System.Text.RegularExpressions;
using Xwt.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using FPlug.Editor;

namespace FPlug
{
    public static partial class App
    {
        // MISC
        public static Task InitializeOptionsTask;

        public static string PathCustom;
        public static string PathCustom_;

        public static bool DebugSettings = false;

        public static bool WpfFancyStyle = false;

        public static readonly string WindowTitle = "4Plug v1.2 test build";

        public static Image Icon;

        public static FVersion Version = new FVersion(1.1m);

        public static string WinVersionUrl = "http://www.yuhrney.square7.ch/TF2/Plug.txt";
        public static string WinUpdateUrl = "http://www.yuhrney.square7.ch/TF2/Plug.zip";

        //public static String MonoVersionUrl = "http://www.yuhrney.square7.ch/TF2/Plug_mono.txt";
        //public static String MonoUpdateUrl = "http://www.yuhrney.square7.ch/TF2/Plug_mono.zip";

        public static string SteamGroup = "http://steamcommunity.com/groups/4stuff";
        public static string TfTvThread = "http://teamfortress.tv/forum/thread/13401";

        public static bool FirstTime = false;
        public static string FirstTimeSteamDetected = null;

        public static bool ApplyUpdate = false;

        // OS SPECIFIC
        public static Action<IWidgetBackend> InitDropShadow = null;
        public static Action<IWidgetBackend> AnimateOpacityIn = null;
        public static Action<IWidgetBackend> InitMoveWidget = null;
        public static Action<IWidgetBackend, double, double, double, double, bool> MoveWidget = null;
        public static Action<IWindowFrameBackend> InitMainWindow = null;
        public static Action<IWindowFrameBackend> BeforeWindowShown = null;
        public static Func<string, string> CustomSelectFolder = null;
        public static Func<string, bool> CustomDelete = null;

        public static Action<WebClient> SetCustomImageViewImageDataDownloadedEvent = null;
        public static Action<string, ImageView> SetImageAsync = null;

        // SCRIPTING
        public static List<ControlDesc> Controls = new List<ControlDesc>();

        public static Dictionary<Type, ScriptType> ScriptTypeByType = new Dictionary<Type, ScriptType>();

        static List<Function> WidgetFunctions = new List<Function>();

        // GAMES
        #region GameTypes
        public static Game CurrentGame { get; set; }

        public static void SetCurrentGame(Game game)
        {
            CurrentGame = game;
            MainWindow.Layout.ClearAll();

            game.Type.InitGame();

            MainWindow.Title = WindowTitle + " - " + game.Type.LibraryName;
        }

        public static List<Game> Games = new List<Game>();

        public static Game LoadGame(string path)
        {
            GameType t = GameTypes.FirstOrDefault(type => type.LibraryName == path);

            if (t != null)
            {
                if (t.GetCustomMenu != null)
                {
                    MainWindow.CustomGameMenu.Visible = true;
                    MainWindow.CustomGameMenu.SubMenu = t.GetCustomMenu();
                    MainWindow.CustomGameMenu.Label = t.LibraryName;
                }
                else
                {
                    MainWindow.CustomGameMenu.Visible = false;
                }
                return new Game { Exists = true, Type = t, Path = path };
            }
            return null;
            //return new Game { Exists = false, Path = path };
        }

        public static void ReloadGames(List<string> libraries, List<string> games)
        {
            Games.Clear();

            string library = null;

            Action<string, string> loadGame = (path, s) =>
                {
                    var type = GameTypes.FirstOrDefault(gt => gt.LibraryName == s);

                    if (type != null && (type.IsValid == null || type.IsValid(Path.Combine(path, s))))
                    {
                        Games.Add(new Game { Type = type, Path = Path.Combine(path, s), Library = library });
                    }
                };

            //games.Do(s => loadGame(s, Path.GetFileName(s.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))));

            foreach (string lib in libraries)
            {
                Dir dir;
                if (Dir.TryGet(lib, out dir))
                {
                    foreach (string s in dir.Folders)
                        loadGame(lib, s);
                }
            }

            if (Games.Count == 0)
                Games.Add(NoGame);

            int index = (int)XSettings.Games.Attribute("selectedIndex");

            if (index >= 0 && index < Games.Count)
                SetCurrentGame(Games[index]);
            else
            {
                SetCurrentGame(Games[0]);
                XSettings.Games.SetAttributeValue("selectedIndex", 0);
            }

            //if (MainWindow != null)
            MainWindow.ReloadGames();
        }
        #endregion GameTypes

        public static bool InstallZip(Stream stream, string name)
        {
            if (CurrentGame.Type.TryInstallZip == null)
                return false;
            return CurrentGame.Type.TryInstallZip(stream, name);
        }

        public static MainWindow MainWindow { get; set; }

        public static bool TryParsePoint(string s, out Point? point)
        {
            try
            {
                int index = s.IndexOf(',');
                if (index != -1)
                {
                    int x, y;
                    if (int.TryParse(s.Remove(index), out x) && int.TryParse(s.Substring(index + 1), out y))
                    {
                        point = new Point(x, y);
                        return true;
                    }
                }
            }
            catch
            {

            }
            point = null;
            return false;
        }

        public static bool TryParseRectangle(string s, out Rectangle point)
        {
            try
            {
                int index = s.IndexOf('|');
                if (index != -1)
                {
                    Point? p1, p2;
                    if (TryParsePoint(s.Remove(index), out p1) && TryParsePoint(s.Substring(index + 1), out p2))
                    {
                        point = new Rectangle(p1.Value.X, p1.Value.Y, p2.Value.X, p2.Value.Y);
                        return true;
                    }
                }
            }
            catch { }

            point = Rectangle.Zero;
            return false;
        }

        public static bool TryParseColor(string s, out Color color)
        {
            string[] S = s.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            byte r, g, b, a = 255;
            if (S.Length == 1)
            {
                bool hasAlpha = false;
                if ((S[0].Length == 7 || (hasAlpha = S[0].Length == 9)) && S[0][0] == '#')
                {
                    uint i;
                    if (uint.TryParse(S[0].Substring(1), System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out i))
                    {
                        if (hasAlpha)
                            color = Xwt.Drawing.Color.FromBytes((byte)((i & 0xFF000000) >> 24), (byte)((i & 0xFF0000) >> 16), (byte)((i & 0xFF00) >> 8), (byte)((i & 0xFF)));
                        else
                            color = Xwt.Drawing.Color.FromBytes((byte)((i & 0xFF0000) >> 16), (byte)((i & 0xFF00) >> 8), (byte)((i & 0xFF)));
                        return true;
                    }
                }
            }
            else if ((S.Length == 3 || S.Length == 4) && (byte.TryParse(S[0], out r) && byte.TryParse(S[1], out g) && byte.TryParse(S[2], out b) && (S.Length == 3 || byte.TryParse(S[3], out a))))
            {
                color = S.Length == 3 ? Color.FromBytes(r, g, b) : Color.FromBytes(r, g, b, a);
                return true;
            }
            color = Xwt.Drawing.Colors.White;
            return false;
        }

        // RUN
        public static void Run(ToolkitType type)
        {
            var cache = new Scripting2.FolderCache(@"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2\tf\custom\7HUD-master");

            //cache.CopyFile("a.txt", "b.txt");
            cache.DeleteDirectory("a");

            try
            {
                if (File.Exists("_update.exe"))
                    File.Delete("_update.exe");
                if (File.Exists("_update.zip"))
                    File.Delete("_update.zip");
            }
            catch { }

            Application.Initialize(type);

            Icon = Image.CreateMultiSizeIcon(new[] { Resources.GetImage("4P.png"), Resources.GetImage("4P 16.png") });

            #region InitalizeOptionsTask
            InitializeOptionsTask = new Task(() =>
                {
                    System.Reflection.Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Do(t =>
                        {
                            object[] typeAttributes;
                            if (t.IsClass // && t.Namespace == "FPlug.Options"
                                && (typeAttributes = t.GetCustomAttributes(typeof(ScriptClassAttribute), false)).Length > 0)
                            {
                                ScriptClassAttribute typeAttribute = (ScriptClassAttribute)typeAttributes[0];

                                List<Method> methods = new List<Method>();
                                List<Property> properties = new List<Property>();
                                List<PropertyDesc> propDescs = new List<PropertyDesc>();
                                Dictionary<string, PropertyInfo> events = new Dictionary<string, PropertyInfo>();

                                t.GetProperties().Do((p) =>
                                {
                                    object[] mAttributes;
                                    if ((mAttributes = p.GetCustomAttributes(typeof(ScriptMemberAttribute), true)).Length > 0)
                                    {
                                        ScriptMemberAttribute memberAttribute = (ScriptMemberAttribute)mAttributes[0];

                                        Property pr0 = new Property(memberAttribute.Name ?? p.Name, ScriptType.DefaultTypes[memberAttribute.ReturnType], p);
                                        properties.Add(pr0);

                                        PropertyDesc propDesc = new PropertyDesc()
                                        {
                                            Name = pr0.Name,
                                            ScriptType = ScriptType.DefaultTypes[memberAttribute.ReturnType],
                                            Get = pr0.GetFunc,
                                            Set = pr0.SetAction
                                        };
                                        propDescs.Add(propDesc);
                                    }
                                    else if ((mAttributes = p.GetCustomAttributes(typeof(ScriptEventAttribute), true)).Length > 0)
                                    {
                                        ScriptEventAttribute eventAttribute = (ScriptEventAttribute)mAttributes[0];

                                        events.Add(p.Name, p);
                                    }
                                });

                                ScriptType scriptType = new ScriptType(typeAttribute.Name, methods.ToArray(), properties.ToArray());

                                ControlDesc controlDesc = new ControlDesc()
                                {
                                    Name = typeAttribute.Name,
                                    ScriptType = scriptType,
                                    Properties = propDescs,
                                    Type = t,
                                    Events = events
                                };
                                Controls.Add(controlDesc);

                                ScriptTypeByType.Add(t, scriptType);
                            }
                        });

                    //StringBuilder controls = new StringBuilder(1024);
                    //foreach (var c in Controls.ToList().OrderBy((desc) => desc.Name))
                    //{
                    //    controls.AppendLine("###" + c.Name);
                    //    if (c.Properties.Count > 2)
                    //    {
                    //        controls.AppendLine("|Name|Type|Description|");
                    //        controls.AppendLine("|---|---|---|");
                    //        foreach (var prop in c.Properties)
                    //        {
                    //            if (prop.Name != "ID" && prop.Name != "Width")
                    //                controls.AppendLine("|" + prop.Name + "|" + prop.ScriptType.Name + "||");
                    //        }
                    //        controls.AppendLine();
                    //    }
                    //}
                    //Application.Invoke(() => Clipboard.SetText(controls.ToString()));
                });
            #endregion

            InitializeOptionsTask.Start();

            FirstTime = !File.Exists("config.xml");
            //FirstTime = true;

            if (FirstTime)
            {
                XSettings.Load("config.xml");
                if (Directory.Exists(@"C:\Program Files\Steam\steamapps\common"))
                    App.FirstTimeSteamDetected = @"C:\Program Files\Steam\steamapps\common";
                else if (Directory.Exists(@"C:\Program Files (x86)\Steam\steamapps\common"))
                    App.FirstTimeSteamDetected = @"C:\Program Files (x86)\Steam\steamapps\common";

                new SplashWindow().Run();
            }

            using (MainWindow w = App.MainWindow = new MainWindow())
            //using (EditorWindow w = new EditorWindow())
            //using (SettingsWindow w = new SettingsWindow("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Team Fortress 2\\tf\\custom\\7HUD\\plugin.xml", "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Team Fortress 2\\tf\\custom\\7HUD", true))
            //using (SettingsWindow w = SettingsWindow.CreateLegacySettings(@"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2\tf\custom_\7HUD", File.ReadAllLines(@"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2\tf\custom_\7HUD\settings.data")))
            {

                XSettings.Load("config.xml");
                w.Closed += (s, e) =>
                {
                    Application.Exit();
                };

                //if (Games.Count == 0)
                //{
                //    var w = new AddGamesWindow();
                //    w.Run();
                //}

                //SetCurrentGame(Games[Math.Min(Math.Max((int)XSettings.Games.Attribute("selectedindex"), Games.Count - 1), 0)]);

                w.Show();

                Application.Run();
            }

            XSettings.Save();

            if (ApplyUpdate)
            {
                File.Copy("4PlugUpdate.exe", "_update.exe");
                Process.Start("_update.exe");
            }

            Application.Dispose();
        }

        public static string[] FindHuds(List<ZipStorer.ZipFileEntry> items)
        {
            // hud is in root
            if (items.Any(e => e.FilenameInZip.StartsWith("resource/ui/")))
            {
                return new[] { "resource/" };
            }
            else
            {
                List<dynamic> allMatches = new List<dynamic>();

                foreach (var item in items.Select(e => e.FilenameInZip))
                {
                    int index = item.IndexOf("/resource/ui/");

                    if (index != -1)
                    {
                        int count = 0;

                        for (int i = 0; i < index; i++)
                        {
                            if (item[i] == '/')
                                count++;
                        }

                        string dir = item.Remove(index);
                        dynamic d = new { Count = count, Dir = dir };

                        if (!allMatches.Any(dd => dd.Count == count && dd.Dir == dir))
                            allMatches.Add(d);
                    }
                }

                if (allMatches.Count == 0)
                {
                    return null; // no hud found
                }
                else
                {
                    // (allMatches.Count == 1)
                    List<dynamic> lowestMatches = allMatches.GroupBy(d => (int)d.Count).OrderBy(g => g.Key).First().ToList();

                    return lowestMatches.Select(d => (string)d.Dir).ToArray();
                }
            }
        }

        public static string MakePluginDirectoryUnique(string path)
        {
            string name;
            string custom;

            if (PathCustom.Length > PathCustom_.Length)
            {
                if (path.StartsWith(PathCustom))
                {
                    name = path.Substring(PathCustom.Length + 1);
                    custom = PathCustom;
                }
                else
                {
                    name = path.Substring(PathCustom_.Length + 1);
                    custom = PathCustom_;
                }
            }
            else
            {
                if (path.StartsWith(PathCustom_))
                {
                    name = path.Substring(PathCustom_.Length + 1);
                    custom = PathCustom_;
                }
                else
                {
                    name = path.Substring(PathCustom.Length + 1);
                    custom = PathCustom;
                }
            }

            for (int i = name.Length - 1; i >= 0; i--)
            {
                if (name[i] >= '0' && name[i] <= '9')
                    continue;
                else if (name[i] == ' ')
                    name = name.Remove(i).Trim();
                else break;
            }

            path = name;

            for (int i = 1; ; ++i)
            {
                if (!Directory.Exists(Path.Combine(PathCustom, path)) && !Directory.Exists(Path.Combine(PathCustom_, path)))
                    return Path.Combine(custom, path);

                path = name + " " + i;
            }
        }

        static int uniqueID = 0;
        public static int GetUniqueID()
        {
            return uniqueID++;
        }
    }
}
