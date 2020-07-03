using SophiAppCE.Managers;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для Switch.xaml
    /// </summary>
    public partial class Switch : UserControl
    {
        internal bool IsChecked { get; private set; } = false;
        private SolidColorBrush checkedBrush = new SolidColorBrush((Color)Application.Current.TryFindResource("Color.Switch.Ellipse.Checked"));
        private SolidColorBrush uncheckedBrush = new SolidColorBrush((Color)Application.Current.TryFindResource("Color.Switch.Ellipse.Unchecked"));

        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationsManager.ShowThicknessAnimation(storyboardName: "Animation.Switch.Click",
                                                     animatedElement: SwitchEllipse,
                                                     animationValue: IsChecked == false ? (Thickness)TryFindResource("Control.Switch.Ellipse.State.Right") : (Thickness)TryFindResource("Control.Switch.Ellipse.State.Left"));

            SwitchEllipse.Fill = IsChecked == false ? checkedBrush : uncheckedBrush;
            IsChecked = IsChecked == true ? false : true;
        }

        public Switch()
        {
            InitializeComponent();
        }
    }
}
