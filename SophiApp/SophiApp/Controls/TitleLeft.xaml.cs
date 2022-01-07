using System.Windows;
using System.Windows.Controls;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleLeft.xaml
    /// </summary>
    public partial class TitleLeft : UserControl
    {
        private new static readonly RoutedEvent MouseLeftButtonDownEvent = EventManager.RegisterRoutedEvent("MouseLeftButtonDown", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TitleLeft));

        public TitleLeft()
        {
            InitializeComponent();
        }

        public new event RoutedEventHandler MouseLeftButtonDown
        {
            add { AddHandler(MouseLeftButtonDownEvent, value); }
            remove { RemoveHandler(MouseLeftButtonDownEvent, value); }
        }

        private void GridTitle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseLeftButtonDownEvent));
    }
}