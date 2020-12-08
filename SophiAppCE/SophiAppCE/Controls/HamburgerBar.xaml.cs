using SophiAppCE.Helpers;
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

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для HamburgerBar.xaml
    /// </summary>
    public partial class HamburgerBar : UserControl
    {
        public HamburgerBar()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ActiveMarker = (e.OriginalSource as HamburgerBarButton).Tag as string;
        }

        public string ActiveMarker
        {
            get { return (string)GetValue(ActiveMarkerProperty); }
            set { SetValue(ActiveMarkerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActiveMarker.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActiveMarkerProperty =
            DependencyProperty.Register("ActiveMarker", typeof(string), typeof(HamburgerBar), new PropertyMetadata(Tags.Privacy));
    }
}
