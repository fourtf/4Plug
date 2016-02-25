using Xwt;
using Xwt.Drawing;
using System.Linq;
using System;
using System.IO;
using System.Xml.Linq;

namespace FPlug.Widgets
{
    public abstract class PluginWidget : PluginLayoutWidget
    {
        static Image imgMoreActions = Resources.GetImage("expand.png");
        static Image imgOpenFolder = Resources.GetImage("folder.png");
        protected static Image imgSettings = Resources.GetImage("gear.png");
        protected static Image imgSettingsAlt = Resources.GetImage("gear_alt.png");
        protected static Image imgUpdate = Resources.GetImage("download.png");

        static Image imgChangeLog = Resources.GetImage("chglog.png");
        static Image imgReadme = Resources.GetImage("info.png");
        static Image imgHelp = Resources.GetImage("help.png");

        public static Image noImage = Resources.GetImage("noimage.png");

        static Menu menu;
        static PluginWidget lastClicked = null;


        //VARS
        public string Path
        {
            get;
            set;
        }
        public MainWindow MainWindow { get; set; }

        public new string Name { get; protected set; }
        public string AuthorUrl { get; set; }

        public abstract bool IsDirectory { get; }

        private bool installed;
        public bool Installed
        {
            get { return installed; }
            set
            {
                installed = value;
                pluginImage.Installed = value;
                QueueDraw();
            }
        }

        private FVersion localVersion = null;
        public FVersion LocalVersion
        {
            get
            {
                return localVersion;
            }
            set
            {
                localVersion = value;
                lblVersion.Text = value.ToString();
            }
        }

        protected string pluginDotXmlPath = null;

        private bool settingsActive;
        public bool SettingsActive
        {
            get
            {
                return settingsActive;
            }
            set
            {
                if (settingsActive != value)
                {
                    settingsActive = value;
                    settingsBtn.Image = !value ? imgSettings : imgSettingsAlt;
                }
            }
        }

        public override PluginType PluginType
        {
            get
            {
                return base.PluginType;
            }
            set
            {
                if (pluginImage != null)
                    pluginImage.PluginType = value;
                base.PluginType = value;
            }
        }
        

        //Widgets
        protected PluginImageButton pluginImage;
        protected HBox box;

        protected ImageButton openFolderBtn;
        protected ImageButton moreActionsBtn;

        protected Label lblName;
        protected TextEntry txtNameRename;
        protected Label lblVersion;
        protected Label lblAuthor;

        protected QuadImageLinkButton linksQuad;

        protected Button settingsBtn;

        protected string readmePath = null;
        protected ImageButton btnReadme;
        protected string faqPath = null;
        protected ImageButton btnFaq;
        protected string changelogPath = null;
        protected ImageButton btnChangelog;

        protected ImageButton buttonUpdate = null;

        protected static MenuItem createCopy;

        //CTOR
        static PluginWidget()
        {
            menu = new Menu();

            MenuItem item;

            (item = new MenuItem("Delete")).Clicked += (s, e) => { if (!lastClicked.Delete()) errorSound(); };
            item.Image = Resources.GetImage("delete.png");
            menu.Items.Add(item);

            (item = new MenuItem("Create Copy")).Clicked += (s, e) => { if (!lastClicked.CreateCopy()) errorSound(); };
            createCopy = item;
            item.Image = Resources.GetImage("copy.png");
            menu.Items.Add(item);

            (item = new MenuItem("Rename")).Clicked += (s, e) => { lastClicked.StartRename(); };
            item.Image = Resources.GetImage("rename.png");
            menu.Items.Add(item);

            menu.Items.Add(new SeparatorMenuItem());

            (item = new MenuItem("Copy Path")).Clicked += (s, e) => { Xwt.Clipboard.SetText(lastClicked.Path); };
            menu.Items.Add(item);
        }

        static void errorSound()
        {
            System.Media.SystemSounds.Exclamation.Play();
        }

