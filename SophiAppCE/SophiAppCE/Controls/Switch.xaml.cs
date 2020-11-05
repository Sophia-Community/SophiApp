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

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для Switch.xaml
    /// </summary>
    public partial class Switch : UserControl
    {
        public Switch()
        {
            InitializeComponent();
        }        

        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = GetTemplateChild("SwitchEllipse") as Ellipse;

            SolidColorBrush brushUnchecked = Application.Current.TryFindResource("Brush.SwitchBar.Ellipse.Unchecked") as SolidColorBrush;
            SolidColorBrush brushChecked = Application.Current.TryFindResource("Brush.SwitchBar.Ellipse.Checked") as SolidColorBrush;

            Thickness marginLeft = (Thickness)Resources["Margin.Switch.Ellipse.Left"];
            Thickness marginRight = (Thickness)Resources["Margin.Switch.Ellipse.Right"];

            Storyboard storyboard = Resources["Animation.Margin.Ellipse"] as Storyboard;
            ThicknessAnimation marginAnimation = storyboard.Children.First() as ThicknessAnimation;
            marginAnimation.To = ellipse.Margin == marginRight ? marginLeft : marginRight;
            storyboard.Begin(ellipse);

            ellipse.Fill = ellipse.Margin == marginRight ? brushUnchecked : brushChecked;
        }        
    }
}
