using SophiApp.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для Hamburger.xaml
    /// </summary>
    public partial class Hamburger : UserControl
    {
        public Hamburger()
        {
            InitializeComponent();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationsManager.ShowDoubleAnimation(storyboardName: "Animation.Hamburger.Marker.Move", 
                                                  animatedElement: HamburgerMarker,
                                                  animationProperty: DoubleAnimation.ToProperty,
                                                  animationValue: ControlsManager.GetParentRelativePoint(childrenElement: e.OriginalSource as FrameworkElement, parentElement: ContentCanvas).Y,
                                                  dispatcher: Dispatcher);
        }
    }
}
