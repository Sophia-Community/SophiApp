using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SophiApp.Interfaces;

namespace SophiApp.Views
{
    /// <summary>
    /// Логика взаимодействия для PrivacyView.xaml
    /// </summary>
    public partial class PrivacyView : UserControl
    {
        public PrivacyView()
        {
            InitializeComponent();
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(PrivacyView), new PropertyMetadata(default(string)));

        private void UIElement_MouseEnter(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Description = (e.OriginalSource as IUIElement).Description;
        }

        private void UIElement_MouseLeave(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Description = string.Empty;
        }
    }
}
