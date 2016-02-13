using FPlug.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug
{
    public enum GameID
    {
        Unknown,
        TeamFortress2,
        Test,
    }

    public class GameType
    {
        public string LibraryName { get; set; }
        public GameID ID { get; set; }
        public Func<string, bool> IsValid { get; set; }
        public Action InitGame { get; set; }
        public Action LoadPlugins { get; set; }
        public Func<Stream, string, bool> TryInstallZip { get; set; }
        public Func<Menu> GetCustomMenu { get; set; }
    }

    public class Game
    {
        public GameType Type { get; set; }
        public string Path { get; set; }
        public bool Exists { get; set; }
        public bool Enabled { get; set; }
        public string Library { get; set; }

        public Game()
        {
            Enabled = true;
        }
    }

    public class GameMenuItem : CheckBoxMenuItem
    {
        public Game Game { get; set; }

        public GameMenuItem(Game game)
        {
            Game = game;
        }
    }
}
