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
    /// Логика взаимодействия для ChangesPanel.xaml
    /// </summary>
    public partial class ChangesPanel : UserControl
    {
        public ChangesPanel()
        {
            InitializeComponent();
        }


        public string ChangedText
        {
            get { return (string)GetValue(ChangedTextProperty); }
            set { SetValue(ChangedTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChangedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangedTextProperty =
            DependencyProperty.Register("ChangedText", typeof(string), typeof(ChangesPanel), new PropertyMetadata(default));



        public string SettingText
        {
            get { return (string)GetValue(SettingTextProperty); }
            set { SetValue(SettingTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SettingText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingTextProperty =
            DependencyProperty.Register("SettingText", typeof(string), typeof(ChangesPanel), new PropertyMetadata(default));


    }
}
