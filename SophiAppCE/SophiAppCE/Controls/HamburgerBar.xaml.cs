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
            if (e.OriginalSource is HamburgerBarButton)
                MarkerMargin = (e.OriginalSource as HamburgerBarButton).Margin;        
        }

        public Thickness MarkerMargin
        {
            get { return (Thickness)GetValue(MarkerMarginProperty); }
            set { SetValue(MarkerMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MarkerMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkerMarginProperty =
            DependencyProperty.Register("MarkerMargin", typeof(Thickness), typeof(HamburgerBar), new PropertyMetadata(new Thickness(0, 0, 0, 0)));


    }
}
