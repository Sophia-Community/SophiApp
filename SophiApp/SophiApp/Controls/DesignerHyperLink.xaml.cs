using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для DesignerHyperLink.xaml
    /// </summary>
    public partial class DesignerHyperLink : UserControl
    {
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(DesignerHyperLink), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DesignerHyperLink), new PropertyMetadata(default));

        public DesignerHyperLink()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        private void DesignerHyperLink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Command?.Execute(CommandParameter);
    }
}