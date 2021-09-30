using SophiApp.ViewModels;
using System.Windows;

namespace SophiApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(MainWindow), new PropertyMetadata(default));

        public MainWindow()
        {
            InitializeComponent();
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        private void TextedElement_MouseEnter(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Description = e.OriginalSource as string;
        }

        private void TextedElement_MouseLeave(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Description = string.Empty;
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