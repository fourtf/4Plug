using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class HudsTFWidget : Canvas
    {
        public ImageView ImageView { get; private set; }

        public static readonly Size ImageSize = new Size(253, 200);

        public static Image HudsTfLogo = Resources.GetImage("hudstf.png");
        public static Image GithubLogo = Resources.GetImage("github.png");

        //Resources.GetImage("tftv.png"), Resources.GetImage("etf2l.png"), Resources.GetImage("steam.png"),

        Button dlButton = null;
        ImageButton hudstfButton = null;
        ImageButton websiteButton = null;
        ImageButton githubButton = null;

        Label titleLabel = null;

        public HudsTFWidget(string title, string creator, string imgUrl, string hudstfUrl, string download, string website)
        {
            var downloadLower = download.ToLower();

            AddChild(ImageView = new ImageView() { WidthRequest = ImageSize.Width, HeightRequest = ImageSize.Height }, new Rectangle(1, 1, ImageSize.Width, ImageSize.Height));
            AddChild(titleLabel = new Label(title + " by " + creator) { TextColor = Colors.White }, new Rectangle(4, 0, ImageSize.Width, 40));

            titleLabel.Font = titleLabel.Font.WithScaledSize(1.5);
            titleLabel.WidthRequest = ImageSize.Width;
            titleLabel.HeightRequest = 40;
            titleLabel.QueueForReallocate();

            WidthRequest = ImageSize.Width + 2;
            HeightRequest = ImageSize.Height + 39;

            if (hudstfUrl != null)
            {
                hudstfButton = new ImageButton(57, 16, HudsTfLogo);
                hudstfButton.Click += (s, e) => { Desktop.OpenUrl(hudstfUrl); };

                AddChild(hudstfButton, ImageSize.Width - 64, ImageSize.Height + 11);
                //AddChild(hudstfButton, ImageSize.Width - 61, 24);
            }

            if (website != null)
            {
                websiteButton = new ImageButton(16, 16, QuadImageLinkButton.DefaultImage);
                websiteButton.Click += (s, e) => { Desktop.OpenUrl(website); };

                AddChild(websiteButton, ImageSize.Width - 84, ImageSize.Height + 11);
                //AddChild(websiteButton, ImageSize.Width - 81, 24);
            }

            bool hasDirectDownload = true;
            string zipLink = null;
            
            //string zipLink = download.Contains("github.com") || download.Contains("gitgud.net");
            if (downloadLower.Contains("github.com"))
                zipLink = download.TrimEnd('/') + "/archive/master.zip";
            else if (downloadLower.Contains("gitgud.net"))
                zipLink = download.TrimEnd('/') + "/repository/archive.zip";
            else
                hasDirectDownload = false;


            if (hasDirectDownload)
            {
                githubButton = new ImageButton(16, 16, GithubLogo);
                githubButton.Click += (s, e) => { Desktop.OpenUrl(download); };

                AddChild(githubButton, ImageSize.Width - 104, ImageSize.Height + 11);
            }

            dlButton = new Button();
            dlButton.Label = hasDirectDownload ? "Install" : "Get";
            dlButton.WidthRequest = 120;
            dlButton.HeightRequest = 24;
            //dlButton.Sensitive = downloadIsGithub;
            //https://gitgud.net/JediThug/jedihud/repository/archive.zip

            dlButton.Clicked += (s, e) =>
            {
                if (hasDirectDownload)
                {
                    DownloadHudWidget dl = new DownloadHudWidget("Fetching " + zipLink);

                    dl.DownloadFinished += (s2, e2) =>
                    {
                        try
                        {
                            if (!App.InstallZip(new MemoryStream(e2.Result), title))
                                MessageDialog.ShowError("Installing the Hud failed", "You can try to install it yourself.");
                        }
                        catch { }
                        Application.Invoke(() => { App.MainWindow.RemovePlugin(dl); });//App.MainWindow.Layout.RemoveChild(this));
                        Dispose();
                    };

                    dl.Download(zipLink);

                    App.MainWindow.Present();
                    System.Threading.Thread.Sleep(50);
                    App.MainWindow.Layout.AddChild(dl);
                }
                else
                {
                    Desktop.OpenUrl(download);
                }
            };

            AddChild(dlButton, 7, ImageSize.Height + 7);
            //AddChild(dlButton, 4, 20);

            Margin = new WidgetSpacing(4,4,4,4);

            //OnMouseExited(null);
        }

        //protected override void OnMouseEntered(EventArgs args)
        //{
        //    base.OnMouseEntered(args);
        //
        //    if (githubButton != null)
        //        githubButton.Visible = true;
        //    if (websiteButton != null)
        //        websiteButton.Visible = true;
        //    if (dlButton != null)
        //        dlButton.Visible = true;
        //    if (hudstfButton != null)
        //        hudstfButton.Visible = true;
        //}
        //
        //protected override void OnMouseExited(EventArgs args)
        //{
        //    base.OnMouseExited(args);
        //
        //    if (githubButton != null)
        //        githubButton.Visible = false;
        //    if (websiteButton != null)
        //        websiteButton.Visible = false;
        //    if (dlButton != null)
        //        dlButton.Visible = false;
        //    if (hudstfButton != null)
        //        hudstfButton.Visible = false;
        //}

        protected override void OnDraw(Context ctx, Rectangle dirtyRect)
        {
            base.OnDraw(ctx, dirtyRect);

            ctx.SetColor(Color.FromBytes(240, 240, 240));
            ctx.Rectangle(1, 1, WidthRequest - 2, HeightRequest - 2);
            //ctx.SetLineWidth(1);
            ctx.Fill();

            ctx.SetColor(Color.FromBytes(190, 190, 190));
            ctx.Rectangle(0, 0, WidthRequest, HeightRequest);
            //ctx.SetLineWidth(1);
            ctx.Stroke();
        }
    }


    public class HudsTFDisplay : Window
    {
        WidgetLayout<HudsTFWidget> Layout = new WidgetLayout<HudsTFWidget>();

        public HudsTFDisplay()
        {
            Icon = App.Icon;

            Layout.ItemWidth = HudsTFWidget.ImageSize.Width;

            Content = new ScrollView(Layout);
            new Task(() => Load()).Start();

            Title = "Huds.TF";
            Width = 900;
            Height = 600;
        }

        void Load()
        {
            string content;
            try
            {
                WebClient client = new WebClient() { Proxy = null };
                content = client.DownloadString("http://huds.tf/directory");

                //File.WriteAllText("hudstfcache", hudstf);

                //content = File.ReadAllText("hudstfcache");

                Regex findDiv = new Regex(@"<div class=""hud-box"">(.*?)</nav>\s+</div>", RegexOptions.Compiled | RegexOptions.Singleline);
                Regex findTitle = new Regex(@"""hud-title"">([^<]+)", RegexOptions.Compiled);
                Regex findCreator = new Regex(@"""hud-creator"">([^<]+)", RegexOptions.Compiled);
                Regex findImg = new Regex(@"""img/main/([^""]+)", RegexOptions.Compiled);
                Regex findHudsTf = new Regex(@"href=""/([^""]+)", RegexOptions.Compiled);
                Regex findDownload = new Regex(@"<a href=""(http[^""]+)[^<]+Download", RegexOptions.Compiled);
                Regex findWebsite = new Regex(@"<a href=""(http[^""]+)[^<]+Website", RegexOptions.Compiled);

                //<li><a href="/7hud">Huds.tf Page</a></li>
                //<li><a href="http://sevin7.github.io/7HUD/" target="_blank">Website</a></li>
                //<li><a href="https://github.com/Sevin7/7HUD" target="_blank">Download</a></li>

                foreach (Match div in findDiv.Matches(content))
                {
                    var divContent = div.Groups[1].Value;

                    var titleMatch = findTitle.Match(divContent);
                    var title = titleMatch.Success ? titleMatch.Groups[1].Value : null;

                    var creatorMatch = findCreator.Match(divContent);
                    var creator = creatorMatch.Success ? creatorMatch.Groups[1].Value : null;

                    var imageUrlMatch = findImg.Match(divContent);
                    var imageUrl = imageUrlMatch.Success ? "http://huds.tf/img/main/" + imageUrlMatch.Groups[1].Value : null;

                    var hudstfMatch = findHudsTf.Match(divContent);
                    var hudstf = hudstfMatch.Success ? "http://huds.tf/" + hudstfMatch.Groups[1].Value : null;

                    var downloadMatch = findDownload.Match(divContent);
                    var download = downloadMatch.Success ? downloadMatch.Groups[1].Value : null;

                    var websiteMatch = findWebsite.Match(divContent);
                    var website = websiteMatch.Success ? websiteMatch.Groups[1].Value : null;

                    Application.Invoke(() =>
                        {
                            App.MainWindow.ScrollToTop();
                            var hudstfWidget = new HudsTFWidget(title, creator, imageUrl, hudstf, download, website);

                            WebClient webclient = new WebClient() { Proxy = null };

                            if (App.SetCustomImageViewImageDataDownloadedEvent != null)
                                App.SetCustomImageViewImageDataDownloadedEvent(webclient);
                            else
                            {
                                client.DownloadDataCompleted += (s, e) =>
                                {
                                    Task.Factory.StartNew(() =>
                                    {
                                        try
                                        {
                                            Image image = Image.FromStream(new MemoryStream(e.Result));
                                            ImageView imageView = (ImageView)e.UserState;
                                            imageView.Image = image;
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine(exc.Message);
                                        }
                                    });
                                };
                            }
                            webclient.DownloadDataAsync(new Uri(imageUrl, UriKind.Absolute), hudstfWidget.ImageView);
                            
                            Layout.AddChild(hudstfWidget);
                        });

                    //try
                    //{
                    //    new Bitmap(new MemoryStream(new WebClient().DownloadData(imageUrl))).Save("image.png");
                    //}
                    //catch { }
                }
            }
            catch
            {

            }
        }
    }
}
