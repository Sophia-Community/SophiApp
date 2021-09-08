using System.Windows;
using System.Windows.Controls;

namespace SophiApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ViewSettings.xaml
    /// </summary>
    public partial class ViewSettings : UserControl
    {
        public ViewSettings()
        {
            InitializeComponent();
        }

        private void ViewSettings_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                var scrollViewer = Template.FindName("ScrollViewerSettings", this) as ScrollViewer;
                scrollViewer?.ScrollToTop();
            }
        }
    }
}