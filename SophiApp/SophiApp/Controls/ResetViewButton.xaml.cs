using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для ResetViewButton.xaml
    /// </summary>
    public partial class ResetViewButton : UserControl
    {
        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ResetViewButton), new PropertyMetadata(default));

        public ResetViewButton()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private void ResetViewButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Command?.Execute(null);
        }
    }
}