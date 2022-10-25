using SophiApp.Helpers;
using SophiApp.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SophiApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ViewSystem.xaml
    /// </summary>
    public partial class ViewSystem : UserControl
    {
        // Using a DependencyProperty as the backing store for Tag.  This enables animation, styling, binding, etc...
        public new static readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(string), typeof(ViewSystem), new PropertyMetadata(default));

        public ViewSystem()
        {
            InitializeComponent();
            AddHandler(PreviewMouseWheelEvent, new MouseWheelEventHandler(OnChildMouseWheelEvent), true);
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
            var scrollViewer = Template.FindName("ScrollViewerSystem", this) as ScrollViewer;
            scrollViewer.RaiseEvent(mouseWheelEventArgs);
        }

        private void TextedElementsFilter(object sender, FilterEventArgs e) => e.Accepted = FilterHelper.FilterByTag(elementTag: (e.Item as TextedElement).Tag, viewTag: Tag);

        private void ViewSystem_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                var scrollViewer = Template.FindName("ScrollViewerSystem", this) as ScrollViewer;
                scrollViewer?.ScrollToTop();
            }
        }
    }
}