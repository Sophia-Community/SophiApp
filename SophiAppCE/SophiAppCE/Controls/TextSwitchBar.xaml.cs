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
    /// Логика взаимодействия для TextSwitchBar.xaml
    /// </summary>
    public partial class TextSwitchBar : UserControl
    {
        public TextSwitchBar()
        {
            InitializeComponent();
        }

        public string TextOn
        {
            get { return (string)GetValue(TextOnProperty); }
            set { SetValue(TextOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextOnProperty =
            DependencyProperty.Register("TextOn", typeof(string), typeof(TextSwitchBar), new PropertyMetadata(default(string)));

        public string TextOff
        {
            get { return (string)GetValue(TextOffProperty); }
            set { SetValue(TextOffProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextOff.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextOffProperty =
            DependencyProperty.Register("TextOff", typeof(string), typeof(TextSwitchBar), new PropertyMetadata(default(string)));

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for State.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(bool), typeof(TextSwitchBar), new PropertyMetadata(null));



    }
}
