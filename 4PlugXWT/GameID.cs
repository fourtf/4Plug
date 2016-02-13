using System;
using System.Collections.Generic;
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
        public String LibraryName { get; set; }
        public GameID ID { get; set; }
        public Func<string, bool> IsValid { get; set; }
        public Action InitGame { get; set; }
    }

    public class Game
    {
        public GameType Type { get; set; }
        public String Path { get; set; }
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
