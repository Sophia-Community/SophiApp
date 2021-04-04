using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для Switch.xaml
    /// </summary>
    public partial class Switch : UserControl
    {
        private static readonly new RoutedEvent MouseEnterEvent = EventManager.RegisterRoutedEvent("MouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Switch));

        private static readonly new RoutedEvent MouseLeaveEvent = EventManager.RegisterRoutedEvent("MouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Switch));

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(Switch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(Switch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(int), typeof(Switch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool), typeof(Switch), new PropertyMetadata(default));

        public Switch()
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
            add { AddHandler(MouseEnterEvent, value); }
            remove { RemoveHandler(MouseEnterEvent, value); }
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

        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        private void Switch_MouseEnter(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseEnterEvent) { Source = Description });

        private void Switch_MouseLeave(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseLeaveEvent));
    }
}