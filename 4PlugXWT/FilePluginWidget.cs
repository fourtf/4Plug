using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug
{
    public class FilePluginWidget : PluginWidget
    {
        public FilePluginWidget(string path)
            : base (path)
        {

        }

        public override void Load()
        {
            
        }

        protected override PluginType GetPluginType()
        {
            return (Path.ToUpper().EndsWith(".VPK")) ? FPlug.PluginType.Vpk : FPlug.PluginType.Unknown;
        }

        public override bool IsDirectory
        {
            get
            {
                return false;
            }
        }

        public override void ShowSettings()
        {
            
        }

        public override bool InstallOrUninstall()
        {
            return false;
        }

        public override bool CreateCopy()
        {
            return false;
        }

        public override bool Delete()
        {
            return false;
        }

        public override bool Rename(string name)
        {
            return false;
        }
    }
}
