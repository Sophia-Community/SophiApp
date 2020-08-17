using SophiAppCE.Classes;
using SophiAppCE.Managers;
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
    /// Логика взаимодействия для Switch.xaml
    /// </summary>
    public partial class Switch : UserControl
    {
        internal bool IsChecked { get; private set; } = false;
        private SolidColorBrush checkedBrush = new SolidColorBrush((Color)Application.Current.TryFindResource("Color.Switch.Ellipse.Checked"));
        private SolidColorBrush uncheckedBrush = new SolidColorBrush((Color)Application.Current.TryFindResource("Color.Switch.Ellipse.Unchecked"));
        private Thickness ellipseRight = (Thickness)Application.Current.TryFindResource("Control.Switch.Ellipse.State.Right");
        private Thickness ellipseLeft = (Thickness)Application.Current.TryFindResource("Control.Switch.Ellipse.State.Left");        

        internal void ChangeState()
        {
            AnimationsManager.ShowThicknessAnimation(storyboardName: "Animation.Switch.Click",
                                                     animatedElement: SwitchEllipse,
                                                     animationValue: IsChecked == false ? ellipseRight : ellipseLeft);

            SwitchEllipse.Fill = IsChecked == false ? checkedBrush : uncheckedBrush;
            IsChecked = !IsChecked;            
        }

        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChangeState();
            //Command?.Execute(CommandParameter);
        }

        public Switch()
        {
            InitializeComponent();
        }



        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(Switch), new PropertyMetadata(OnStateChanged));

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Switch).ChangeState();
        }



        //public RelayCommand Command
        //{
        //    get { return (RelayCommand)GetValue(CommandProperty); }
        //    set { SetValue(CommandProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CommandProperty =
        //    DependencyProperty.Register("Command", typeof(RelayCommand), typeof(Switch), new UIPropertyMetadata(null));

        //public object CommandParameter
        //{
        //    get { return (object)GetValue(CommandParameterProperty); }
        //    set { SetValue(CommandParameterProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CommandParameterProperty =
        //    DependencyProperty.Register("CommandParameter", typeof(object), typeof(Switch), new UIPropertyMetadata(null));
    }
}
