using System.Windows;
using System.Windows.Controls;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для SwitchBar.xaml
    /// </summary>
    public partial class SwitchBar : UserControl
    {
        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SwitchBar), new PropertyMetadata(default(string)));

        public SwitchBar()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}