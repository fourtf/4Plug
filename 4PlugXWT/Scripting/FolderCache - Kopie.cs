using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using _Path = System.IO.Path;

namespace FPlug.Scripting
{
    //
    //  This class desperately should definitely be refactored. Whatever ¯\_(ツ)_/¯
    //

    public class FolderCache
    {
        public FolderCache Parent = null;

        private List<FolderCache> folders = null;

        public List<FolderCache> Folders
        {
            get
            {
                if (files == null)
                    Load();
                return folders;
            }
        }

        private List<string> files = null;

        public List<string> Files
        {
            get
            {
                if (files == null)
                    Load();
                return files;
            }
        }

        public string Path { get; private set; }
        public string FullPath { get; private set; }

        public bool Loaded { get; private set; }

        public FolderCache(string folder)
            : this(folder, true)
        {

        }

        public FolderCache(string folder, bool preload = true)
        {
            Path = _Path.GetFileName(folder.TrimEnd(_Path.DirectorySeparatorChar, _Path.AltDirectorySeparatorChar));
            FullPath = folder;

            if (preload)
                Load();

            foreach (string s in Directory.EnumerateDirectories(folder))
            {
                Folders.Add(new FolderCache(s, false) { Parent = this });
            }

            foreach (string s in Directory.EnumerateFiles(folder))
            {
                Files.Add(_Path.GetFileName(s));
            }
        }

        static char[] dirSepChars = new[] { '\\', '/' };

        public bool TryCopyFile(string from, string to)
        {
            try
            {
                return jizzinmyface(from, to, false, (s1, s2) =>
                    {
                        if (!File.Exists(s1))
                            throw new Exception();
                        if (File.Exists(s2))
                            File.Delete(s2);
                        File.Copy(s1, s2);
                    });
            }
            catch { return false; }
        }

        public bool TryMoveFile(string from, string to)
        {
            try
            {
                return jizzinmyface(from, to, false, (s1, s2) =>
                    {
                        if (!File.Exists(s1))
                            throw new Exception();
                        if (File.Exists(s2))
                            File.Delete(s2);
                        File.Move(s1, s2);
                    });
            }
            catch { return false; }
        }

        public bool TryCopyDirectory(string from, string to)
        {
            try
            {
                return jizzinmyface(from, to, true, (s1, s2) => 
                    {
                        if (!Directory.Exists(s1))
                            throw new Exception();
                        if (Directory.Exists(s2))
                            Directory.Delete(s2, true);
                        Util.CopyDirectory(s1, s2);
                    });
            }
            catch { return false; }
        }

        public bool TryMoveDirectory(string from, string to)
        {
            try
            {

                return jizzinmyface(from, to, true, (s1, s2) =>
                    {
                        if (!Directory.Exists(s1))
                            throw new Exception();
                        if (Directory.Exists(s2))
                            Directory.Delete(s2, true);
                        Directory.Move(s1, s2);
                    });
            }
            catch { return false; }
        }

        public bool FileExists(string path)
        {
            try
            {
                return TryResolvePath(path, false) != null;
            }
            catch { return false; }
        }

        public bool DirectoryExists(string path)
        {
            try
            {
                return TryResolvePath(path, true) != null;
            }
            catch { return false; }
        }

        public bool DeleteFile(string path)
        {
            try
            {
                string s = TryResolvePath(path, false);
                if (s == null) return false;
                if (!File.Exists(s)) return true;
                File.Delete(s);

                RemovePath(path, false);

                return true;
            }
            catch { return false; }
        }

        public bool DeleteDirectory(string path)
        {
            try
            {
                string s = TryResolvePath(path, true);
                if (s == null) return false;
                if (!Directory.Exists(s)) return true;
                Directory.Delete(s, true);
                RemovePath(path, true);
                return true;
            }
            catch { return false; }
        }

