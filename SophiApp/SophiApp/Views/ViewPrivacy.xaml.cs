using SophiApp.Models;
using System.Windows;
using System.Windows.Controls;

namespace SophiApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ViewPrivacy.xaml
    /// </summary>
    public partial class ViewPrivacy : UserControl
    {
        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ViewPrivacy), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ViewPrivacy), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Tag.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(string), typeof(ViewPrivacy), new PropertyMetadata(default));

        public ViewPrivacy()
        {
            InitializeComponent();
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public new string Tag
        {
            get { return (string)GetValue(TagProperty); }
            set { SetValue(TagProperty, value); }
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

        private void TextedElementsFilter(object sender, System.Windows.Data.FilterEventArgs e)
        {
            var element = e.Item as BaseTextedElement;
            e.Accepted = element.Tag == Tag;
        }
    }
}