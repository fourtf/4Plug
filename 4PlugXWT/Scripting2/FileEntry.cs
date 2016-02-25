using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _Path = System.IO.Path;

namespace FPlug.Scripting2
{
    public class FileEntry
    {
        Type currentCacheType = null;
        ResourceFile currentResourceFile = null;

        public string Path { get; private set; }
        public string FileName { get; private set; }

        public FileEntry(string path)
        {
            Path = path;
            FileName = _Path.GetFileName(path);
        }

        public T GetCache<T>() where T : ResourceFile, new()
        {
            Type t = typeof(T);

            if (currentResourceFile != null)
            {
                if (currentCacheType == t)
                    return (T)currentResourceFile;
                else
                {
                    currentResourceFile.Save();
                }
            }

            currentResourceFile = new T();
            currentResourceFile.Filename = FileName;
            currentResourceFile.Init();
            currentCacheType = t;

            return (T)currentResourceFile;
        }

        public void SaveCache()
        {
            if (currentResourceFile != null)
                currentResourceFile.Save();
        }

        public override string ToString()
        {
            return $"File: " + FileName;
        }
    }

    public abstract class ResourceFile
    {
        public string Filename { get; set; }

        public abstract void Init();
        public abstract void Save();    
    }
}
