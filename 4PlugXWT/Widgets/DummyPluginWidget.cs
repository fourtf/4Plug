using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class DummyPluginWidget : PluginWidget
    {
        Label lbl;

        public DummyPluginWidget(Label lbl)
            : base("Name of the Plugin")
        {
            this.lbl = lbl;
            PluginType = FPlug.PluginType.None;
        }

        Image thumb1;
        Image thumb2;

        public override void Load()
        {
            thumb1 = Resources.GetImage("tutorial_thumb.png");
            thumb2 = Resources.GetImage("tutorial_thumb2.png");

            pluginImage.Image = thumb1;

            lblAuthor.Text = "Author";
            lblVersion.Text = "Version";

            linksQuad.AddLink(App.SteamGroup);
            linksQuad.AddLink(App.TfTvThread);

            btnChangelog.Visible = btnFaq.Visible = btnReadme.Visible = true;

            btnChangelog.MouseEntered += (s, e) => { lbl.Text = "Show Changelog"; };
            btnChangelog.MouseExited += (s, e) => { lbl.Text = ""; };
            btnFaq.MouseEntered += (s, e) => { lbl.Text = "Show Faq"; };
            btnFaq.MouseExited += (s, e) => { lbl.Text = ""; };
            btnReadme.MouseEntered += (s, e) => { lbl.Text = "Show Readme"; };
            btnReadme.MouseExited += (s, e) => { lbl.Text = ""; };

            linksQuad.MouseEntered += (s, e) => { lbl.Text = "Links"; };
            linksQuad.MouseExited += (s, e) => { lbl.Text = ""; };

            openFolderBtn.MouseEntered += (s, e) => { lbl.Text = "Open the Mod in the File Explorer"; };
            openFolderBtn.MouseExited += (s, e) => { lbl.Text = ""; };

            moreActionsBtn.MouseEntered += (s, e) => { lbl.Text = "More Actions"; };
            moreActionsBtn.MouseExited += (s, e) => { lbl.Text = ""; };

            pluginImage.MouseEntered += (s, e) => { lbl.Text = "Install/Uninstall the Mod"; };
            pluginImage.MouseExited += (s, e) => { lbl.Text = ""; };

            settingsBtn.Sensitive = true;
            settingsBtn.Image = imgSettings;

            lblName.MouseEntered += (s, e) => { lbl.Text = "Name of the Mod"; };
            lblName.MouseExited += (s, e) => { lbl.Text = ""; };

            lblAuthor.MouseEntered += (s, e) => { lbl.Text = "Author of the Mod"; };
            lblAuthor.MouseExited += (s, e) => { lbl.Text = ""; };

            lblVersion.MouseEntered += (s, e) => { lbl.Text = "Version of the Mod"; };
            lblVersion.MouseExited += (s, e) => { lbl.Text = ""; };

            settingsBtn.MouseEntered += (s, e) => { lbl.Text = "Open the Settings of the Hud (if available)"; };
            settingsBtn.MouseExited += (s, e) => { lbl.Text = ""; };
        }

        public override bool IsDirectory
        {
            get { return true; }
        }

        public override void ShowSettings()
        {

        }

        public override bool InstallOrUninstall()
        {
            Installed = !Installed;
            pluginImage.Image = !Installed ? thumb1 : thumb2;
            return true;
        }

        protected override bool OnDelete()
        {
            return false;
        }

        public override bool CreateCopy()
        {
            return false;
        }

        public override bool Rename(string name)
        {
            return false;
        }
    }
}
