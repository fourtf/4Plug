using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Xwt;

namespace FPlug.Widgets
{
    public class DownloadHudWidget : PluginLayoutWidget
    {
        public event EventHandler<DownloadDataCompletedEventArgs> DownloadFinished;

        Label progressLabel;
        WebClient client;

        public DownloadHudWidget(string title)
        {
            PluginType = FPlug.PluginType.Downloading;
            HasBorder = true;

            progressLabel = new Label();
            progressLabel.MinWidth = 400;
            progressLabel.WidthRequest = 400;
            progressLabel.Text = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

            AddChild(new Label(title), 16, 8);

            AddChild(progressLabel, new Rectangle(16, 24, 400, 20));

            client = new WebClient() { Proxy = null };

            client.DownloadProgressChanged += (s, e) =>
			{
                Application.Invoke(() => progressLabel.Text = (e.BytesReceived < 1048576 ? e.BytesReceived / 1024 + " KiB: " : (float)(int)(((float)e.BytesReceived / 1048576) * 100) / 100 + " MiB: ") + (e.ProgressPercentage == 0 ? "?%" : e.ProgressPercentage + "%"));
			};

            client.DownloadDataCompleted += (s, e) =>
            {
                if (!e.Cancelled)
                {
                    Application.Invoke(() => progressLabel.Text = "Download Completed!");
                    if (DownloadFinished != null)
                        DownloadFinished(this, e);
                }
            };

            // client.DownloadFileAsync(new Uri("https://github.com/Sevin7/7HUD/archive/master.zip"), "7hud.zip");
        }

        public override bool CanClose()
        {
            client.CancelAsync();
            return base.CanClose();
        }

        public void Download(string url)
        {
            if (client.IsBusy)
                throw new InvalidOperationException("Webclient is busy.");

            client.DownloadDataAsync(new Uri(url));
        }
    }
}
