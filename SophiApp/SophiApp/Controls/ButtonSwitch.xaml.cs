using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для ButtonSwitch.xaml
    /// </summary>
    public partial class ButtonSwitch : UserControl
    {
        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(ButtonSwitch), new PropertyMetadata(default(object)));

        // Using a DependencyProperty as the backing store for OffCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffCommandProperty =
            DependencyProperty.Register("OffCommand", typeof(ICommand), typeof(ButtonSwitch), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for OnCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OnCommandProperty =
            DependencyProperty.Register("OnCommand", typeof(ICommand), typeof(ButtonSwitch), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(ButtonSwitch), new PropertyMetadata(default(bool)));

        // Using a DependencyProperty as the backing store for TextLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextOffProperty =
            DependencyProperty.Register("TextOff", typeof(string), typeof(ButtonSwitch), new PropertyMetadata(default(string)));

        // Using a DependencyProperty as the backing store for TextRight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextOnProperty =
            DependencyProperty.Register("TextOn", typeof(string), typeof(ButtonSwitch), new PropertyMetadata(default(string)));

        public ButtonSwitch()
        {
            InitializeComponent();
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ICommand OffCommand
        {
            get { return (ICommand)GetValue(OffCommandProperty); }
            set { SetValue(OffCommandProperty, value); }
        }

        public ICommand OnCommand
        {
            get { return (ICommand)GetValue(OnCommandProperty); }
            set { SetValue(OnCommandProperty, value); }
        }

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public string TextOff
        {
            get { return (string)GetValue(TextOffProperty); }
            set { SetValue(TextOffProperty, value); }
        }

        public string TextOn
        {
            get { return (string)GetValue(TextOnProperty); }
            set { SetValue(TextOnProperty, value); }
        }

        private void BorderOff_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (State)
            {
                State = false;
                OffCommand?.Execute(CommandParameter);
            }
        }

        private void BorderOn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!State)
            {
                State = true;
                OnCommand?.Execute(CommandParameter);
            }
        }
    }
}