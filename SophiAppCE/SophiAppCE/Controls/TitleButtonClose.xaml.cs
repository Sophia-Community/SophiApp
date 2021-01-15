using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleButtonClose.xaml
    /// </summary>
    public partial class TitleButtonClose : UserControl
    {
        public TitleButtonClose()
        {
            InitializeComponent();
        }

        private void TitleButtonClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}