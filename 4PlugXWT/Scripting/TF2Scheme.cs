using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FPlug.Scripting
{
    public class Source1SchemeFile : Source1ResourceFile
    {
        Node colors = null;
        Node baseSettings = null;
        Node bitmapFontFiles = null;
        Node fonts = null;
        Node borders = null;
        Node customFontFiles = null;

        public Source1SchemeFile(Script script, string filename)
            : base (script, filename)
        {
            if (Loaded)
            {
                BaseNode.Children.TryGetValue("Colors", out colors);
                BaseNode.Children.TryGetValue("BaseSettings", out baseSettings);
                BaseNode.Children.TryGetValue("BitmapFontFiles", out bitmapFontFiles);
                BaseNode.Children.TryGetValue("Fonts", out fonts);
                BaseNode.Children.TryGetValue("Borders", out borders);
                BaseNode.Children.TryGetValue("CustomFontFiles", out customFontFiles);
            }
        }

        public void SetColor(string name, string value)
        {
            if (colors != null)
            {
                Line line;
                colors.Items.TryGetValue(name, out line);
                if (line != null)
                    line.SetValue(value);
            }
        }

        public void SetBaseSetting(string name, string value)
        {
            if (baseSettings != null)
            {
                Line line;
                baseSettings.Items.TryGetValue(name, out line);
                if (line != null)
                    line.SetValue(value);
            }
        }

        public void SetBitmapFontFile(string name, string value)
        {
            if (bitmapFontFiles != null)
            {
                Line line;
                bitmapFontFiles.Items.TryGetValue(name, out line);
                if (line != null)
                    line.SetValue(value);
            }
        }

        public void SetFont(string fontName, string index, string valueName, string value)
        {
            Node node = fonts; // Fonts
            if (node == null) return;

            node.Children.TryGetValue(fontName, out node); // "fogCrosshair"
            if (node == null) return;

            node.Children.TryGetValue(index, out node); // "1"
            if (node == null) return;

            Line line;
            node.Items.TryGetValue(valueName, out line);
            if (line != null)
                line.SetValue(value);
        }

        public void SetBorder(string[] path, string name, string value)
        {
            if (borders != null)
            {
                Node node = borders;

                foreach (string s in path)
                {
                    node.Children.TryGetValue(s, out node);
                    if (node == null) return;
                }

                Line line;
                node.Items.TryGetValue(name, out line);
                if (line != null)
                    line.SetValue(value);
            }
        }

        public void SetCustomFontFile(string name, string value)
        {
            if (customFontFiles != null)
            {
                Line line;
                customFontFiles.Items.TryGetValue(name, out line);
                if (line != null)
                    line.SetValue(value);
            }
        }

        public void SetCustomFontFile(string[] path, string name, string value)
        {
            if (customFontFiles != null)
            {
                Node node = customFontFiles;

                foreach (string s in path)
                {
                    node.Children.TryGetValue(s, out node);
                    if (node == null) return;
                }

                Line line;
                node.Items.TryGetValue(name, out line);
                if (line != null)
                    line.SetValue(value);
            }
        }
    }

    public class Source1ResourceFile
    {
        public class Node
        {
            public String Name { get; private set; }

            public Node(string name, Node parent)
            {
                Name = name;
                Parent = parent;
                Children = new Dictionary<string, Node>();
                Items = new Dictionary<string, Line>();
            }

            public Node Parent { get; private set; }
            public Dictionary<string, Node> Children { get; private set; }
            public Dictionary<string, Line> Items { get; private set; }
        }

        public class Line
        {
            public string Text { get; set; }
            public string Name { get; set; }

            public int NameStart { get; set; }
            public int NameLength { get; set; }
            public int ValueStart { get; set; }
            public int ValueLength { get; set; }

            public Line(string text)
            {
                Text = text;
            }

            public void SetValue(string value)
            {
                Text = Text.Remove(ValueStart) + value + Text.Substring(ValueStart + ValueLength);
                ValueLength = value.Length;
            }

            public string GetValue()
            {
                return Text.Substring(ValueStart, ValueLength);
            }
        }

        public void SetValue(string[] path, string name, string value)
        {
            if (BaseNode != null)
            {
                Node node = BaseNode;

                foreach (string s in path)
                {
                    node.Children.TryGetValue(s, out node);
                    if (node == null) return;
                }

                Line line;
                node.Items.TryGetValue(name, out line);
                if (line != null)
                    line.SetValue(value);
            }
        }

        public Node BaseNode = null;

        public List<Line> AllLines = new List<Line>();

        public string Filename { get; private set; }

        public bool Loaded { get; private set; }

        Script _script;

        public Source1ResourceFile(Script script, string filename)
        {
            _script = script;

            Filename = filename = script.FolderCache.TryResolvePath(filename, false);

            Loaded = false;
            if (filename != null)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(filename))
                    {
                        Node currentNode = null;

                        string lastName = null;
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Line currentLine = new Line(line);
                            AllLines.Add(currentLine); 

                            int lineEnd = line.IndexOf("//");

                            lineEnd = lineEnd == -1 ? line.Length : lineEnd;

                            int i = 0;

                            // whitespace
                            for (; i < lineEnd; i++)
                                if (!Char.IsWhiteSpace(line, i))
                                    break;
                            if (i == lineEnd) continue;

                            // { }
                            char c = line[i];

                            bool isOpenBrace;
                            if ((isOpenBrace = c == '{') || c == '}')
                            {
                                i++;
                                for (; i < lineEnd; i++)
                                    if (!Char.IsWhiteSpace(line, i))
                                        break;
                                if (i != lineEnd) goto error;

                                if (isOpenBrace)
                                {
                                    if (currentNode == null)
                                        currentNode = BaseNode = new Node(lastName, null);
                                    else
                                        currentNode.Children[lastName] = currentNode = new Node(lastName, currentNode);
                                }
                                else
                                {
                                    if (currentNode == null)
                                        goto error;
                                    else
                                        currentNode = currentNode.Parent;
                                }
                                continue;
                            }

                            string name = null;
                            int start;

                            // name
                            if (c == '\"')
                            {
                                start = ++i;

                                for (; i < lineEnd; i++)
                                    if (line[i] == '\"')
                                        break;
                                if (i == lineEnd) continue;

                                //if (++i >= lineEnd || !Char.IsWhiteSpace(line[i])) continue;
                                currentLine.NameStart = start;
                                currentLine.NameLength = i - start;
                                name = line.Substring(start, i - start);
                                ++i;
                            }
                            else
                            {
                                start = i;

                                for (; i < lineEnd; i++)
                                    if (Char.IsWhiteSpace(line, i))
                                        break;

                                //if (i == lineEnd) continue;
                                currentLine.NameStart = start;
                                currentLine.NameLength = i - start;
                                name = line.Substring(start, i - start);
                                //try
                                //{
                                //    var sioajioaijopdsafpjosdfpdsjiofapjodsfaiojfdooiohpgnohipdsgionhpdnji = line.Remove(i);
                                //    ;
                                //}
                                //catch { }
                            }

                            // whitespace
                            for (; i < lineEnd; i++)
                                if (!Char.IsWhiteSpace(line, i))
                                    break;
                            if (i >= lineEnd)
                            {
                                lastName = name;
                                continue;
                            }

                            var sch = line.Remove(i);

                            // value
                            if (line[i] != '\"')
                                continue;

                            start = ++i;

                            for (; i < lineEnd; i++)
                                if (line[i] == '\"')
                                    break;
                            var ch = line.Remove(i);
                            //if (i >= lineEnd) continue;

                            //if (++i >= lineEnd || !Char.IsWhiteSpace(line[i])) continue;
                            currentLine.Name = name;
                            currentLine.ValueStart = start;
                            currentLine.ValueLength = i - start;
                            if (currentNode != null)
                                currentNode.Items[name] = currentLine;
                        }

                        Loaded = true;

                    error:
                        ;
                    }
                }
                catch
                {

                }
            }
        }

        public void Save()
        {
            if (Loaded)
            {
                using (StreamWriter writer = new StreamWriter(Filename))
                {
                    for (int i = 0; i < AllLines.Count; i++)
                    {
                        if (i == AllLines.Count - 1)
                            writer.Write(AllLines[i].Text);
                        else
                            writer.WriteLine(AllLines[i].Text);
                    }
                }
            }
        }
    }
}
