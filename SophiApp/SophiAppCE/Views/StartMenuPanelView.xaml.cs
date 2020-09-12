using SophiAppCE.Controls;
using SophiAppCE.Models;
using SophiAppCE.ViewModels;
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
    /// Логика взаимодействия для StartMenuPanelView.xaml
    /// </summary>
    public partial class StartMenuPanelView : UserControl
    {
        public StartMenuPanelView()
        {
            InitializeComponent();
        }

        public ushort ItemsCount
        {
            get { return (ushort)GetValue(ItemsCountProperty); }
            set { SetValue(ItemsCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsCountProperty =
            DependencyProperty.Register("ItemsCount", typeof(ushort), typeof(StartMenuPanelView), new PropertyMetadata(default(ushort)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(StartMenuPanelView), new PropertyMetadata(default(string)));

        private void Odd_Filter(object sender, FilterEventArgs e)
        {
            SwitchBarModel switchBarModel = e.Item as SwitchBarModel;
            e.Accepted = switchBarModel.Tag == Convert.ToString(Tag) && Convert.ToInt32(switchBarModel.Id.Split('x')[1]) % 2 == 1
                       ? true : false;

            IncreaseItemsCount(e.Accepted);
        }

        private void Even_Filter(object sender, FilterEventArgs e)
        {
            SwitchBarModel switchBarModel = e.Item as SwitchBarModel;
            e.Accepted = switchBarModel.Tag == Convert.ToString(Tag) && Convert.ToInt32(switchBarModel.Id.Split('x')[1]) % 2 == 0
                       ? true : false;

            IncreaseItemsCount(e.Accepted);
        }

        private void IncreaseItemsCount(bool value)
        {
            if (value)
                ItemsCount++;
        }

        private void SelectAllSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LeftStateSwitchBar stateSwitchBar = sender as LeftStateSwitchBar;
            (DataContext as AppViewModel).SelectAllCommand.Execute(new string[] { Convert.ToString(stateSwitchBar.Tag), Convert.ToString(stateSwitchBar.State) });
        }

        public bool ScrollToUpper
        {
            get { return (bool)GetValue(ScrollToUpperProperty); }
            set { SetValue(ScrollToUpperProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScrollToUpper.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollToUpperProperty =
            DependencyProperty.Register("ScrollToUpper", typeof(bool), typeof(StartMenuPanelView), new PropertyMetadata(OnScrollToUpperChanged));

        private static void OnScrollToUpperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as StartMenuPanelView).ContentPanelScrollViewer.ScrollToHome();
    }
}
