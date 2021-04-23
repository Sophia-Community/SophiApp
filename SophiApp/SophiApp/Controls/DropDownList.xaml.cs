using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для DropDownList.xaml
    /// </summary>
    public partial class DropDownList : UserControl
    {
        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(List<string>), typeof(DropDownList), new PropertyMetadata(default));

        public DropDownList()
        {
            InitializeComponent();
        }

        public List<string> Source
        {
            get { return (List<string>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private void ButtonClosePopup_Click(object sender, RoutedEventArgs e)
        {
            var popup = Template.FindName("Popup", this) as Popup;
            popup.IsOpen = false;
        }

        private void ButtonOpenPopup_Click(object sender, RoutedEventArgs e)
        {
            var popup = Template.FindName("Popup", this) as Popup;
            popup.IsOpen = true;
        }
    }
}