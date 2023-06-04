using Microsoft.UI.Xaml.Controls;

using SophiApp.ViewModels;

namespace SophiApp.Views;

public sealed partial class ContextMenuPage : Page
{
    public ContextMenuViewModel ViewModel
    {
        get;
    }

    public ContextMenuPage()
    {
        ViewModel = App.GetService<ContextMenuViewModel>();
        InitializeComponent();
    }
}
