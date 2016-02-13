using System;
using Xwt.Drawing;

namespace FPlug
{
    public enum PluginType
    {
        None,
        Downloading,
        Hud,
        Vpk,
        Hitsound,
        Addon,
        Unknown,
    }

    public static class PluginTypeExtensions
    {
        static Color[] PluginColors = new[]{
            Colors.Black,
            Colors.Black,

            Color.FromBytes(0x03, 0xA9, 0xF4), // #0288D1

            Color.FromBytes(0x67, 0x3A, 0xB7), // #512DA8
            Color.FromBytes(0x4C, 0xAF, 0x50), // #388E3C
            
            //Colors.DarkCyan,
            //Colors.DarkRed,

            Color.FromBytes(0xF4, 0x43, 0x36), // #D32F2F
            Color.FromBytes(0x9C, 0x27, 0xB0), // #7B1FA2
            //Color.FromBytes(0xFF, 0x57, 0x22), // #E64A19
            //Colors.DarkGoldenrod,
            //Colors.DarkMagenta,
            //Colors.DarkGray,
            //Colors.SlateGray
        };

        static Color[] PluginBgColors;

        static PluginTypeExtensions()
        {
            PluginBgColors = new Color[PluginColors.Length];
            for (int i = 0; i < PluginColors.Length; i++)
            {
                PluginBgColors[i] = PluginColors[i].WithAlpha(0.09d);
            }
        }

        public static Color GetColor(this PluginType type)
        {
            return PluginColors[(int)type];
        }

        public static Color GetBGColor(this PluginType type)
        {
            return PluginBgColors[(int)type];
        }

        public static string GetName(this PluginType type)
        {
            return type.ToString();
        }
    }
}
