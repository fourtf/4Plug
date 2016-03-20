using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Options.Controls
{
    public interface IControl
    {
        void PerformLayout(double width);

        SettingsWindow Window { get; set; }

        double Height { get; set; }
        double Width { get; set; }
    }
}
