using SophiApp.ViewModels;
using System;
using System.Windows;

namespace SophiApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double height;
        private double left;
        private double top;
        private double width;

        public bool IsMaximized
        {
            get { return (bool)GetValue(IsMaximizedProperty); }
            private set { SetValue(IsMaximizedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMaximized.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMaximizedProperty =
            DependencyProperty.Register("IsMaximized", typeof(bool), typeof(MainWindow), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(MainWindow), new PropertyMetadata(default));

        public MainWindow()
        {
            InitializeComponent();
        }

        public void ChangeSize()
        {
            if (!IsMaximized)
            {
                Height = SystemParameters.WorkArea.Height;                
                Left = SystemParameters.WorkArea.Left;
                Top = SystemParameters.WorkArea.Top;
                Width = SystemParameters.WorkArea.Width;
                IsMaximized = true;
                return;
            }

            SetPosition();
            IsMaximized = false;
        }

        private void SetPosition()
        {
            Height = height;
            Left = left;
            Top = top;
            Width = width;
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
            GetPosition();
            var appVM = new AppVM();
            DataContext = appVM;
            appVM.InitData();
        }

        private void GetPosition()
        {
            height = Height;
            left = Left;
            top = Top;
            width = Width;
        }

    }
}