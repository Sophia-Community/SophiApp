using SophiApp.Commons;
using SophiApp.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для SimpleCheckBox.xaml
    /// </summary>
    public partial class SimpleCheckBox : UserControl
    {
        private new static readonly RoutedEvent MouseEnterEvent = EventManager.RegisterRoutedEvent("MouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SimpleCheckBox));

        private new static readonly RoutedEvent MouseLeaveEvent = EventManager.RegisterRoutedEvent("MouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SimpleCheckBox));

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SimpleCheckBox), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SimpleCheckBox), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SimpleCheckBox), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for Hover.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverProperty =
            DependencyProperty.Register("Hover", typeof(Brush), typeof(SimpleCheckBox), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(ElementStatus), typeof(SimpleCheckBox), new PropertyMetadata(default));

        public SimpleCheckBox()
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

        public Brush Hover
        {
            get { return (Brush)GetValue(HoverProperty); }
            set { SetValue(HoverProperty, value); }
        }

        public ElementStatus IsChecked
        {
            get { return (ElementStatus)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        private void ContextMenu_DescriptionCopyClick(object sender, RoutedEventArgs e) => ClipboardHelper.CopyText(Description);

        private void ContextMenu_HeaderCopyClick(object sender, RoutedEventArgs e) => ClipboardHelper.CopyText(Header);

        private void SimpleCheckBox_MouseEnter(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseEnterEvent) { Source = Description });

        private void SimpleCheckBox_MouseLeave(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseLeaveEvent));

        private void SimpleCheckBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Command?.Execute(null);
    }
}