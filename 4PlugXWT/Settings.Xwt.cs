using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug
{
    public partial class Settings
    {
        // Point
        public Point GetPoint(string key, Point Default = new Point())
        {
            string a;
            Point? i;
            return items.TryGetValue(key, out a) ? (App.TryParsePoint(a, out i) ? i.Value : Default) : Default;
        }

        public void SetPoint(string key, Point value)
        {
            items[key] = value.X + "," + value.Y;
        }

        // Rectangle
        public Rectangle GetRectangle(string key, Rectangle Default = new Rectangle())
        {
            string a;
            Rectangle i;
            return items.TryGetValue(key, out a) ? (App.TryParseRectangle(a, out i) ? i : Default) : Default;
        }

        public void SetRectangle(string key, Rectangle value)
        {
            items[key] = value.X + "," + value.Y + "|" + value.Width + "," + value.Height;
        }
    }
}
