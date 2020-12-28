using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Логика взаимодействия для LinkBar.xaml
    /// </summary>
    public partial class LinkBar : UserControl
    {
        public LinkBar()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LinkBar), new PropertyMetadata(default(string)));

        public string Link
        {
            get { return (string)GetValue(LinkProperty); }
            set { SetValue(LinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Link.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LinkProperty =
            DependencyProperty.Register("Link", typeof(string), typeof(LinkBar), new PropertyMetadata("https://www.yandex.ru"));

        private void LinkBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.FileName = Link;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.Start();
        }
    }
}
