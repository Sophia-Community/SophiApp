using SophiApp.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace SophiApp.Controls
{
    /// <summary>
    /// Логика взаимодействия для ItemsList.xaml
    /// </summary>
    public partial class ItemsList : UserControl
    {
        // Using a DependencyProperty as the backing store for IsExpanded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(ItemsList), new PropertyMetadata(true));

        public ItemsList()
        {
            InitializeComponent();
        }

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        private void ExpanderWrapper_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => IsExpanded = !IsExpanded;

        private void InContainerFilter(object sender, FilterEventArgs e)
        {
            var childIds = (DataContext as IItemsListModel).ChildIds;
            var elementId = (e.Item as IUIElementModel).Id;
            e.Accepted = childIds.Contains(elementId);
        }
    }
}