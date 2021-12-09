using SophiApp.Commons;
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

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для SimpleSwitch.xaml
    /// </summary>
    public partial class SimpleSwitch : UserControl
    {
        public ElementStatus Status
        {
            get { return (ElementStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(ElementStatus), typeof(SimpleSwitch), new PropertyMetadata(ElementStatus.UNCHECKED));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SimpleSwitch), new PropertyMetadata(default));

        public SimpleSwitch()
        {
            InitializeComponent();
        }

        private void SimpleSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Status = Status == ElementStatus.UNCHECKED ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;
            Command?.Execute(Status);
        }
    }
}
