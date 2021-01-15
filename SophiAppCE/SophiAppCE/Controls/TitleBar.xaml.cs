using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TitleBar), new PropertyMetadata(string.Empty));

        public TitleBar()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                Application.Current.MainWindow.DragMove();
            }
        }
    }
}