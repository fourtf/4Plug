﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Options.Controls
{
    public class DropDownControl : Control
    {
        [ScriptMember("text")]
        public string Text { get; set; }
    }
}