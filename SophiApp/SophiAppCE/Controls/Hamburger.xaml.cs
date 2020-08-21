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
            Canvas.SetTop(HamburgerMarker, GuiManager.GetParentRelativePoint(childrenElement: hamburgerMenuButton, parentElement: ContentCanvas).Y);
            (DataContext as AppViewModel).HamburgerClickCommand.Execute(hamburgerMenuButton.Tag);            
        }
    }
}
