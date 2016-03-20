using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using _Path = System.IO.Path;

namespace FPlug.Options.IO
{
    public class FolderCache
    {
        // static
        static char[] dirSepChars = new[] { '\\', '/' };


        // public fields
        public string DirectoryName { get; private set; }
        public string FullPath { get; private set; }

        public FolderCache Parent = null;


        // Folders
        private List<FolderCache> folders = null;

        public List<FolderCache> Folders
        {
            get
            {
                if (folders == null)
                {
                    folders = new List<FolderCache>();
                    foreach (string s in Directory.EnumerateDirectories(FullPath))
                    {
                        folders.Add(new FolderCache(s) { Parent = this });
                    }
                }
                return folders;
            }
        }

        // Files
        private List<FileEntry> files = null;

        public List<FileEntry> Files
        {
            get
            {
                if (files == null)
                {
                    files = new List<FileEntry>();
                    foreach (string s in Directory.EnumerateFiles(FullPath))
                    {
                        files.Add(new FileEntry(s));
                    }
                }
                return files;
            }
        }


        // ctor
        public FolderCache(string folder)
            : this(folder, false)
        {

        }

        public FolderCache(string folder, bool preload = false)
        {
            DirectoryName = _Path.GetFileName(folder.TrimEnd(_Path.DirectorySeparatorChar, _Path.AltDirectorySeparatorChar));
            FullPath = folder;

            if (preload)
            {
                var a = Folders;
                var b = Files;
            }
        }


        // Get Files / Folders
        public FolderCache GetFolder(string path)
        {
            var S = path.Split(dirSepChars);

            FolderCache currentCache = this;
            foreach (string s in S)
            {
                foreach (FolderCache f in currentCache.Folders)
                {
                    if (f.DirectoryName.Equals(s, StringComparison.OrdinalIgnoreCase))
                    {
                        currentCache = f;
                        goto e;
                    }
                }
                currentCache = null;
                break;
            e:;
            }

            if (currentCache == null)
                return null;

            return currentCache;
        }

        public FileEntry GetFile(string path)
        {
            FolderCache cache;

            string filename = path;

            int index;
            if ((index = path.LastIndexOfAny(dirSepChars)) == -1)
                cache = this;
            else
            {
                cache = GetFolder(path.Remove(index));
                filename = path.Substring(index + 1);
            }

            if (cache == null)
                return null;

            foreach (FileEntry e in cache.Files)
            {
                if (e.FileName.Equals(filename, StringComparison.OrdinalIgnoreCase))
                    return e;
            }

            return null;
        }


        // Modify Files
        public bool DeleteFile(string path)
        {
            FolderCache cache;

            string filename = path;

            int index;
            if ((index = path.LastIndexOfAny(dirSepChars)) == -1)
                cache = this;
            else
            {
                cache = GetFolder(path.Remove(index));
                filename = path.Substring(index + 1);
            }

            if (cache == null)
                return false;

            foreach (FileEntry e in cache.Files)
            {
                if (e.FileName.Equals(filename, StringComparison.OrdinalIgnoreCase))
                {
                    e.SaveCache();
                    File.Delete(e.Path);
                    cache.Files.Remove(e);
                    return true;
                }
            }

            return false;
        }

        public bool MoveFile(string from, string to, bool _override = true)
        {
            return _copyFile(from, to, false, _override);
        }

        public bool CopyFile(string from, string to, bool _override = true)
        {
            return _copyFile(from, to, true, _override);
        }

        private bool _copyFile(string from, string to, bool copy, bool _override)
        {
            FileEntry From = GetFile(from);

            if (From == null)
                return false;

            FileEntry To = GetFile(to);

            if (To != null)
            {
                if (!_override)
                    return false; // can't override
                else
                {
                    To.SaveCache();
                }
            }

            if (copy)
            {
                File.Copy(From.Path, To.Path, _override);
                return true;
            }
            else
            {
                File.Delete(To.Path);
                File.Move(From.Path, To.Path);
                DeleteFile(from);
                return true;
            }
        }


        // Modify Directories
        public bool DeleteDirectory(string path)
        {
            FolderCache cache = GetFolder(path);

            if (cache == null)
                return false;

            cache.disposeFolders();

            Directory.Delete(cache.FullPath);
            cache.Parent.Folders.Remove(cache);

            return true;
        }

        //public bool MoveDirectory(string from, string to, bool _override)
        //{
        //
        //}

        private void disposeFolders()
        {
            if (folders != null)
                foreach (var f in Folders)
                {
                    f.disposeFolders();
                }
            if (files != null)
                foreach (var f in Files)
                {
                    f.SaveCache();
                }
        }


        // override
        public override string ToString()
        {
            return "Folder: " + DirectoryName;
        }
    }
}
