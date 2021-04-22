using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleButtonMin.xaml
    /// </summary>
    public partial class TitleButtonMin : UserControl
    {
        public TitleButtonMin()
        {
            InitializeComponent();
        }

        private void TitleButtonMin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }
}