using SophiAppCE.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для SwitchBar.xaml
    /// </summary>
    public partial class SwitchBar : UserControl
    {        
        private Thickness marginLeft = (Thickness)Application.Current.TryFindResource("Margin.Switch.Ellipse.Left");
        private Thickness marginRight = (Thickness)Application.Current.TryFindResource("Margin.Switch.Ellipse.Right");
        private SolidColorBrush brushChecked = (SolidColorBrush)Application.Current.TryFindResource("Brush.SwitchBar.Ellipse.Checked");
        private SolidColorBrush brushUnchecked = (SolidColorBrush)Application.Current.TryFindResource("Brush.SwitchBar.Ellipse.Unchecked");
        private bool animationFinished = true;

        public SwitchBar()
        {
            InitializeComponent();            
        }

        private static readonly RoutedEvent ClickedEvent = EventManager.RegisterRoutedEvent("Clicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SwitchBar));

        public event RoutedEventHandler Clicked
        {
            add { AddHandler(ClickedEvent, value); }
            remove { RemoveHandler(ClickedEvent, value); }
        }

        public UInt16 Id
        {
            get { return (UInt16)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Id.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register("Id", typeof(UInt16), typeof(SwitchBar), new PropertyMetadata(default(UInt16)));

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(SwitchBar), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(SwitchBar), new PropertyMetadata(default(string)));

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(SwitchBar), new PropertyMetadata(false));       

        public bool ActualState
        {
            get { return (bool)GetValue(ActualStateProperty); }
            set { SetValue(ActualStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualStateProperty =
            DependencyProperty.Register("ActualState", typeof(bool), typeof(SwitchBar), new PropertyMetadata(false, new PropertyChangedCallback(OnActualStateChanged)));

        private static void OnActualStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as SwitchBar).ChangeState();

        private void ChangeState()
        {
            Ellipse ellipse = GetTemplateChild("SwitchEllipse") as Ellipse;
            Animator.ShowThicknessAnimation(storyboardName: "Animation.Switch.Click", element: ellipse, from: ellipse.Margin,
                                            to: ellipse.Margin == marginLeft ? marginRight : marginLeft, isComplited: OnAnimationFinished);
        }

        private void OnAnimationFinished(object sender, EventArgs e) => animationFinished = true;
        
        private void SwitchBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (animationFinished)
            {
                animationFinished = false;
                RaiseEvent(new RoutedEventArgs(ClickedEvent));
            }
                
        }        
    }
}
