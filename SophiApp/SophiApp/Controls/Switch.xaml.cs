using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для Switch.xaml
    /// </summary>
    public partial class Switch : UserControl
    {
        // Using a DependencyProperty as the backing store for ActualState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualStateProperty =
            DependencyProperty.Register("ActualState", typeof(bool), typeof(Switch), new PropertyMetadata(default(bool)));

        // Using a DependencyProperty as the backing store for IsClicked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsClickedProperty =
            DependencyProperty.Register("IsClicked", typeof(bool), typeof(Switch), new PropertyMetadata(default(bool)));

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(Switch), new PropertyMetadata(default(bool)));

        public Switch()
        {
            InitializeComponent();
        }

        public bool ActualState
        {
            get { return (bool)GetValue(ActualStateProperty); }
            set { SetValue(ActualStateProperty, value); }
        }

        public bool IsClicked
        {
            get { return (bool)GetValue(IsClickedProperty); }
            set { SetValue(IsClickedProperty, value); }
        }

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => IsClicked = e.ButtonState == MouseButtonState.Pressed;
    }
}