using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleRight.xaml
    /// </summary>
    public partial class TitleRight : UserControl
    {
        private static readonly RoutedEvent CloseButtonClickedEvent = EventManager.RegisterRoutedEvent("CloseButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TitleRight));

        private static readonly RoutedEvent MinimizeButtonClickedEvent = EventManager.RegisterRoutedEvent("MinimizeButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TitleRight));

        private static readonly RoutedEvent MinMaxButtonClickedEvent = EventManager.RegisterRoutedEvent("MinMaxButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TitleRight));

        private new static readonly RoutedEvent MouseLeftButtonDownEvent = EventManager.RegisterRoutedEvent("MouseLeftButtonDown", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TitleRight));

        public TitleRight()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler CloseButtonClicked
        {
            add { AddHandler(CloseButtonClickedEvent, value); }
            remove { RemoveHandler(CloseButtonClickedEvent, value); }
        }

        public event RoutedEventHandler MinimizeButtonClicked
        {
            add { AddHandler(MinimizeButtonClickedEvent, value); }
            remove { RemoveHandler(MinimizeButtonClickedEvent, value); }
        }

        public event RoutedEventHandler MinMaxButtonClicked
        {
            add { AddHandler(MinMaxButtonClickedEvent, value); }
            remove { RemoveHandler(MinMaxButtonClickedEvent, value); }
        }

        public new event RoutedEventHandler MouseLeftButtonDown
        {
            add { AddHandler(MouseLeftButtonDownEvent, value); }
            remove { RemoveHandler(MouseLeftButtonDownEvent, value); }
        }

        private void GridClosed_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => RaiseEvent(new RoutedEventArgs(CloseButtonClickedEvent));

        private void GridMinimize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => RaiseEvent(new RoutedEventArgs(MinimizeButtonClickedEvent));

        private void GridRestore_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => RaiseEvent(new RoutedEventArgs(MinMaxButtonClickedEvent));

        private void GridTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseLeftButtonDownEvent));
    }
}