using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для WindowTitle.xaml
    /// </summary>
    public partial class WindowTitle : UserControl
    {
        public WindowTitle()
        {
            InitializeComponent();
        }

        private void WindowTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                Application.Current.MainWindow.DragMove();
        }
    }
}