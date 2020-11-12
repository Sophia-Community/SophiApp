using SophiAppCE.Helpers;
using SophiAppCE.Models;
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

namespace SophiAppCE.Views
{
    /// <summary>
    /// Логика взаимодействия для PagePrivacyView.xaml
    /// </summary>
    public partial class PagePrivacyView : UserControl
    {
        public PagePrivacyView()
        {
            InitializeComponent();
        }

        private void Filter_OddControls(object sender, FilterEventArgs e)
        {
            ControlModel model = e.Item as ControlModel;
            e.Accepted = (model.Tag == Tag as string) && (model.Id % 2 == 1) ? true : false;
        }

        private void Filter_EvenControls(object sender, FilterEventArgs e)
        {
            ControlModel model = e.Item as ControlModel;
            e.Accepted = (model.Tag == Tag as string) && (model.Id % 2 == 0) ? true : false;
        }       

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(PagePrivacyView), new PropertyMetadata(default(string)));        
    }
}
