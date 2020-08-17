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
            UiPanelViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.Ui);
            ContextMenuViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.ContextMenu);
            StartMenuPanelViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.StartMenu);
            SystemPanelViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.System);
            TaskShedulerPanelViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.TaskSheduler);
            SecurityPanelViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.Security);
            GamePanelViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.Game);
            UwpPanelViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.Uwp);
            OneDrivePanelViewControl.DataContext = new SwitchBarPanelViewModel(TagManager.OneDrive);
        }
    }
}
