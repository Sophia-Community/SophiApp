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

        private void Window_Loaded(object sender, RoutedEventArgs e) => Application.Current.MainWindow.DataContext = new AppVM();
    }
}