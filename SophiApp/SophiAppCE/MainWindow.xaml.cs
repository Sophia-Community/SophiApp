using SophiAppCE.Managers;
using SophiAppCE.ViewModels;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Forms;

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
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            MainWindow mainWindow = sender as MainWindow;

            if (mainWindow.WindowState == WindowState.Maximized)
                mainWindow.WindowState = WindowState.Normal;                        
        }
    }
}
