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
    /// Логика взаимодействия для ProgressBarDotsLine.xaml
    /// </summary>
    public partial class ProgressBarDotsLine : UserControl
    {
        public ProgressBarDotsLine()
        {
            InitializeComponent();
        }

        public double DotSize
        {
            get { return (double)GetValue(DotSizeProperty); }
            set { SetValue(DotSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DotSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DotSizeProperty =
            DependencyProperty.Register("DotSize", typeof(double), typeof(ProgressBarDotsLine), new PropertyMetadata(default(double)));

    }
}
