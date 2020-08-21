using SophiAppCE.Managers;
using SophiAppCE.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для StateSwitchBar.xaml
    /// </summary>
    public partial class StateSwitchBar : UserControl
    {
        private SolidColorBrush checkedBrush = new SolidColorBrush((Color)Application.Current.TryFindResource("Color.Switch.Ellipse.Checked"));
        private SolidColorBrush uncheckedBrush = new SolidColorBrush((Color)Application.Current.TryFindResource("Color.Switch.Ellipse.Unchecked"));
        private Thickness ellipseRight = (Thickness)Application.Current.TryFindResource("Control.Switch.Ellipse.State.Right");
        private Thickness ellipseLeft = (Thickness)Application.Current.TryFindResource("Control.Switch.Ellipse.State.Left");
        public bool State { get; private set; } = default(bool);

        public StateSwitchBar()
        {
            InitializeComponent();
        }

        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            State = !State;
            AnimationsManager.ShowThicknessAnimation(storyboardName: "Animation.Switch.Click",
                                                     animatedElement: SwitchEllipse,
                                                     animationValue: State == true ? ellipseRight : ellipseLeft);

            SwitchEllipse.Fill = State == true ? checkedBrush : uncheckedBrush;
            Title.Text = State == true ? TextOff : TextOn;            
        }

        public string TextOff
        {
            get { return (string)GetValue(TextOffProperty); }
            set { SetValue(TextOffProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextOff.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextOffProperty =
            DependencyProperty.Register("TextOff", typeof(string), typeof(StateSwitchBar), new PropertyMetadata(default(string)));

        public string TextOn
        {
            get { return (string)GetValue(TextOnProperty); }
            set { SetValue(TextOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextOnProperty =
            DependencyProperty.Register("TextOn", typeof(string), typeof(StateSwitchBar), new PropertyMetadata(default(string)));
    }
}

