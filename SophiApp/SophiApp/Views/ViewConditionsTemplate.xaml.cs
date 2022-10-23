using System.Windows;
using System.Windows.Controls;

namespace SophiApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ViewConditionsTemplate.xaml
    /// </summary>
    public partial class ViewConditionsTemplate : UserControl
    {
        // Using a DependencyProperty as the backing store for TextContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextContentProperty =
            DependencyProperty.Register("TextContent", typeof(object), typeof(ViewConditionsTemplate), new PropertyMetadata(default));

        public ViewConditionsTemplate()
        {
            InitializeComponent();
        }

        public object TextContent
        {
            get { return (object)GetValue(TextContentProperty); }
            set { SetValue(TextContentProperty, value); }
        }
    }
}