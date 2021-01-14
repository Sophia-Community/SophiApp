using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
