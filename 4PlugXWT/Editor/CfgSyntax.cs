using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public class CfgSyntax : Syntax
    {
        public CfgSyntax(TextSource source)
            : base(source)
        {

        }

        public override void TextAdded(Range range)
        {
            base.TextAdded(range);

            parseLines(range.Start.Y, range.End.Y);
        }

        public override void TextRemoved(Range range)
        {
            base.TextRemoved(range);

            parseLine(range.Start.Y);
        }

        void parseLines(int from, int to)
        {
            for (int i = from; i <= to; i++)
            {
                parseLine(i);
            }
        }

        static string keywords = "bind|unbind|alias|exec|rcon";
        static string[] Keywords = keywords.Split('|');

        void parseLine(int line)
        {
            Source.SetRangeStyle(Source.RangeAtLine(line), CharpStyle.None);

            Line l = Source[line];

            string s = l.ToString();

            foreach (string key in Keywords)
            {
                int index = -1;
                while (index < s.Length && (index = s.IndexOf(key, index + 1)) != -1)
                {
                    //if ((index == 0 || Char.IsWhiteSpace(l[index].Char))

                    l.SetColor(1, index, index + key.Length);
                }
            }


            //Scripting.StringParser parser = new Scripting.StringParser(l.ToString());
            //
            //parser.ReadCfgWs();
            //if (parser.Ended()) return;
            //string s = parser.ReadCfgToken();
            //if (s == "alias" || s == "bind")
            //{
            //
            //}
        }
    }
}
