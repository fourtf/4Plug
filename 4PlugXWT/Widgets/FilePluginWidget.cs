using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using _Path = System.IO.Path;

namespace FPlug.Widgets
{
    public class FilePluginWidget : PluginWidget
    {
        public FilePluginWidget(string path)
            : base (path)
        {
            Name = path.GetFileName().RemoveFromRight(4);
            lblName.Text = Name + new string(' ', 30);

            moreActionsBtn.Click += (s, e) => { createCopy.Visible = false; };
        }

        public override void Load()
        {
            Dir dir = new Dir(new FileInfo(Path).Directory.FullName);
            string tmp;

            if (dir.TryGetFile(_Path.GetFileNameWithoutExtension(Path) + ".plugin.xml", out tmp))
                LoadXml(tmp, false);
        }

        protected override PluginType GetPluginType()
        {
            return (Path.ToUpper().EndsWith(".VPK")) ? FPlug.PluginType.Vpk : FPlug.PluginType.Unknown;
        }

        public override bool IsDirectory
        {
            get
            {
                return false;
            }
        }

        public override void ShowSettings()
        {
            
        }

        public override bool InstallOrUninstall()
        {
            string oldCustom = Installed ? App.PathCustom : App.PathCustom_;
            string newCustom = Installed ? App.PathCustom_ : App.PathCustom;

            Dir oldDir = new Dir(oldCustom);
            Dir newDir = new Dir(newCustom);

            string tmp;

            List<string> files = new List<string>();

            var p = _Path.GetFileNameWithoutExtension(Path) + ".";

            string newPath = _Path.Combine(newCustom, _Path.GetFileName(Path));

            foreach (string s in oldDir.Files)
            {
                if (s.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                {
                    if (newDir.TryGetFile(s, out tmp))
                        return false;

                    files.Add(s);
                }
            }

            if (files.Count == 0)
                return false;

            try
            {
                foreach (string s in files)
                    File.Move(_Path.Combine(oldCustom, s), _Path.Combine(newCustom, s));
                Path = newPath;
                Installed = !Installed;
                return true;
            }
            catch
            {

            }
                
            return false;
        }

        public override bool CreateCopy()
        {
            return false;
        }

        protected override bool OnDelete()
        {
            string custom = Installed ? App.PathCustom : App.PathCustom_;

            Dir dir = new Dir(custom);
            string p = Name + ".";

            try
            {
                foreach (string s in dir.Files)
                {
                    if (s.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                        File.Delete(_Path.Combine(custom, s));
                }
            }
            catch { }

            return false;
        }

        public override bool Rename(string name)
        {
            string custom = Installed ? App.PathCustom : App.PathCustom_;

            Dir dir = new Dir(custom);
            string tmp;
            List<Tuple<string, string>> files = new List<Tuple<string, string>>();
            string p = Name + ".";

            foreach (string s in dir.Files)
            {
                if (s.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                {
                    string newFile = name + "." + s.Substring(p.Length);

                    if (dir.TryGetFile(newFile, out tmp))
                        return false;

                    files.Add(Tuple.Create(_Path.Combine(custom, s), _Path.Combine(custom, newFile)));
                }
            }

            if (files.Count == 0)
                return false;

            try
            {
                foreach (var x in files)
                    File.Move(x.Item1, x.Item2);
                Name = name;
                lblName.Text = name + new string(' ', 30);
                return true;
            }
            catch
            {

            }

            return false;
        }
    }
}
