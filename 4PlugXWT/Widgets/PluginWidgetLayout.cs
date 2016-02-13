using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Widgets
{
    public class PluginWidgetLayout : WidgetLayout<PluginLayoutWidget>
    {
        public PluginWidgetLayout()
        {
            EnableCustomItemMove = true;

            ControlComparer = (a, b) =>
                {
                    if (a.PluginType > b.PluginType)
                        return false;
                    else if (a.PluginType == b.PluginType)
                    {
                        if (a is PluginTitleWidget)
                            return true;
                        if (b is PluginTitleWidget)
                            return false;

                        if (a is AddPluginWidget)
                            return false;
                        if (b is AddPluginWidget)
                            return true;

                        //return String.Compare(((PluginWidget)a).Name, ((PluginWidget)b).Name) != 1;
                        return new AlphanumComparatorFast().Compare(((PluginWidget)a).Name, ((PluginWidget)b).Name) != 1;
                    }
                    return true;
                };

            ItemMargin = PluginLayoutWidget.DefaultMargin;
            ItemWidth = PluginLayoutWidget.DefaultSize.Width;
        }

        public void ClearPlugins()
        {
            children.ToList().Where(w => w.Widget is PluginWidget && ((PluginWidget)w.Widget).PluginType != PluginType.None).Do(w => { RemoveChild(w.Widget); children.Remove(w); });
            //if (App.InitOpacityAnimation != null)
            //    children.ForEach(w =>
            //    {
            //        w.Opacity = 0;
            //        var backend = w as IHasCanvasBackend;
            //        if (backend != null)
            //            App.InitOpacityAnimation(backend);
            //    });
            Layout();
        }
    }

    public interface IHasPluginType
    {
        PluginType PluginType { get; set; }
    }
}
