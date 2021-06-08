using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SophiApp.Helpers
{
    internal class MouseHelper
    {
        internal static void ShowWaitCursor(bool show) => Mouse.OverrideCursor = show ? Cursors.Wait : null;
    }
}
