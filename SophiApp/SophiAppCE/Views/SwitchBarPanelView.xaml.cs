using SophiAppCE.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiAppCE.Views
{
    /// <summary>
    /// Логика взаимодействия для SwitchBarPanelView.xaml
    /// </summary>
    public partial class SwitchBarPanelView : UserControl
    {
        public SwitchBarPanelView()
        {
            InitializeComponent();
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SwitchBarPanelView), new PropertyMetadata(default(string)));
    }
}
