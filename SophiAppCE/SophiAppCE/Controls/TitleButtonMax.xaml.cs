using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleButtonMax.xaml
    /// </summary>
    public partial class TitleButtonMax : UserControl
    {
        public TitleButtonMax()
        {
            InitializeComponent();
        }

        private void TitleButtonMax_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState == WindowState.Normal
                                                                                                    ? WindowState.Maximized
                                                                                                    : WindowState.Normal;
        }
    }
}