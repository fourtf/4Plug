using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xwt;

namespace FPlug.Widgets
{
    public class AddPluginWidget : PluginLayoutWidget
    {
        public AddPluginWidget(PluginType type, string title = null)
        {
            PluginType = type;

            Cursor = CursorType.Hand;

            Label l = new Label(title ?? "Add new " + type.GetName());
            //l.TextColor = type.GetColor();

            l.TextAlignment = Alignment.Center;

            AddChild(l, new Rectangle(0, 0, WidthRequest, HeightRequest));

            //BackgroundColor = type.GetBGColor();

            HasBorder = false;
            HasBorderLineDash = true;
        }

        protected override void OnButtonPressed(ButtonEventArgs args)
        {
            base.OnButtonPressed(args);


            if (PluginType == FPlug.PluginType.Hud)
            {
                new Task(() =>
                    {
                        System.Threading.Thread.Sleep(50);
                        Application.Invoke(() =>
                            {
                                var d = new HudsTFDisplay();
                                d.Show();
                                d.Present();
                            });
                    }).Start();
            }
        }
    }
}
