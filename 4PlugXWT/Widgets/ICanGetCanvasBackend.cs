using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Widgets
{
    public interface IHasCanvasBackend
    {
        Xwt.Backends.ICanvasBackend GetBackend();
    }
}
