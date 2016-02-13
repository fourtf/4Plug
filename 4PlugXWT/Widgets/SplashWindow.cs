using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Widgets
{
    public class SplashWindow : Dialog
    {
        public SplashWindow()
        {
            Icon = App.Icon;

            Width = 550;

            Title = "4Plug First Use";
            Resizable = false;

            VBox V = new VBox();
            Content = V;

            Label lbl;

            lbl = new Label("A small introduction.") { Font = Font.FromName("Segoe UI Light 24"), TextColor = PluginType.Vpk.GetColor() };
            V.PackStart(lbl);

            lbl = new Label("This tool allows you to quickly enable/disable mods as well as install new ones.");
            V.PackStart(lbl);

            lbl = new Label(""); V.PackStart(lbl);

            lbl = new Label("This is what a mod looks like in 4Plug!") { Font = Font.FromName("Segoe UI Light 24"), TextColor = PluginType.Unknown.GetColor() };
            V.PackStart(lbl);

            lbl = new Label("You can enable/disable mods by clicking on the image.");
            V.PackStart(lbl);

            //lbl = new Label("Uninstalled mods are saved in the \"custom_\" instead of the \"custom\" folder of you game.");
            //V.PackStart(lbl);

            lbl = new Label("");

            DummyPluginWidget dummy = new DummyPluginWidget(lbl);
            dummy.MarginTop += 16;
            dummy.MarginBottom += 8;
            V.PackStart(dummy);

            V.PackStart(lbl);

            lbl = new Label(""); V.PackStart(lbl);

            {
                HBox box = new HBox();
                Button btn;

                btn = new Button(" Got it! ");
                box.PackEnd(btn);
                btn.Clicked += (s, e) => { Close(); };

                Label lbl2;
                lbl2 = new Label(" Feel free to leave feedback (mainmenu -> submit feedback) later ");
                box.PackStart(lbl2);
                //btn.Clicked += (s, e) => { new SubmitFeedbackWindow("via Splash Window").Run(); };

                V.PackStart(box);
            }
        }
    }
}
