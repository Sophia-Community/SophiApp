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
            dynamic control = e.Item;
        }

        //private void OddControls_Filter(object sender, FilterEventArgs e)
        //{
        //    SwitchBarModel switchBarModel = e.Item as SwitchBarModel;
        //    //e.Accepted = switchBarModel.Tag == Convert.ToString(Tag) && Convert.ToInt32(switchBarModel.Id.Split('x')[1]) % 2 == 1
        //    //           ? true : false;

        //    //IncreaseItemsCount(e.Accepted);
        //}

        private void EvenControls_Filter(object sender, FilterEventArgs e)
        {
            //SwitchBarModel switchBarModel = e.Item as SwitchBarModel;
            //e.Accepted = switchBarModel.Tag == Convert.ToString(Tag) && Convert.ToInt32(switchBarModel.Id.Split('x')[1]) % 2 == 0
            //           ? true : false;

            //IncreaseItemsCount(e.Accepted);
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
