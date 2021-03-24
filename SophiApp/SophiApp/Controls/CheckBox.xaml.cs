using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для CheckBox.xaml
    /// </summary>
    public partial class CheckBox : UserControl
    {
        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckBox), new PropertyMetadata(default(bool)));

        public CheckBox()
        {
            InitializeComponent();
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        private void CheckBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => IsChecked = !IsChecked;
    }
}