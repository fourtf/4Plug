using System;

namespace FPlug.Scripting
{
    [Flags]
    public enum ScriptTypeID
    {
        None = 0,
        Void,
        Object,
        IfState,
        Unchanged,
        Boolean,
        String,
        Number,
        File,
        Directory,
        SChild,
    }
}
