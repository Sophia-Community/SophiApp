using SophiAppCE.Classes;
using SophiAppCE.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        private void Odd_Filter(object sender, FilterEventArgs e)
        {
            SwitchBarModel switchBarModel = e.Item as SwitchBarModel;
            e.Accepted = switchBarModel.Tag == Convert.ToString(Tag) && Convert.ToInt32(switchBarModel.Id.Split('x')[1]) % 2 == 1
                       ? true : false;
        }

        private void Even_Filter(object sender, FilterEventArgs e)
        {
            SwitchBarModel switchBarModel = e.Item as SwitchBarModel;
            e.Accepted = switchBarModel.Tag == Convert.ToString(Tag) && Convert.ToInt32(switchBarModel.Id.Split('x')[1]) % 2 == 0
                       ? true : false;
        }
    }
}