        public PluginWidget(string path)
        {
            Path = path;
            Name = path.GetFileName();

            this.PluginType = GetPluginType();

            //---- image
            pluginImage = new PluginImageButton();
            pluginImage.Click += (s, e) =>
                {
                    if (!InstallOrUninstall())
                        errorSound();
                };
            pluginImage.PluginType = PluginType;
            AddChild(pluginImage, 33, 3);

            //---- buttons
            openFolderBtn = new ImageButton();
            openFolderBtn.Image = imgOpenFolder;
            openFolderBtn.Click += (s, e) => { Desktop.OpenFolder(Path); };

            moreActionsBtn = new ImageButton();
            moreActionsBtn.Image = imgMoreActions;
            moreActionsBtn.ButtonReleased += (s, e) =>
                {
                    if (e.Button == PointerButton.Left)
                    {
                        lastClicked = this;
                        menu.Popup(this, e.X, e.Y);
                    }
                };

            AddChild(openFolderBtn, 6, 6);
            AddChild(moreActionsBtn, 6, 25);

            AddChild(new VerticalLine(), new Rectangle(28, 6, 1, 34));

            //---- lbl
            lblName = new Label(Name + new string(' ', 20));
            lblAuthor = new Label("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"); // I only choose the very best of hacks
            lblVersion = new Label("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

            AddChild(lblName, new Rectangle(117, 1, 132, 18));
            AddChild(lblAuthor, new Rectangle(117, 14, 132, 18));
            AddChild(lblVersion, new Rectangle(117, 27, 132, 18));
            
            lblName.ButtonPressed += (s, e) =>
            {
                if (e.MultiplePress == 2)
                {
                    StartRename();
                    e.Handled = true;
                }
            };

            lblAuthor.Opacity = lblVersion.Opacity = .5;
            lblAuthor.Text = lblVersion.Text = "";

            //---- rename
            txtNameRename = new TextEntry();
            txtNameRename.KeyPressed += (s, e) =>
            {
                if (e.Key == Key.Return)
                {
                    if (Rename(txtNameRename.Text))
                    {
                        EndRename();
                        // disabled if there's no move animation
                        if (App.MoveWidget != null)
                            App.MainWindow.Layout.Sort(true);
                    }
                    else
                        errorSound();
                }
                else if (e.Key == Key.Escape)
                {
                    EndRename();
                }
            };

            AddChild(txtNameRename, new Rectangle(117, 2, 132, 22));
            txtNameRename.Visible = false;

            AddChild(new VerticalLine(), new Rectangle(250, 6, 1, 34));

            //---- little icons
            AddChild(btnChangelog = new ImageButton(12, 12, imgChangeLog) { Visible = false }, 208, 32);
            AddChild(btnReadme = new ImageButton(12, 12, imgReadme) { Visible = false }, 222, 32);
            AddChild(btnFaq = new ImageButton(12, 12, imgHelp) { Visible = false }, 236, 32);

            btnChangelog.Click += (s, e) => { if (changelogPath != null) new TextViewerWindow(changelogPath).Show(); };
            btnReadme.Click += (s, e) => { if (readmePath != null) new TextViewerWindow(readmePath).Show(); };
            btnFaq.Click += (s, e) => { if (faqPath != null) new TextViewerWindow(faqPath).Show(); };

            //---- update btn
            buttonUpdate = new ImageButton(16, 16, imgUpdate);
            buttonUpdate.Visible = false;
            AddChild(buttonUpdate, 232, 8);

            //---- settings button
            settingsBtn = new Button();
            //settingsBtn.Image = imgSettings;
            settingsBtn.ImagePosition = ContentPosition.Center;
            settingsBtn.WidthRequest = 25;
            settingsBtn.HeightRequest = 22;
            settingsBtn.Sensitive = false;
            settingsBtn.Clicked += (s, e) => { ShowSettings(); };
            AddChild(settingsBtn, 256, 11);

            AddChild(new VerticalLine(), new Rectangle(285, 6, 1, 34));

            //---- links
            linksQuad = new QuadImageLinkButton();
            AddChild(linksQuad, 284, 0);
            //AddChild(linksQuad, 252, 0);

            Load();
        }

        protected virtual PluginType GetPluginType()
        {
            return FPlug.PluginType.Unknown;
        }


        //ABSTRACT SHIT
        public abstract void Load();

        public abstract void ShowSettings();

        public abstract bool InstallOrUninstall();

        protected abstract bool OnDelete();

        public abstract bool CreateCopy();

        public abstract bool Rename(string name);


        //NOT SO ABSTRACT SHIT
        public void ProcessDataTxt(string text)
        {
            foreach (string s in text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    string[] S = s.Split(new char[] { '=' }, 2);
                    switch (S[0])
                    {
                        case "creator":
                            lblAuthor.Text = S[1];
                            break;
                        case "version":
                            LocalVersion = FVersion.TryParse(S[1]) ?? LocalVersion;
                            break;
                        case "steamid":
                            AuthorUrl = "http://steamcommunity.com/profiles/" + S[1];
                            break;
                        case "url":
                            linksQuad.AddLink(S[1]);
                            break;
                        //case "desc":
                        //    Description = S[1];
                        //    break;
                        //case "name":
                        //    Alias = S[1];
                        //    break;
                        //case "external_editor":
                        //    if (File.Exists(LocalPath + S[1]))
                        //        ExternalEditor = S[1];
                        //    break;
                        //case "version_url":
                        //    if (SupportsUpdates)
                        //        VersionUrl = S[1];
                        //    break;
                        //case "update_url":
                        //    if (SupportsUpdates)
                        //        UpdateUrl = S[1];
                        //    break;
                        //case "update_name":
                        //    if (SupportsUpdates)
                        //        UpdateNamespace = S[1];
                        //    Use4Update = true;
                        //    break;
                    }
                }
                catch { }
            }
        }

