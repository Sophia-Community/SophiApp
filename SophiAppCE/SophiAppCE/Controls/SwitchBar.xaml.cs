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
    /// Логика взаимодействия для SwitchBar.xaml
    /// </summary>
    public partial class SwitchBar : UserControl
    {
        public SwitchBar()
        {
            InitializeComponent();
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SwitchBar), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SwitchBar), new PropertyMetadata(default(string)));

        public bool SystemState
        {
            get { return (bool)GetValue(SystemStateProperty); }
            set { SetValue(SystemStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SystemState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SystemStateProperty =
            DependencyProperty.Register("SystemState", typeof(bool), typeof(SwitchBar), new PropertyMetadata(false));

        public bool ActualState
        {
            get { return (bool)GetValue(ActualStateProperty); }
            set { SetValue(ActualStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualStateProperty =
            DependencyProperty.Register("ActualState", typeof(bool), typeof(SwitchBar), new PropertyMetadata(false));



        public bool SwitchState
        {
            get { return (bool)GetValue(SwitchStateProperty); }
            set { SetValue(SwitchStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SwitchState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchStateProperty =
            DependencyProperty.Register("SwitchState", typeof(bool), typeof(SwitchBar), new PropertyMetadata(false));



        private void SwitchContainerPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {   
            ChangeState();
        }

        internal void ChangeState()
        {
            ActualState = !ActualState;
            Ellipse ellipse = GetTemplateChild("SwitchEllipse") as Ellipse;

            SolidColorBrush brushUnchecked = Application.Current.TryFindResource("Brush.SwitchBar.Ellipse.Unchecked") as SolidColorBrush;
            SolidColorBrush brushChecked = Application.Current.TryFindResource("Brush.SwitchBar.Ellipse.Checked") as SolidColorBrush;

            Thickness marginLeft = (Thickness) Resources["Margin.Switch.Ellipse.Left"];
            Thickness marginRight = (Thickness)Resources["Margin.Switch.Ellipse.Right"];
            
            Storyboard storyboard = Resources["Animation.Margin.Ellipse"] as Storyboard;
            ThicknessAnimation marginAnimation = storyboard.Children.First() as ThicknessAnimation;
            marginAnimation.To = ellipse.Margin == marginRight ? marginLeft : marginRight;
            storyboard.Begin(ellipse);

            ellipse.Fill = ellipse.Margin == marginRight ? brushUnchecked : brushChecked;
        }
    }
}
