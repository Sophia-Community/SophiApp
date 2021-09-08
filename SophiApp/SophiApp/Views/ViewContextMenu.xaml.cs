using SophiApp.Helpers;
using SophiApp.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SophiApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ViewContextMenu.xaml
    /// </summary>
    public partial class ViewContextMenu : UserControl
    {
        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ViewContextMenu), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Tag.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(string), typeof(ViewContextMenu), new PropertyMetadata(default));

        public ViewContextMenu()
        {
            InitializeComponent();
            AddHandler(PreviewMouseWheelEvent, new MouseWheelEventHandler(OnChildMouseWheelEvent), true);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public new string Tag
        {
            get => (string)GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }

        private void OnChildMouseWheelEvent(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            var mouseWheelEventArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) { RoutedEvent = MouseWheelEvent };
            var scrollViewer = Template.FindName("ScrollViewerContent", this) as ScrollViewer;
            scrollViewer.RaiseEvent(mouseWheelEventArgs);
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

        private void TextedElementsFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = FilterHelper.FilterByTag(elementTag: (e.Item as TextedElement).Tag, viewTag: Tag);
        }

        private void ViewContextMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                var scrollViewer = Template.FindName("ScrollViewerContent", this) as ScrollViewer;
                scrollViewer?.ScrollToTop();
            }
        }
    }
}