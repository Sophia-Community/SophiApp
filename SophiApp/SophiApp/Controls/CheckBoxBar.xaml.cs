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
    /// Логика взаимодействия для CheckBoxBar.xaml
    /// </summary>
    public partial class CheckBoxBar : UserControl, IUIElement
    {
        public CheckBoxBar()
        {
            InitializeComponent();
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        private static new readonly RoutedEvent MouseEnterEvent = EventManager.RegisterRoutedEvent("MouseEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CheckBoxBar));

        public new event RoutedEventHandler MouseEnter
        {
            add { AddHandler(MouseEnterEvent, value); }
            remove { RemoveHandler(MouseEnterEvent, value); }
        }

        private static new readonly RoutedEvent MouseLeaveEvent = EventManager.RegisterRoutedEvent("MouseLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CheckBoxBar));

        public new event RoutedEventHandler MouseLeave
        {
            add { AddHandler(MouseLeaveEvent, value); }
            remove { RemoveHandler(MouseLeaveEvent, value); }
        }

        public new bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            set { SetValue(IsMouseOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        public static new readonly DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(CheckBoxBar), new PropertyMetadata(default(bool)));

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(CheckBoxBar), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CheckBoxBar), new PropertyMetadata(default(string)));

        private void CheckBoxBar_MouseEnter(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseEnterEvent));

        private void CheckBoxBar_MouseLeave(object sender, MouseEventArgs e) => RaiseEvent(new RoutedEventArgs(MouseLeaveEvent));
        
    }
}
