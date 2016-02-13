using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Xwt.Drawing;
namespace FPlug
{
    public class DirectoryPluginWidget : PluginWidget
    {
        public DirectoryPluginWidget(string path)
            : base(path)
        {

        }

        public override void Load()
        {
            string tmp;
            if (File.Exists(tmp = System.IO.Path.Combine(Path, "thumbnail40.png")))
            {
                try
                {
                    pluginImage.Image = Image.FromFile(tmp);
                }
                catch { }
            }
            else
            {
                pluginImage.Image = noImage;
            }

            if (File.Exists(tmp = System.IO.Path.Combine(Path, "plugin.xml")))
            {
                pluginDotXmlPath = "plugin.xml";
                LoadXml(tmp);
            }
            else if (File.Exists(tmp = System.IO.Path.Combine(Path, "data.txt")))
            {
                ProcessDataTxt(File.ReadAllText(tmp));
            }

            settingsBtn.Sensitive = true;
        }

        protected override PluginType GetPluginType()
        {
            //Hud
            if (Directory.Exists(System.IO.Path.Combine(Path, "resource", "ui")))
                return PluginType.Hud;

            //Hitsounds
            if (File.Exists(System.IO.Path.Combine(Path, "sound", "ui", "hitsound.wav")))
                return PluginType.Hitsound;

            //Addon
            string tmp;
            if (Directory.Exists(tmp = System.IO.Path.Combine(Path, "addons")))
            {
                foreach (string s in Directory.GetFiles(tmp))
                {
                    if (s.EndsWith(".dll"))
                        return PluginType.Addon;
                }
            }

            //Something else
            return PluginType.Unknown;
        }

        public override bool IsDirectory
        {
            get
            {
                return true;
            }
        }

        public override void ShowSettings()
        {
            new FPlug.Options.SettingsWindow(System.IO.Path.Combine(Path, pluginDotXmlPath), Path, true).Show();
        }

        public override bool InstallOrUninstall()
        {
            string newPath = System.IO.Path.Combine((Installed ? App.PathCustom_ : App.PathCustom), Name);

            if (!Directory.Exists(Path) || Directory.Exists(newPath))
                return false;

            try
            {
                Directory.Move(Path, newPath);
                Path = newPath;
                Installed = !Installed;
                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine("InstallOrUninstall(): " + exc.Message);
            }

            return false;
        }

        public override bool CreateCopy()
        {
            try
            {
                string path = App.MakePluginDirectoryUnique(Path);
                Util.CopyDirectory(Path, path);
                MainWindow.AddDirectoryPlugin(path, Installed);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool Delete()
        {
            if (App.CustomDelete != null)
                return App.CustomDelete(Path);
            else
                try
                {
                    Directory.Delete(Path, true);
                    MainWindow.RemovePlugin(this);
                    return true;
                }
                catch (Exception exc)
                {
                    exc.Message.Log();
                    return false;
                }
        }

        public override bool Rename(string name)
        {
            name = name.Trim();
            
            if (name == Name)
                return true;

            if (Directory.Exists(System.IO.Path.Combine(App.PathCustom, name)) || Directory.Exists(System.IO.Path.Combine(App.PathCustom_, name)))
                return false;

            try
            {
                string pth = System.IO.Path.Combine((Installed ? App.PathCustom : App.PathCustom_), name);
                Directory.Move(Path, pth);
                lblName.Text = name;
                Name = name;
                Path = pth;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
