using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FPlug
{
    public partial class Settings
    {
        public SortedDictionary<string, string> items = new SortedDictionary<string, string>();

        public char KeyValueSepertaor { get; set; } = '=';

        string path = null;

        public Settings()
        {

        }

        public Settings(string path)
            : this()
        {
            LoadFile(path);
        }

        public void LoadFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    Read(new StreamReader(path));
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Settings: Couldn't load {path}, \"{exc.Message}\".");
            }
        }

        public void LoadString(string s)
        {
            Read(new StringReader(s));
        }

        private void Read(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length < 3)
                    continue;

                int index = line.IndexOf('=');
                if (index > 0)
                {
                    if (line[index] == '=')
                    {
                        string key = line.Remove(index);
                        string value = index != line.Length ? line.Substring(index + 1) : null;

                        items[key] = value;
                    }
                }
            }
        }

        public void Save()
        {
            if (path == null)
                throw new InvalidOperationException("The file of the settings was never set. Use LoadFile().");
            SaveTo(path);
        }

        public void SaveTo(string path)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    foreach (var item in items)
                    {
                        writer.WriteLine(item.Key + KeyValueSepertaor + item.Value);
                    }
                }
            }
            catch { }
        }

        // String
        public string GetString(string key, string Default = null)
        {
            string a;
            return items.TryGetValue(key, out a) ? a : Default;
        }

        public void SetString(string key, string value)
        {
            items[key] = value;
        }

        // Int
        public int GetInt(string key, int Default = -1)
        {
            string a;
            int i;
            return items.TryGetValue(key, out a) ? (int.TryParse(a, out i) ? i : Default) : Default;
        }

        public void SetInt(string key, int value)
        {
            items[key] = value.ToString();
        }
    }
}
