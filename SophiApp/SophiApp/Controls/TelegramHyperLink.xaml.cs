using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для TelegramHyperLink.xaml
    /// </summary>
    public partial class TelegramHyperLink : UserControl
    {
        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(TelegramHyperLink), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TelegramHyperLink), new PropertyMetadata(default));

        public TelegramHyperLink()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        private void TelegramHyperLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Command?.Execute(CommandParameter);
    }
}