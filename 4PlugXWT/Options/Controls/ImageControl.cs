using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Options.Controls
{
    public class ImageControl : SingleWidgetControl<ImageView>
    {
        bool imageSet = false;

        private string src;

        [ScriptMember("src")]
        public string Src
        {
            get { return src; }
            set
            {
                src = value;
                imageSet = false;
                setImage();
            }
        }

        void setImage()
        {
            try
            {
                if (Window != null)
                {
                    var file = Window.Folder.GetFile(src);
                    if (file != null)
                        control.Image = Image.FromFile(file.Path);
                    Height = control.Image.Height;
                }
            }
            catch
            {

            }
        }

        protected override void OnWindowSet()
        {
            base.OnWindowSet();

            if (!imageSet)
                setImage();
        }

        // CTOR
        public ImageControl()
        {
            
        }
    }
}
