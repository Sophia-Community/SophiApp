using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для ComboListBox.xaml
    /// </summary>
    public partial class ComboListBox : UserControl
    {
        public ComboListBox()
        {
            InitializeComponent();            
        }

        private void ButtonOpenPopup_Click(object sender, RoutedEventArgs e)
        {
            var popup = Template.FindName("Popup", this) as Popup;
            popup.IsOpen = true;
        }

        private void ButtonClosePopup_Click(object sender, RoutedEventArgs e)
        {
            var popup = Template.FindName("Popup", this) as Popup;
            popup.IsOpen = false;
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ComboListBox), new PropertyMetadata(default));

    }
}
