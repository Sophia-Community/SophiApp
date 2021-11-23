using SophiApp.Commons;
using SophiApp.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для Switch.xaml
    /// </summary>
    public partial class AdvancedSwitch : UserControl
    {
        private new static readonly RoutedEvent MouseEnterEvent = EventManager.RegisterRoutedEvent("MouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AdvancedSwitch));

        private new static readonly RoutedEvent MouseLeaveEvent = EventManager.RegisterRoutedEvent("MouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AdvancedSwitch));

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(AdvancedSwitch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(AdvancedSwitch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(AdvancedSwitch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(AdvancedSwitch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(uint), typeof(AdvancedSwitch), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(ElementStatus), typeof(AdvancedSwitch), new PropertyMetadata(ElementStatus.UNCHECKED));

        public AdvancedSwitch()
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

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value);
        }

        public string Header
        {
            get => (string)GetValue(HeaderProperty); set => SetValue(HeaderProperty, value);
        }

        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public uint Id
        {
            get { return (uint)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public ElementStatus Status
        {
            get { return (ElementStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        private void AdvancedSwitch_MouseEnter(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseEnterEvent) { Source = Description });

        private void AdvancedSwitch_MouseLeave(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseLeaveEvent));

        private void AdvancedSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Command?.Execute(DataContext);

        private void ContextMenu_DescriptionCopyClick(object sender, RoutedEventArgs e) => ClipboardHelper.CopyText(Description);

        private void ContextMenu_HeaderCopyClick(object sender, RoutedEventArgs e) => ClipboardHelper.CopyText(Header);
    }
}