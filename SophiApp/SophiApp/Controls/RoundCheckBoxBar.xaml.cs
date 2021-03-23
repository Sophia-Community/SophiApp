using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SophiApp.Interfaces;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для RoundCheckBoxBar.xaml
    /// </summary>
    public partial class RoundCheckBoxBar : UserControl, IUIElement
    {
        public RoundCheckBoxBar()
        {
            InitializeComponent();
        }

        private static new readonly RoutedEvent MouseEnterEvent = EventManager.RegisterRoutedEvent("MouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoundCheckBoxBar));

        public new event RoutedEventHandler MouseEnter
        {
            add { AddHandler(MouseEnterEvent, value); }
            remove { RemoveHandler(MouseEnterEvent, value); }
        }

        private static new readonly RoutedEvent MouseLeaveEvent = EventManager.RegisterRoutedEvent("MouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RoundCheckBoxBar));

        public new event RoutedEventHandler MouseLeave
        {
            add { AddHandler(MouseEnterEvent, value); }
            remove { RemoveHandler(MouseEnterEvent, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(RoundCheckBoxBar), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(RoundCheckBoxBar), new PropertyMetadata(default(string)));

        private void RoundCheckBoxBar_MouseEnter(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            RaiseEvent(new RoutedEventArgs(MouseEnterEvent));
        }

        private void RoundCheckBoxBar_MouseLeave(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            RaiseEvent(new RoutedEventArgs(MouseLeaveEvent));
        }
    }
}
