using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FPlug
{
    public static class XSettings
    {
        public static bool Initialized { get; private set; }

        public static XDocument Document = null;
        public static XElement Root;

        public static XElement Games;

        public static XElement Settings;
        public static XElement SettingsDebugAutoreload;

        public static string Path { get; set; }

        public static void Load(string path)
        {
            Path = path;

            // try to load xml
            try
            {
                if (File.Exists(path))
                    Document = XDocument.Load(path);
            }
            catch { }

            // validate <Settings>
            if (Document == null || Document.Element("settings") == null)
            {
                Document = new XDocument();
                Document.Add(Root = new XElement("settings"));
            }
            else
                Root = Document.Element("settings");

            Settings = Root.Element("settings") ?? Add(Root, "settings");
            SettingsDebugAutoreload = Settings.Element("debugAutoreload");

            // add child elements
            Games = Root.Element("games") ?? Add(Root, "games");

            if (XSettings.Games.Attribute("selectedIndex") != null || (int?)XSettings.Games.Attribute("selectedIndex") == null)
                XSettings.Games.SetAttributeValue("selectedIndex", 0);

            if (App.FirstTimeSteamDetected != null)
            {
                var e = new XElement("library");
                e.SetAttributeValue("path", App.FirstTimeSteamDetected);
                XSettings.Games.Add(e);
            }

            // load games
            List<string> libraries = new List<string>();
            List<string> games = new List<string>();

            foreach (var e in XSettings.Games.Elements())
            {
                XAttribute attr = e.Attribute("path");

                if ((attr = e.Attribute("path")) != null)
                {
                    string p = (string)attr;
                    if (e.Name == "library")
                        libraries.Add(p);
                    else if (e.Name == "game")
                        games.Add(p);
                }
            }

            if (App.MainWindow != null)
                App.ReloadGames(libraries, games);

            Initialized = true;
        }

        public static void Save()
        {
            Document.Save(Path);
        }

        public static XElement Add(XElement parent, XName name)
        {
            var e = new XElement(name);
            parent.Add(e);
            return e;
        }
    }
}
