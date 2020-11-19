using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SophiAppCE.Helpers
{
    internal static class Animator
    {
        internal static void ShowThicknessAnimation(string storyboardName, FrameworkElement element,Thickness from, Thickness to, EventHandler isComplited)
        {
            Storyboard storyboard = Application.Current.TryFindResource(storyboardName) as Storyboard;
            ThicknessAnimation animation = storyboard.Children.First() as ThicknessAnimation;
            animation.From = from;
            animation.To = to;            
            storyboard.Completed += isComplited;
            storyboard.Begin(element);            
        }        
    }
}
