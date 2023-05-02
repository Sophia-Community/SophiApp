using System.Windows;
using System.Windows.Controls;

namespace SophiApp.UI
{
    /// <summary>
    /// Interaction logic for WindowCaption.xaml
    /// </summary>
    public partial class WindowCaption : UserControl
    {
        public WindowCaption()
        {
            InitializeComponent();
        }

        public Thickness TextMargin
        {
            get { return (Thickness)GetValue(TextMarginProperty); }
            set { SetValue(TextMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextMarginProperty =
            DependencyProperty.Register("MyProperty", typeof(int), typeof(WindowCaption), new PropertyMetadata(default));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WindowCaption), new PropertyMetadata(default));

    }
}