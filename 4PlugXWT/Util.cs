using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FPlug
{
    public static partial class Util
    {
        public static bool FileExists(this string file, Action<string> action)
        {
            bool b = File.Exists(file);
            if (b)
                action(file);
            return b;
        }

        public static bool DirectoryExists(this string file, Action<string> action)
        {
            bool b = Directory.Exists(file);
            if (b)
                action(file);
            return b;
        }

        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);

            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, temppath);
            }
        }

        public static string GetFileName(this string s, bool isDirectory = false)
        {
            return isDirectory ? Path.GetDirectoryName(s) : Path.GetFileName(s);
        }

        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }

        public static string RemoveFromRight(this string s, int count)
        {
            if (count == 0)
                return s;
            return s.Remove(s.Length - count);
        }

        public static T Log<T>(this T obj)
        {
            Log(obj, 0);
            return obj;
        }

        public static void Log(this object obj, int intend)
        {
            if (obj == null)
            {
                Console.WriteLine(intend > 0 ? new string(' ', intend) + "null" : "null");
            }
            else if (!(obj is string) && obj is IEnumerable)
            {
                Console.WriteLine(intend > 0 ? new string(' ', intend) + obj.ToString() : obj.ToString());
                if (((IEnumerable)obj).GetEnumerator().MoveNext())
                {
                    foreach (object o in (IEnumerable)obj)
                        Log(o, intend + 2);
                }
            }
            else if (obj is char)
            {
                if ((char)obj <= ' ')
                    Console.WriteLine(intend > 0 ? new string(' ', intend) + "0x" + (short)(char)obj : "0x" + (short)(char)obj);
                else
                    Console.WriteLine(intend > 0 ? new string(' ', intend) + obj.ToString() : obj.ToString());
            }
            else
            {
                Console.WriteLine(intend > 0 ? new string(' ', intend) + obj.ToString() : obj.ToString());
            }
        }

        public static void LogEach<T>(this IEnumerable<T> obj, Func<T, object> func)
        {
            Console.WriteLine(obj);
            foreach (T t in obj)
                Console.WriteLine("  " + func(t));
        }

        public static void Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }

        public static int For<T>(this IEnumerable<T> list, Action<int, T> action)
        {
            if (action == null) throw new ArgumentNullException("action");
        
            var index = 0;
        
            foreach (var elem in list)
                action(index++, elem);
        
            return index;
        }

        public static T Pop<T>(this List<T> list)
        {
            T t = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return t;
        }

        public static T AddAnd<T>(this List<T> list, T item)
        {
            list.Add(item);
            return item;
        }

        public static string[] SplitAndTrimEscaped(string s, char escapeChar, char splitChar)
        {
            bool inEscape = false;
            string item = "";
            List<string> items = new List<string>();

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (inEscape)
                {
                    item += c;
                    inEscape = false;
                }
                else
                {
                    if (c == escapeChar)
                        inEscape = true;
                    else if (c == splitChar)
                    {
                        if (!string.IsNullOrWhiteSpace(item))
                            items.Add(item.Trim());
                        item = "";
                    }
                    else
                        item += c;
                }
            }

            if (!string.IsNullOrWhiteSpace(item))
                items.Add(item.Trim());
            return items.ToArray();
        }
    }
}
