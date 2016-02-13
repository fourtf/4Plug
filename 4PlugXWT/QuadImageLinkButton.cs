using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug
{
    public class QuadImageLinkButton : Canvas
    {
        Point[] PossibleLocations = new[] { new Point(6, 6), new Point(6, 24), new Point(24, 6), new Point(24, 24) };

        String[] StartsWith = new[] { "teamfortress.tv/", "etf2l.org/", "steamcommunity.com/groups/", "github.com/" };
        Image[] CorrespondingImage = new[] { Resources.GetImage("tftv.png"), Resources.GetImage("etf2l.png"), Resources.GetImage("steam.png"), Resources.GetImage("github.png") };

        Image DefaultImage = Resources.GetImage("home.png");

        int currentLink = 0;

        public QuadImageLinkButton()
        {
            WidthRequest = 40;
            HeightRequest = 40;
        }

        public void AddLink(string link)
        {
            if (currentLink < 4)
            {
                string linkSub = null;
                if (link.StartsWith("http://"))
                {
                    linkSub = link.Substring(7);
                }
                else if (link.StartsWith("https://"))
                {
                    linkSub = link.Substring(8);
                }

                if (linkSub != null)
                {
                    if (linkSub.StartsWith("www."))
                        linkSub = linkSub.Substring(4);
                    linkSub = linkSub.ToLower();
                    
                    ImageLinkButton b = new ImageLinkButton();

                    for (int i = 0; i < StartsWith.Length; i++)
                    {
                        if (linkSub.StartsWith(StartsWith[i]))
                        {
                            b.Image = CorrespondingImage[i];
                            break;
                        }
                    }

                    b.Url = link;

                    if (b.Image == null)
                        b.Image = DefaultImage;

                    AddChild(b, PossibleLocations[currentLink].X, PossibleLocations[currentLink].Y);

                    currentLink++;
                }
            }
        }

        //protected override void OnDraw(Xwt.Drawing.Context ctx, Rectangle dirtyRect)
        //{
        //    base.OnDraw(ctx, dirtyRect);
        //
        //    ctx.SetColor(Colors.Red);
        //    ctx.Rectangle(dirtyRect);
        //    ctx.Stroke();
        //}
    }
}
