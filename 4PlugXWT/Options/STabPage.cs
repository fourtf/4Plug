using FPlug.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Options
{
    [ScriptClass("Tab")]
    public class STabPage : SGroup
    {
        public event EventHandler TitleChanged;

        private string text = "Tab";

        [ScriptMember(ScriptTypeID.String)]
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;
                    if (TitleChanged != null)
                        TitleChanged(this, EventArgs.Empty);
                }
            }
        }
        
        public STabPage()
        {
            _GroupType = GroupType.None;
            paddingLeft = paddingRight = paddingTop = paddingBottom = 0;
        }
    }
}
