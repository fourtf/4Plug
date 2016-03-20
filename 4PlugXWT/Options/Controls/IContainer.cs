using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Options.Controls
{
    public interface IContainer : IControl
    {
        IList<Control> Controls { get; }

        void AddControl(Control c);
    }
}