        public void LoadXml(string path, bool allowSettings = true)
        {
            try
            {
                XDocument doc = XDocument.Load(path);
                var plugin = doc.Element("mod");
                if (plugin != null)
                {
                    var meta = plugin.Element("info");
                    if (meta != null)
                    {
                        XElement x;
                        if ((x = meta.Element("author")) != null)
                            lblAuthor.Text = (string)x;
                        if ((x = meta.Element("version")) != null)
                            lblVersion.Text = (string)x;
                        if ((x = meta.Element("links")) != null)
                        {
                            foreach (var e in x.Elements("link"))
                                linksQuad.AddLink((string)e);
                        }
                    }

                    if (allowSettings)
                    {
                        if (plugin.Element("options") != null)
                        {
                            settingsBtn.Sensitive = true;
                            settingsBtn.Image = imgSettings;
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public virtual void StartRename()
        {
            txtNameRename.Text = lblName.Text.Trim();
            //txtNameRename.Sensitive = true;
            txtNameRename.Visible = true;
            txtNameRename.SelectionStart = lblName.Text.Length;
            txtNameRename.SetFocus();
        }

        public void EndRename()
        {
            //txtNameRename.Sensitive = false;
            txtNameRename.Visible = false;
        }

        public bool Delete()
        {
            bool b = OnDelete();
            if (b)
                MainWindow.Layout.Layout(true);
            return b;
        }

        protected override void OnMouseEntered(EventArgs args)
        {
            //pluginImage.ShowInstall = true;
            base.OnMouseEntered(args);
        }

        protected override void OnMouseExited(EventArgs args)
        {
            //pluginImage.ShowInstall = false;
            base.OnMouseExited(args);
        }


        //DRAWING
        static Color BgColorInstalled = new Color(.95d, 1d, .95);
        static Color BorderColorInstalled = new Color(.6d, .8d, .6d);

        static Color BgColorUninstalled = new Color(1d, .95d, .95d);
        static Color BorderColorUninstalled = new Color(.8d, .6d, .6d);

        protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
        {
            //bg
            //ctx.SetColor(installed ? BgColorInstalled : BgColorUninstalled);
            //ctx.SetColor(Disabled ? Colors.Gray.WithAlpha(.15) : (installed ? PluginType.GetBGColor() : Colors.White));
            ctx.SetColor(installed ? PluginType.GetBGColor() : Colors.White);
            //ctx.SetColor(Colors.White);
            ctx.Rectangle(dirtyRect);
            ctx.Fill();

            //border
            //ctx.SetColor(installed ? BorderColorInstalled : BorderColorUninstalled);
            //ctx.SetColor(installed ? PluginType.GetColor() : Colors.White); // Color.FromBytes(220, 220, 220));
            ctx.SetColor(SettingsActive ? Colors.Gray : PluginType.GetColor());
            ctx.SetLineWidth(1);

            if (!installed)
                ctx.SetLineDash(0, 1, 3);
            ctx.Rectangle(0, 0, WidthRequest, HeightRequest);
            ctx.Stroke();

            //if (Disabled)
            //{
            //    ctx.SetColor(new Color(1, .5, 0, .1));
            //    ctx.Rectangle(dirtyRect);
            //    ctx.Fill();
            //}
        }
    }
}
