using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для RadioButtonGroup.xaml
    /// </summary>
    public partial class RadioButtonGroup : UserControl
    {
        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(RadioButtonGroup), new PropertyMetadata(default));

        public RadioButtonGroup()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Command?.Execute(e.OriginalSource);
        }
    }
}