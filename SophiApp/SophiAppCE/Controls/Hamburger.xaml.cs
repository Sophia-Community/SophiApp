using SophiAppCE.Managers;
using SophiAppCE.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для Hamburger.xaml
    /// </summary>
    public partial class Hamburger : UserControl
    {
        public Hamburger()
        {
            InitializeComponent();
        }

        private void HamburgerMenuButton_Click(object sender, RoutedEventArgs e)
        {
            HamburgerMenuButton hamburgerMenuButton = e.OriginalSource as HamburgerMenuButton;            
            (DataContext as AppViewModel).HamburgerMenuButtonClickCommand.Execute(hamburgerMenuButton);
            e.Handled = true;
        }

        private void HamburgerApplyButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Change this !!!
            (DataContext as AppViewModel).ChangeUiLanguageCommand.Execute(null);
            e.Handled = true;
        }
    }
}
