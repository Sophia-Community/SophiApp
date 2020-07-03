using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Managers
{
    internal class ControlsManager
    {
        internal static Point GetParentRelativePoint(FrameworkElement childrenElement, FrameworkElement parentElement)
        {
            return childrenElement.TranslatePoint(new Point(0, 0), parentElement);
        }
    }
}
