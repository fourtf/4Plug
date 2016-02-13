using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FPlug.Editor
{
    public enum TextAction
    {
        // Delete
        DeleteLeft,
        DeleteLeftControl,
        DeleteRight,
        DeleteRightControl,

        // Caret
        MoveCaretLeft,
        MoveCaretLeftControl,
        MoveCaretRight,
        MoveCaretRightControl,
        MoveCaretUp,
        MoveCaretDown,

        // Selection
        SelectionExtendLeft,
        SelectionExtendLeftControl,
        SelectionExtendRight,
        SelectionExtendRightControl,
        SelectionExtendDown,
        SelectionExtendUp,
    }
}
