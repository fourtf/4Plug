using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace FPlug.Options.Controls
{
    public class DropDownControl : SingleWidgetControl<ComboBox>
    {
        [ScriptMember("text")]
        public string Text { get; set; }
    }
}
