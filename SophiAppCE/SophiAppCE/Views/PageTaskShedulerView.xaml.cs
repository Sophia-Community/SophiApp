using SophiAppCE.Controls;
using SophiAppCE.Models;
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

namespace SophiAppCE.Views
{
    /// <summary>
    /// Логика взаимодействия для PageTaskShedulerView.xaml
    /// </summary>
    public partial class PageTaskShedulerView : UserControl, ICommandSource
    {
        public PageTaskShedulerView()
        {
            InitializeComponent();
        }

        private void ExecuteCommand(object parameter)
        {
            ICommand command = Command;
            object commandParameter = parameter;
            IInputElement commandTarget = CommandTarget;

            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }

        private void Filter_OddControls(object sender, FilterEventArgs e)
        {
            ControlModel model = e.Item as ControlModel;
            e.Accepted = (model.Tag == Tag as string) && (model.Id % 2 == 1) ? true : false;
        }

        private void Filter_EvenControls(object sender, FilterEventArgs e)
        {
            ControlModel model = e.Item as ControlModel;
            e.Accepted = (model.Tag == Tag as string) && (model.Id % 2 == 0) ? true : false;
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(PageTaskShedulerView), new PropertyMetadata(default(string)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(PageTaskShedulerView), new PropertyMetadata(null));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(PageTaskShedulerView), new PropertyMetadata(null));

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(PageTaskShedulerView), new PropertyMetadata(null));

        private void SwitchBar_Clicked(object sender, RoutedEventArgs e) => ExecuteCommand((e.OriginalSource as SwitchBar).Id);

        private void ContentScroll_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;
            bool visibility = Convert.ToBoolean(e.NewValue);

            if (visibility == true)
                scroll.ScrollToTop();
        }
    }
}
