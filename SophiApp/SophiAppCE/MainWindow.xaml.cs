using SophiAppCE.Controls;
using SophiAppCE.Managers;
using SophiAppCE.ViewModels;
using SophiAppCE.Views;
using System.Windows;

namespace SophiAppCE
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GuiManager.SetWindowBlur();
            

            //SwitchBarPanelViewModel privacySwitchBarPanelViewModel = new SwitchBarPanelViewModel();
            //privacySwitchBarPanelViewModel.Loaded(jsonData: appManager.GetJsonDataByTag(TagManager.Privacy));
            //PrivacyPanelViewControl.DataContext = privacySwitchBarPanelViewModel;
        }


    }
}
