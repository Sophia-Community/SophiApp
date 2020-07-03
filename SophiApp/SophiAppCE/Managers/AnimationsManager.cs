using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SophiAppCE.Managers
{
    internal class AnimationsManager
    {
        internal static void ShowDoubleAnimation(string storyboardName, FrameworkElement animatedElement, double animationValue)
        {
            Storyboard storyboard = Application.Current.TryFindResource(storyboardName) as Storyboard;
            DoubleAnimation animation = storyboard.Children.First() as DoubleAnimation;
            animation.To = animationValue;
            storyboard.Begin(animatedElement);
        }

        internal static void ShowThicknessAnimation(string storyboardName, FrameworkElement animatedElement, Thickness animationValue)
        {
            Storyboard storyboard = Application.Current.TryFindResource(storyboardName) as Storyboard;
            ThicknessAnimation animation = storyboard.Children.First() as ThicknessAnimation;
            animation.To = animationValue;
            storyboard.Begin(animatedElement);
        }

        internal static void ShowColorAnimation(string storyboardName, FrameworkElement animatedElement, Color animationValue)
        {
            Storyboard storyboard = Application.Current.TryFindResource(storyboardName) as Storyboard;
            ColorAnimation animation = storyboard.Children.First() as ColorAnimation;
            animation.To = animationValue;
            storyboard.Begin(animatedElement);
        }
    }
}
