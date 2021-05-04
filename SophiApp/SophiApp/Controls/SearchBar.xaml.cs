using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для SearchBar.xaml
    /// </summary>
    public partial class SearchBar : UserControl
    {
        public SearchBar()
        {
            InitializeComponent();
        }

        //TODO: Remove comments in XAML
        private void SearchBar_MouseLeave(object sender, MouseEventArgs e) => Keyboard.ClearFocus();
    }
}