using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Editor
{
    public class EditorWindow : Window
    {
        public EditorWindow()
        {
            Width = 600;
            Height = 500;

            EditorWidget editor = new EditorWidget();
            Content = editor;
            editor.SetFocus();

            //Font font = Font.FromName("Consolas 12");
            //
            //double refWidth =  new TextLayout()
            //    {
            //        Text = "X",
            //        Font = font
            //    }.GetSize().Width;
            //
            //List<char> chars = new List<char>();
            //
            //for (int i = 0; i < 4000; i++)
            //{
            //    TextLayout layout = new TextLayout()
            //    {
            //        Text = ((char)i).ToString(),
            //        Font = font
            //    };
            //    if (layout.GetSize().Width != refWidth)
            //        chars.Add((char)i);
            //}
            //123.Log();
        }
    }
}
