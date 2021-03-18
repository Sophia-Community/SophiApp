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

namespace SophiApp.Controls
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

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(Switch), new PropertyMetadata(default(bool)));

        public bool ActualState
        {
            get { return (bool)GetValue(ActualStateProperty); }
            set { SetValue(ActualStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualStateProperty =
            DependencyProperty.Register("ActualState", typeof(bool), typeof(Switch), new PropertyMetadata(default(bool)));


        public bool IsClicked
        {
            get { return (bool)GetValue(IsClickedProperty); }
            set { SetValue(IsClickedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsClicked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsClickedProperty =
            DependencyProperty.Register("IsClicked", typeof(bool), typeof(Switch), new PropertyMetadata(default(bool)));

        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => IsClicked = e.ButtonState == MouseButtonState.Pressed;

    }
}
