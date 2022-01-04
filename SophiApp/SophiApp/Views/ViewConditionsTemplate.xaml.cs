using System.Windows;
using System.Windows.Controls;

namespace SophiApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ViewConditionsTemplate.xaml
    /// </summary>
    public partial class ViewConditionsTemplate : UserControl
    {
        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ViewConditionsTemplate), new PropertyMetadata(default));

        public ViewConditionsTemplate()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}