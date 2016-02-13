using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Xwt;

namespace FPlug.Widgets
{
    public class UpdateAvailableWidget : PluginLayoutWidget
    {
        public UpdateAvailableWidget(FVersion version)
        {
            PluginType = PluginType.None;

            if (!File.Exists("4PlugUpdate.exe"))
            {
                Label lbl = new Label("4Plug can't update because the \"4PlugUpdate.exe\" is missing! Redownload 4Plug to fix this issue.");
                AddChild(lbl, 4, 2);
            }
            else
            {
                string text = "An update (v{0})) is available! (Download {1}%)          ";

                Label lbl = new Label(string.Format(text, version.ToString(), 0));
                AddChild(lbl, 4, 2);

                Button btn = new Button(" Install now ");
                btn.Sensitive = false;
                AddChild(btn, 4, 20);

                var client = new WebClient();
                client.Proxy = null;

                client.DownloadProgressChanged += (s, e) => { lbl.Text = string.Format(text, version.ToString(), e.ProgressPercentage); };
                client.DownloadFileCompleted += (s, e) =>
                {
                    lbl.Text = string.Format("An update (v{0})) is available!", version.ToString());
                    btn.Sensitive = true;
                    App.ApplyUpdate = true;
                };
                client.DownloadFileAsync(new Uri(App.WinUpdateUrl), "_update.zip");

                btn.Clicked += (s, e) => { App.MainWindow.Close(); };
            }

            HasBorder = true;
            HasBorderLineDash = true;
        }
    }
}
