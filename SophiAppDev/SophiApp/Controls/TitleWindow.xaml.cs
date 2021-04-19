using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleWindow.xaml
    /// </summary>
    public partial class TitleWindow : UserControl
    {
        public TitleWindow()
        {
            InitializeComponent();
        }

        private void TitleWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                Application.Current.MainWindow.DragMove();
            }
        }
    }
}