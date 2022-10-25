using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для SettingSwitch.xaml
    /// </summary>
    public partial class SettingSwitch : UserControl
    {
        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SettingSwitch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SettingSwitch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(SettingSwitch), new PropertyMetadata(default));

        public SettingSwitch()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value);
        }

        public string Header
        {
            get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value);
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value);
        }

        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Command?.Execute(DataContext);
    }
}