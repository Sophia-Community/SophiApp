using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SophiApp.Managers
{
    internal class AnimationsManager
    {
        internal static void ShowDoubleAnimation(string storyboardName, FrameworkElement animatedElement, DependencyProperty animationProperty, double animationValue, Dispatcher dispatcher)
        {
            Task.Run(() => dispatcher.Invoke(() =>
            {
                Storyboard storyboard = Application.Current.TryFindResource(storyboardName) as Storyboard;
                DoubleAnimation animation = storyboard.Children.First() as DoubleAnimation;
                animation.SetValue(animationProperty, animationValue);
                storyboard.Begin(animatedElement);
            }));

        }

        
    }
}
