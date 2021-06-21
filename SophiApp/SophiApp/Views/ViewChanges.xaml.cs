using System.Windows;
using System.Windows.Controls;

namespace SophiApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ViewChanges.xaml
    /// </summary>
    public partial class ViewChanges : UserControl
    {
        // Using a DependencyProperty as the backing store for ChangedText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChangedTextProperty =
            DependencyProperty.Register("ChangedText", typeof(string), typeof(ViewChanges), new PropertyMetadata(default));

        // Using a DependencyProperty as the backing store for SettingText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SettingTextProperty =
            DependencyProperty.Register("SettingText", typeof(string), typeof(ViewChanges), new PropertyMetadata(default));

        public ViewChanges()
        {
            InitializeComponent();
        }

        public string ChangedText
        {
            get { return (string)GetValue(ChangedTextProperty); }
            set { SetValue(ChangedTextProperty, value); }
        }

        public string SettingText
        {
            get { return (string)GetValue(SettingTextProperty); }
            set { SetValue(SettingTextProperty, value); }
        }
    }
}