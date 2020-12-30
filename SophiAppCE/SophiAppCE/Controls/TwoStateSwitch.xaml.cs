using SophiAppCE.Helpers;
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
    /// Логика взаимодействия для TwoStateSwitch.xaml
    /// </summary>
    public partial class TwoStateSwitch : UserControl, ICommandSource
    {
        private bool animationFinished = true;
        private Thickness marginLeft = (Thickness)Application.Current.TryFindResource("Margin.Switch.Ellipse.Left");
        private Thickness marginRight = (Thickness)Application.Current.TryFindResource("Margin.Switch.Ellipse.Right");        

        public TwoStateSwitch()
        {
            InitializeComponent();            
        }

        private void ExecuteCommand()
        {
            ICommand command = Command;
            object commandParameter = CommandParameter;
            IInputElement commandTarget = CommandTarget;

            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }

        public string TextLeft
        {
            get { return (string)GetValue(TextLeftProperty); }
            set { SetValue(TextLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextLeftProperty =
            DependencyProperty.Register("TextLeft", typeof(string), typeof(TwoStateSwitch), new PropertyMetadata(default(string)));

        public string TextRight
        {
            get { return (string)GetValue(TextRightProperty); }
            set { SetValue(TextRightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextRight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextRightProperty =
            DependencyProperty.Register("TextRight", typeof(string), typeof(TwoStateSwitch), new PropertyMetadata(default(string)));

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(TwoStateSwitch), new PropertyMetadata(default(bool)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TwoStateSwitch), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(TwoStateSwitch), new PropertyMetadata(null));

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(TwoStateSwitch), new PropertyMetadata(null));

        private void TwoStateSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (animationFinished)
            {
                animationFinished = false;
                Ellipse ellipse = GetTemplateChild("SwitchEllipse") as Ellipse;
                Animator.ShowThicknessAnimation(storyboardName: "Animation.Switch.Click", element: ellipse, from: ellipse.Margin,
                                                to: State == true ? marginLeft : marginRight, isComplited: OnAnimationFinished);
                ExecuteCommand();
            }
        }

        private void OnAnimationFinished(object sender, EventArgs e) => animationFinished = true;

        private void TwoStateSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            Ellipse ellipse = GetTemplateChild("SwitchEllipse") as Ellipse;
            ellipse.Margin = State == true ? marginRight : marginLeft;
        }
    }
}
