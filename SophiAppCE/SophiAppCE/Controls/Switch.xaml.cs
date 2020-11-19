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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SophiAppCE.Controls
{
    /// <summary>
    /// Логика взаимодействия для Switch.xaml
    /// </summary>
    public partial class Switch : UserControl, ICommandSource
    {
        private bool animationFinished = true;
        public Switch()
        {
            InitializeComponent();
        }

        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {   
            if (animationFinished)
            {
                animationFinished = false;
                ActualState = !ActualState;
                Ellipse ellipse = GetTemplateChild("SwitchEllipse") as Ellipse;
                Thickness marginLeft = (Thickness)Application.Current.TryFindResource("Margin.Switch.Ellipse.Left");
                Thickness marginRight = (Thickness)Application.Current.TryFindResource("Margin.Switch.Ellipse.Right");
                Animator.ShowThicknessAnimation(storyboardName: "Animation.Switch.Click", element: ellipse, from: ellipse.Margin,
                                                to: ellipse.Margin == marginLeft ? marginRight : marginLeft, isComplited: OnAnimationFinished);
                ExecuteCommand();
            }                
        }

        private void OnAnimationFinished(object sender, EventArgs e) => animationFinished = true;
       
        private void ExecuteCommand()
        {
            ICommand command = Command;
            object commandParameter = CommandParameter;
            IInputElement commandTarget = CommandTarget;

            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }

        public bool ActualState
        {
            get { return (bool)GetValue(ActualStateProperty); }
            set { SetValue(ActualStateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ActualState.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ActualStateProperty =
            DependencyProperty.Register("ActualState", typeof(bool), typeof(Switch), new PropertyMetadata(false));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(Switch), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(Switch), new PropertyMetadata(null));

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(Switch), new PropertyMetadata(null));
    }
}
