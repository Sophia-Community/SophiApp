using SophiApp.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace SophiApp.Views
{
    /// <summary>
    /// Логика взаимодействия для PrivacyView.xaml
    /// </summary>
    public partial class PrivacyView : UserControl
    {
        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(PrivacyView), new PropertyMetadata(default(string)));

        public PrivacyView()
        {
            InitializeComponent();
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        private void UIElement_MouseEnter(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            //Description = (e.OriginalSource as IUIElement).Description;
        }

        private void UIElement_MouseLeave(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Description = string.Empty;
        }
    }
}