        private bool jizzinmyface(string yeah, string I, bool like, Action<string, string> that)
        {
            var s1 = TryResolvePath(yeah, like);
            var s2 = TryResolvePath(I, like);

            if (s1 == null)
                return false;
            if (s2 == null)
            {
                AddPath(I, like);
                s2 = TryResolvePath(I, like);
            }
            try
            {
                that(s1, s2);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public FolderCache GetRootFromFile(string file)
        {
            if (file[0] == '/' || file[0] == '\\')
                file = file.Substring(1);

            var S = file.Split(dirSepChars);

            if (S.Length == 0)
                return null;

            FolderCache cache = this;

            //path = "";

            for (int i = 0; i < S.Length - 1; i++)
            {
                cache = cache.Folders.FirstOrDefault(p => string.Equals(p.Path, S[i], StringComparison.OrdinalIgnoreCase));
                if (cache == null)
                    return null;
                //path += cache.Path + _Path.DirectorySeparatorChar;
            }

            return cache;
        }

        public FolderCache GetFolder(string file)
        {
            if (file[0] == '/' || file[0] == '\\')
                file = file.Substring(1);

            var S = file.Split(dirSepChars);

            if (S.Length == 0)
                return null;

            FolderCache cache = this;

            //path = "";

            for (int i = 0; i < S.Length; i++)
            {
                cache = cache.Folders.FirstOrDefault(p => string.Equals(p.Path, S[i], StringComparison.OrdinalIgnoreCase));
                if (cache == null)
                    return null;
                //path += cache.Path + _Path.DirectorySeparatorChar;
            }

            return cache;
        }

        public void RemovePath(string path, bool isDirectory)
        {
            try
            {
                if (path[0] == '/' || path[0] == '\\')
                    path = path.Substring(1);

                var S = path.Split(dirSepChars);

                //if (S.Length == 0)
                //    return;

                FolderCache cache = this;

                //path = "";

                for (int i = 0; i < S.Length - 1; i++)
                {
                    cache = cache.Folders.FirstOrDefault(p => string.Equals(p.Path, S[i], StringComparison.OrdinalIgnoreCase));
                    //if (cache == null)
                    //    return;
                    //path += cache.Path + _Path.DirectorySeparatorChar;
                }

                if (isDirectory)
                    cache.Folders.RemoveAt(cache.Folders.FindIndex(p => string.Equals(p.Path, S[S.Length - 1], StringComparison.OrdinalIgnoreCase)));
                else
                    cache.Files.RemoveAt(cache.Files.FindIndex(p => string.Equals(p, S[S.Length - 1], StringComparison.OrdinalIgnoreCase)));
            }
            catch
            {

            }
        }

        public void AddPath(string path, bool isDirectory)
        {
            try
            {
                if (path[0] == '/' || path[0] == '\\')
                    path = path.Substring(1);

                var S = path.Split(dirSepChars);

                //if (S.Length == 0)
                //    return;

                FolderCache cache = this;

                //path = "";

                for (int i = 0; i < S.Length - (isDirectory ? 0 : 1); i++)
                {
                    FolderCache cache2 = cache.Folders.FirstOrDefault(p => string.Equals(p.Path, S[i], StringComparison.OrdinalIgnoreCase));
                    if (cache2 == null)
                    {
                        string pth = FullPath;
                        for (int j = 0; j < i + 1; j++)
                            pth = _Path.Combine(pth, S[j]);
                        if (!Directory.Exists(pth))
                            Directory.CreateDirectory(pth);
                        cache.Folders.Add(cache2 = new FolderCache(pth));
                    }
                    cache = cache2;
                    path += cache.Path + _Path.DirectorySeparatorChar;
                }

                if (!isDirectory)
                    cache.Files.Add(S[S.Length - 1]);
            }
            catch { }
        }

        public string TryResolvePath(string path, bool isDirectory)
        {
            if (path.Length == 0)
                return null;

            if (path[0] == '/' || path[0] == '\\')
                path = path.Substring(1);

            if (path.Length == 0)
                return null;

            var S = path.Split(dirSepChars);

            if (S.Length == 0)
                return null;

            FolderCache cache = this;

            path = "";

            for (int i = 0; i < S.Length - (isDirectory ? 0 : 1); i++)
            {
                cache = cache.Folders.FirstOrDefault(p => string.Equals(p.Path, S[i], StringComparison.OrdinalIgnoreCase));
                if (cache == null)
                    return null;
                path += cache.Path + _Path.DirectorySeparatorChar;
            }

            if (!isDirectory)
            {
                var s = cache.Files.FirstOrDefault(p => string.Equals(p, S[S.Length - 1], StringComparison.OrdinalIgnoreCase));
                if (s == null)
                    return null;
                path += s;
            }
            return _Path.Combine(FullPath, path);
        }

        private void Load()
        {
            if (!Loaded)
            {
                folders = new List<FolderCache>();
                files = new List<string>();

                Loaded = true;
            }
        }

        public override string ToString()
        {
            return "Folder: " + Path;
        }
    }
}
