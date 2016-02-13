using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public enum NewlineType : byte
    {
        RN, N, R
    }

    public enum LineStyle : short
    {
        None = 0x0000,
        Unsaved = 0x0001,
        Edited = 0x0002,
        Marked = 0x0004,
    }

    public class Line : PrimitiveList<Charp>
    {
        public NewlineType NewlineType { get; set; }
        public LineStyle LineStyle { get; set; }

        public Line()
        {
            
        }

        public Line(string text)
        {
            var line = arr = new Charp[text.Length];
            for (int i = 0; i < line.Length; i++)
                line[i] = text[i].ToStyledChar();
        }

        public string GetString()
        {
            char[] C = new char[Count];
            for (int i = 0; i < Count; i++)
                C[i] = arr[i].Char;
            return new string(C);
        }

        public string GetString(int from, int to)
        {
            char[] C = new char[to - from];
            for (int i = 0, count = to - from; i < count; i++)
                C[i] = arr[from + i].Char;
            return new string(C);
        }

        public override string ToString()
        {
            return GetString();
        }

        public void SetStyle(CharpStyle style, int from, int to)
        {
            for (int i = from; i < to; i++)
                arr[i] = arr[i].WithStyle(style);
        }

        public void SetColor(int color, int from, int to)
        {
            SetStyle((CharpStyle)color, from, to);
        }
    }
}
