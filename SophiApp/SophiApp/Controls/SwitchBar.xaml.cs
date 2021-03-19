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
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SwitchBar), new PropertyMetadata(default(string)));

        public SwitchBar()
        {
            InitializeComponent();
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SwitchBar), new PropertyMetadata(default(string)));


    }
}