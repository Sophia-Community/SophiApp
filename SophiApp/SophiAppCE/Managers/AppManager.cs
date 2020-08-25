using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Managers
{
    internal static class AppManager
    {
        internal static Point GetParentElementRelativePoint(FrameworkElement childrenElement) => childrenElement.TranslatePoint(new Point(0, 0), childrenElement.Parent as UIElement);
    }
}
