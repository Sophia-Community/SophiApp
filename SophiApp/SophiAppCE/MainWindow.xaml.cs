using SophiAppCE.Managers;
using SophiAppCE.ViewModels;
using System;
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
            InitializeViewsModels();
            GuiManager.SetWindowBlur();                        
        }

        private void InitializeViewsModels()
        {
            PrivacyPanelViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.Privacy);
        }
    }
}
