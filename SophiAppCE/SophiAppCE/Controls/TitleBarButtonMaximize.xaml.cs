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

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для TitleBarButtonMaximize.xaml
    /// </summary>
    public partial class TitleBarButtonMaximize : UserControl
    {
        public TitleBarButtonMaximize()
        {
            InitializeComponent();
            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            State = (sender as MainWindow).WindowState;
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TitleBarButtonMaximize));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        private void TitleBarButtonMaximize_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        public WindowState State
        {
            get { return (WindowState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(WindowState), typeof(TitleBarButtonMaximize), new PropertyMetadata(WindowState.Normal));



        public Brush Hover
        {
            get { return (Brush)GetValue(HoverProperty); }
            set { SetValue(HoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Hover.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverProperty =
            DependencyProperty.Register("Hover", typeof(Brush), typeof(TitleBarButtonMaximize), new PropertyMetadata(default(Brush)));

        public Geometry IconMaximize
        {
            get { return (Geometry)GetValue(IconMaximizeProperty); }
            set { SetValue(IconMaximizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconMaximize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconMaximizeProperty =
            DependencyProperty.Register("IconMaximize", typeof(Geometry), typeof(TitleBarButtonMaximize), new PropertyMetadata(default(Geometry)));

        public Geometry IconRestore
        {
            get { return (Geometry)GetValue(IconRestoreProperty); }
            set { SetValue(IconRestoreProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconRestore.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconRestoreProperty =
            DependencyProperty.Register("IconRestore", typeof(Geometry), typeof(TitleBarButtonMaximize), new PropertyMetadata(default(Geometry)));

        public Brush IconBrush
        {
            get { return (Brush)GetValue(IconBrushProperty); }
            set { SetValue(IconBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconBrushProperty =
            DependencyProperty.Register("IconBrush", typeof(Brush), typeof(TitleBarButtonMaximize), new PropertyMetadata(default(Brush)));
    }
}
