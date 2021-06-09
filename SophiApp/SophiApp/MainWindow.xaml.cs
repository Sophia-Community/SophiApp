using SophiApp.ViewModels;
using System.Windows;

namespace SophiApp
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var appVM = new AppVM();
            Application.Current.MainWindow.DataContext = appVM;
            appVM.InitData();
        }
    }
}