using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для DropDownList.xaml
    /// </summary>
    public partial class DropDownList : UserControl
    {
        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DropDownList), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for IsOpened.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenedProperty =
            DependencyProperty.Register("IsOpened", typeof(bool), typeof(DropDownList), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for SelectedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedTextProperty =
            DependencyProperty.Register("SelectedText", typeof(string), typeof(DropDownList), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(List<string>), typeof(DropDownList), new PropertyMetadata(default));

        public DropDownList()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public bool IsOpened
        {
            get { return (bool)GetValue(IsOpenedProperty); }
            set { SetValue(IsOpenedProperty, value); }
        }

        public string SelectedText
        {
            get { return (string)GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
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

        private void DropDownList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void ListBoxContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = Template.FindName("ListBoxContent", this) as ListBox;
            var popup = Template.FindName("Popup", this) as Popup;

            if ((listBox.SelectedItem as string) == SelectedText || listBox.SelectedItem is null)
            {
                return;
            }

            popup.IsOpen = false;
            Command?.Execute(listBox.SelectedItem);
        }
    }
}