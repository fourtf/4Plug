using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using _Dir = System.IO.Directory;
using _Path = System.IO.Path;

namespace FPlug
{
    public class Dir
    {
        // Path
        private string path;

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                if (path != value)
                {
                    path = value;
                    IsCached = false;
                }
            }
        }

        public DirectoryInfo Directory { get; private set; }

        // Cache
        public bool IsCached { get; private set; }

        private IEnumerable<string> folders;

        public IEnumerable<string> Folders
        {
            get
            {
                if (!IsCached)
                    folders = _Dir.GetDirectories(Path).Select(p => _Path.GetFileName(p)).ToList();
                return folders;
            }
        }

        private IEnumerable<string> files;

        public IEnumerable<string> Files
        {
            get
            {
                if (!IsCached)
                    files = _Dir.GetFiles(Path).Select(p => _Path.GetFileName(p)).ToList();
                return files;
            }
        }

        // Ctor
        public Dir(string dir)
        {
            Path = dir;
        }

        // Static
        public static bool TryGet(string path, out Dir outDir)
        {
            if (_Dir.Exists(path))
            {
                outDir = new Dir(path);
                return true;
            }
            outDir = null;
            return false;
        }

        // Directory
        string firstDirMatch(string dir)
        {
            return Folders.FirstOrDefault((s) => string.Equals(dir, s, StringComparison.OrdinalIgnoreCase));
        }
        
        public bool TryGetDir(string dir, out Dir outDir)
        {
            string d = firstDirMatch(dir);
            if (d == null)
            {
                outDir = null;
                return false;
            }
            outDir = new Dir(_Path.Combine(Path, d));
            return true;
        }
        
        public bool ContainsDir(string dir)
        {
            return firstDirMatch(dir) != null;
        }

        // File
        string firstFileMatch(string dir)
        {
            return Files.FirstOrDefault((s) => string.Equals(dir, s, StringComparison.OrdinalIgnoreCase));
        }

        public bool TryGetFile(string dir, out string outFile)
        {
            string d = firstFileMatch(dir);
            if (d == null)
            {
                outFile = null;
                return false;
            }
            outFile = _Path.Combine(Path, d);
            return true;
        }

        public bool ContainsFile(string dir)
        {
            return firstFileMatch(dir) != null;
        }
    }
}
