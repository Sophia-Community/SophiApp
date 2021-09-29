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

        private void Window_Closed(object sender, System.EventArgs e) => ((sender as MainWindow).DataContext as AppVM).SaveDebugLogCommand.Execute(null);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var appVM = new AppVM();
            DataContext = appVM;
            appVM.InitData();
        }
    }
}