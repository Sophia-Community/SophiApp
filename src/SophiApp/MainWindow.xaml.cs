using SophiApp.Helpers;
using SophiApp.ViewModels;
using System.Windows;

namespace SophiApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double startupLeft;
        private double startupTop;
        private double workAreaHeight = SystemParameters.WorkArea.Height;
        private double workAreaLeft = SystemParameters.WorkArea.Left;
        private double workAreaTop = SystemParameters.WorkArea.Top;
        private double workAreaWidth = SystemParameters.WorkArea.Width;

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(MainWindow), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for IsMaximized.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMaximizedProperty =
            DependencyProperty.Register("IsMaximized", typeof(bool), typeof(MainWindow), new PropertyMetadata(default));

        public MainWindow()
        {
            InitializeComponent();
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public bool IsMaximized
        {
            get { return (bool)GetValue(IsMaximizedProperty); }
            private set { SetValue(IsMaximizedProperty, value); }
        }

        private void GetStartupPosition()
        {
            startupLeft = Left;
            startupTop = Top;
        }

        private void ResizeWindow()
        {
            Top = IsMaximized ? startupTop : workAreaTop;
            Left = IsMaximized ? startupLeft : workAreaLeft;
            Width = IsMaximized ? MinWidth : workAreaWidth;
            Height = IsMaximized ? MinHeight : workAreaHeight;
            IsMaximized = IsMaximized.Invert();
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

        private void Title_CloseButtonClicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }

        private void Title_MinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            WindowState = WindowState.Minimized;
        }

        private void Title_MinMaxButtonClicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            ResizeWindow();
        }

        private void Title_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            DragMove();
        }

        private async void Window_Closed(object sender, System.EventArgs e)
        {
            var vm = DataContext as AppVM;
            vm.SaveDebugLogCommand.Execute(null);
            await vm.RemoveFrameworkLog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetStartupPosition();
            var appVM = new AppVM();
            DataContext = appVM;
            appVM.InitializeStartupConditionsAsync();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            e.Handled = true;
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                ResizeWindow();
            }
        }
    }
}