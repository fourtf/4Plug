using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xwt;
using Xwt.Drawing;

namespace FPlug.Options
{
    [Scripting.ScriptClass("Image")]
    public class SImage : SSingleWidget<ImageView>
    {
        private string path;

        [Scripting.ScriptMember(Scripting.ScriptTypeID.String)]
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                if (path != value)
                {
                    control.Image = null;
                    path = value;

                    string fullpath = Window.FolderCache.TryResolvePath(value, false);

                    if (fullpath != null)
                    {
                        if (App.SetImageAsync != null)
                            App.SetImageAsync(fullpath, control);
                        else
                        {
                            try
                            {
                                control.Image = Image.FromFile(fullpath);
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        public SImage()
        {
            HeightRequest = 100;
            Height = 100;
        }
    }
}
