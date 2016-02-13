using Xwt;
using Xwt.Drawing;
using System.Linq;
using System;
using System.IO;
using System.Xml.Linq;

namespace FPlug
{
    public abstract class PluginWidget : Canvas, IHasPluginType, IHasCanvasBackend
    {
        static Image imgMoreActions = Resources.GetImage("expand.png");
        static Image imgOpenFolder = Resources.GetImage("folder.png");
        static Image imgSettings = Resources.GetImage("gear.png");

        static Image imgChangeLog = Resources.GetImage("chglog.png");
        static Image imgReadme = Resources.GetImage("info.png");
        static Image imgHelp = Resources.GetImage("help.png");

        protected static Image noImage = Resources.GetImage("noimage.png");

        static Menu menu;
        static PluginWidget lastClicked = null;

        public static new readonly Size Size = new Size(330, 46);
        public static new readonly WidgetSpacing Margin = new WidgetSpacing(0, 12, 12);

        //VARS
        public string Path
        {
            get;
            set;
        }
        public MainWindow MainWindow { get; set; }

        public new String Name { get; protected set; }
        public String AuthorUrl { get; set; }

        public abstract bool IsDirectory { get; }

        private bool installed;
        public bool Installed
        {
            get { return installed; }
            set { installed = value; QueueDraw(); }
        }

        private VersionVar localVersion = null;
        public VersionVar LocalVersion
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


        //CTOR
        static PluginWidget()
        {
            menu = new Menu();
            
            MenuItem item;

            (item = new MenuItem("Delete")).Clicked += (s, e) => { if (!lastClicked.Delete()) errorSound(); };
            item.Image = Resources.GetImage("delete.png");
            menu.Items.Add(item);
            
            (item = new MenuItem("Create Copy")).Clicked += (s, e) => { if (!lastClicked.CreateCopy()) errorSound(); };
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

            base.Margin = Margin;
            this.PluginType = GetPluginType();

            //---- image
            pluginImage = new PluginImageButton();
            pluginImage.Click += (s, e) =>
                {
                    if (!InstallOrUninstall())
                        errorSound();
                };
            //pluginImage.MouseEntered += (s, e) => openFolderBtn.Visible = moreActionsBtn.Visible = true;
            //pluginImage.MouseExited += (s, e) => openFolderBtn.Visible = moreActionsBtn.Visible = false;
            AddChild(pluginImage, 33, 3);

            //---- buttons
            openFolderBtn = new ImageButton();
            //openFolderBtn.Visible = false;
            openFolderBtn.Image = imgOpenFolder;
            openFolderBtn.Click += (s, e) => { Desktop.OpenFolder(Path); };

            moreActionsBtn = new ImageButton();
            //moreActionsBtn.Visible = false;
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
            lblName = new Label(Name);
            lblAuthor = new Label("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"); // I only choose the very best of hacks
            lblVersion = new Label("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

            AddChild(lblName, new Rectangle(117, 2, 132, 19));
            AddChild(lblAuthor, new Rectangle(117, 15, 132, 19));
            AddChild(lblVersion, new Rectangle(117, 28, 132, 19));

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

            //---- txt
            txtNameRename = new TextEntry();
            txtNameRename.Visible = false;
            txtNameRename.KeyPressed += (s, e) =>
            {
                if (e.Key == Key.Return)
                {
                    if (Rename(txtNameRename.Text))
                        EndRename();
                    else
                        errorSound();
                }
                else if (e.Key == Key.Escape)
                {
                    EndRename();
                }
            };

            AddChild(txtNameRename, new Rectangle(117, 2, 132d, 22));

            //AddChild(new VerticalLine(), new Rectangle(250, 6, 1, 34));

            //---- settings button
            settingsBtn = new Button();
            settingsBtn.Image = imgSettings;
            settingsBtn.ImagePosition = ContentPosition.Center;
            settingsBtn.WidthRequest = 25;
            settingsBtn.HeightRequest = 22;
            settingsBtn.Sensitive = false;
            settingsBtn.Clicked += (s, e) => { ShowSettings(); };
            AddChild(settingsBtn, 256, 11);

            //AddChild(new VerticalLine(), new Rectangle(250, 6, 1, 34));

            //----links
            linksQuad = new QuadImageLinkButton();
            AddChild(linksQuad, 284, 0);
            //AddChild(linksQuad, 252, 0);

            //----size
            HeightRequest = Size.Height;
            WidthRequest = Size.Width;

            if (App.InitDropShadow != null)
                App.InitDropShadow(this);
            if (App.InitOpacityAnimation != null)
            {
                Opacity = 0;
                App.InitOpacityAnimation(this);
            }
            Load();
        }

        public Xwt.Backends.ICanvasBackend GetBackend()
        {
            return (Xwt.Backends.ICanvasBackend)BackendHost.Backend;
        }

        protected virtual PluginType GetPluginType()
        {
            return FPlug.PluginType.Unknown;
        }

        //ABSTRACT SHIT
        public abstract void Load();

        public abstract void ShowSettings();

        public abstract bool InstallOrUninstall();

        public abstract bool Delete();

        public abstract bool CreateCopy();

        public abstract bool Rename(string name);

        //NOT SO ABSTRACT SHIT
        public void ProcessDataTxt(string text)
        {
            foreach (string s in text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                try
                {
                    String[] S = s.Split(new char[] { '=' }, 2);
                    switch (S[0])
                    {
                        case "creator":
                            lblAuthor.Text = S[1];
                            break;
                        case "version":
                            VersionVar v = new VersionVar();
                            if (v.TryParse(S[1]))
                                LocalVersion = v;
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

        public void LoadXml(string path)
        {
            try
            {
                XDocument doc = XDocument.Load(path);
                var meta = doc.Element("Plugin");
                if (meta != null)
                {
                    meta = meta.Element("Meta");
                    if (meta != null)
                    {
                        XElement x;
                        if ((x = meta.Element("Author")) != null)
                            lblAuthor.Text = (string)x;
                        if ((x = meta.Element("Version")) != null)
                            lblVersion.Text = (string)x;
                        if ((x = meta.Element("Links")) != null)
                        {
                            foreach (var e in x.Elements("Link"))
                                linksQuad.AddLink((string)e);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public void StartRename()
        {
            txtNameRename.Text = lblName.Text;
            txtNameRename.Sensitive = true;
            txtNameRename.Visible = true;
            txtNameRename.SelectionStart = lblName.Text.Length;
            txtNameRename.SetFocus();
        }

        public void EndRename()
        {
            txtNameRename.Sensitive = false;
            txtNameRename.Visible = false;
        }

        //DRAWING
        static Color BgColorInstalled = new Color(.95d, 1d, .95);
        static Color BorderColorInstalled = new Color(.6d, .8d, .6d);

        static Color BgColorUninstalled = new Color(1d, .95d, .95d);
        static Color BorderColorUninstalled = new Color(.8d, .6d, .6d);

        protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
        {
            //bg
            ctx.SetColor(installed ? BgColorInstalled : BgColorUninstalled);
            //ctx.SetColor(Colors.White);
            ctx.Rectangle(dirtyRect);
            ctx.Fill();

            //border
            ctx.SetColor(installed ? BorderColorInstalled : BorderColorUninstalled);
            ctx.SetLineWidth(1);
            ctx.Rectangle(0, 0, WidthRequest, HeightRequest);
            ctx.Stroke();
        }

        public PluginType PluginType
        {
            get;
            set;
            //get
            //{
            //    throw new NotImplementedException();
            //}
            //set
            //{
            //    throw new NotImplementedException();
            //}
        }
    }
}
