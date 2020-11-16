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
    /// Логика взаимодействия для CommandSwitch.xaml
    /// </summary>
    public partial class CommandSwitch : UserControl, ICommandSource
    {
        public CommandSwitch()
        {
            InitializeComponent();
        }

        private void Switch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            State = !State;
            Ellipse ellipse = GetTemplateChild("SwitchEllipse") as Ellipse;

            SolidColorBrush brushUnchecked = Application.Current.TryFindResource("Brush.SwitchBar.Ellipse.Unchecked") as SolidColorBrush;
            SolidColorBrush brushChecked = Application.Current.TryFindResource("Brush.SwitchBar.Ellipse.Checked") as SolidColorBrush;

            Thickness marginLeft = (Thickness)Resources["Margin.Switch.Ellipse.Left"];
            Thickness marginRight = (Thickness)Resources["Margin.Switch.Ellipse.Right"];

            Storyboard storyboard = Resources["Animation.Margin.Ellipse"] as Storyboard;
            ThicknessAnimation marginAnimation = storyboard.Children.First() as ThicknessAnimation;
            marginAnimation.To = ellipse.Margin == marginRight ? marginLeft : marginRight;
            storyboard.Begin(ellipse);

            ellipse.Fill = ellipse.Margin == marginRight ? brushUnchecked : brushChecked;
            ExecuteCommand();
        }

        private void ExecuteCommand()
        {
            ICommand command = Command;
            object commandParameter = CommandParameter;
            IInputElement commandTarget = CommandTarget;

            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(CommandSwitch), new PropertyMetadata(false));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandSwitch), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandSwitch), new PropertyMetadata(null));

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(CommandSwitch), new PropertyMetadata(null));
    }
}
