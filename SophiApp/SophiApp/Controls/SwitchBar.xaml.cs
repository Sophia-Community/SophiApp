using SophiApp.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для SwitchBar.xaml
    /// </summary>
    public partial class SwitchBar : UserControl, IUIElement
    {
        private static new readonly RoutedEvent MouseEnterEvent = EventManager.RegisterRoutedEvent("MouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SwitchBar));

        private static new readonly RoutedEvent MouseLeaveEvent = EventManager.RegisterRoutedEvent("MouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SwitchBar));

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SwitchBar), new PropertyMetadata(default(string)));

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SwitchBar), new PropertyMetadata(default(string)));

        public SwitchBar()
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

        private void SwitchBar_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseEnterEvent));

        private void SwitchBar_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseLeaveEvent));
    }
}