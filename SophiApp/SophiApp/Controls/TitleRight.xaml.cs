using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleRight.xaml
    /// </summary>
    public partial class TitleRight : UserControl
    {
        public TitleRight()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void IconClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Application.Current.MainWindow.Close();

        private void IconMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Application.Current.MainWindow.WindowState = WindowState.Minimized;

        private void TitleRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                Application.Current.MainWindow.DragMove();
            }
        }
    }
}