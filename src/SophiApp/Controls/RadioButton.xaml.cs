using SophiApp.Commons;
using SophiApp.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для RadioButton.xaml
    /// </summary>
    public partial class RadioButton : UserControl
    {
        private new static readonly RoutedEvent MouseEnterEvent = EventManager.RegisterRoutedEvent("MouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadioButton));

        private new static readonly RoutedEvent MouseLeaveEvent = EventManager.RegisterRoutedEvent("MouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RadioButton));

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(RadioButton), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(RadioButton), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(RadioButton), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(uint), typeof(RadioButton), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(ElementStatus), typeof(RadioButton), new PropertyMetadata(ElementStatus.UNCHECKED));

        public RadioButton()
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

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
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

        private void ContextMenu_DescriptionCopyClick(object sender, RoutedEventArgs e) => ClipboardHelper.CopyText(Header);

        private void ContextMenu_HeaderCopyClick(object sender, RoutedEventArgs e) => ClipboardHelper.CopyText(Header);

        private void RadioButton_MouseEnter(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseEnterEvent) { Source = Description });

        private void RadioButton_MouseLeave(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseLeaveEvent));

        private void RadioButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Status == ElementStatus.UNCHECKED)
                Command?.Execute(DataContext);
        }
    }
}