using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xwt.Drawing;

namespace FPlug
{
    public static class Resources
    {
        public static Image GetImage(string resource)
        {
            try
            {
                Image img;
                using (Stream s = GetStream(resource))
                {
                    img = Image.FromStream(s);
                }
                return img;
            }
            catch
            {
                return null;
            }
        }

        static Assembly ca = Assembly.GetExecutingAssembly();
        static string path = "FPlug.res.";

        public static Stream GetStream(string resource)
        {
            return ca.GetManifestResourceStream(path + resource);
        }
    }
}
