using FPlug.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class DirectoryPluginWidget : PluginWidget
    {
        SettingsWindow settingsWindow = null;

        public DirectoryPluginWidget(string path)
            : base(path)
        {
            moreActionsBtn.Click += (s, e) => { createCopy.Visible = true; };
        }

        public override void Load()
        {
            Dir dir = new Dir(Path);

            string tmp;
            if (dir.TryGetFile("thumbnail.png", out tmp) || dir.TryGetFile("thumbnail40.png", out tmp))
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

            if (dir.TryGetFile("plugin.xml", out tmp))
            {
                pluginDotXmlPath = "plugin.xml";
                LoadXml(tmp);
            }
            else if (dir.TryGetFile("data.txt", out tmp))
            {
                ProcessDataTxt(File.ReadAllText(tmp));
            }

            if (dir.TryGetFile("readme.md", out tmp) || dir.TryGetFile("readme.txt", out tmp))
            {
                readmePath = tmp;
                btnReadme.Visible = true;
            }
            if (dir.TryGetFile("faq.md", out tmp) || dir.TryGetFile("faq.txt", out tmp))
            {
                faqPath = tmp;
                btnFaq.Visible = true;
            }
            if (dir.TryGetFile("changelog.md", out tmp) || dir.TryGetFile("changelog.txt", out tmp))
            {
                changelogPath = tmp;
                btnChangelog.Visible = true;
            }
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

        public override bool CanClose()
        {
            if (settingsWindow == null)
                return true;

            settingsWindow.Present();
            return false;
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
            if (SettingsActive)
            {
                if (settingsWindow != null)
                    settingsWindow.Present();
            }
            else
            {
                settingsWindow = new FPlug.Options.SettingsWindow(System.IO.Path.Combine(Path, pluginDotXmlPath), Path, true);
                MainWindow.OpenWindows.Add(settingsWindow);
                SettingsActive = true;
                settingsWindow.Closed += (s, e) => { SettingsActive = false; MainWindow.OpenWindows.Remove(settingsWindow); };
                settingsWindow.Show();
            }
        }

        public override bool InstallOrUninstall()
        {
            if (SettingsActive)
            {
                if (settingsWindow != null)
                    settingsWindow.Present();
                return false;
            }
            else
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
        }

        public override bool CreateCopy()
        {
            try
            {
                string path = App.MakePluginDirectoryUnique(Path);
                Util.CopyDirectory(Path, path);
                MainWindow.AddDirectoryPlugin(path, Installed, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool OnDelete()
        {
            if (SettingsActive)
            {
                if (settingsWindow != null)
                    settingsWindow.Present();
                return false;
            }
            else
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
        }

        public override void StartRename()
        {
            if (SettingsActive)
            {
                if (settingsWindow != null)
                    settingsWindow.Present();
            }
            else
                base.StartRename();
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
