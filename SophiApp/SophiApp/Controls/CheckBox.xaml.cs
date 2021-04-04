using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для CheckBox.xaml
    /// </summary>
    public partial class CheckBox : UserControl
    {
        private static new readonly RoutedEvent MouseEnterEvent = EventManager.RegisterRoutedEvent("MouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CheckBox));

        private static new readonly RoutedEvent MouseLeaveEvent = EventManager.RegisterRoutedEvent("MouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CheckBox));

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CheckBox), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(CheckBox), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(CheckBox), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckBox), new PropertyMetadata(default(bool)));

        public CheckBox()
        {
            InitializeComponent();
        }

        public new event RoutedEventHandler MouseEnter
        {
            add { AddHandler(MouseEnterEvent, value); }
            remove { RemoveHandler(MouseEnterEvent, value); }
        }

        public new event RoutedEventHandler MouseLeave
        {
            add { AddHandler(MouseLeaveEvent, value); }
            remove { RemoveHandler(MouseLeaveEvent, value); }
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

        public int Id
        {
            get { return (int)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        private void CheckBox_MouseEnter(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseEnterEvent) { Source = Description });

        private void CheckBox_MouseLeave(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseLeaveEvent));
    }
}