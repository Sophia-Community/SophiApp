using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleLeft.xaml
    /// </summary>
    public partial class TitleLeft : UserControl
    {
        public TitleLeft()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void TitleLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                Application.Current.MainWindow.DragMove();
        }
    }
}