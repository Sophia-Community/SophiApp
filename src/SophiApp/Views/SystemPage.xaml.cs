using Microsoft.UI.Xaml.Controls;

using SophiApp.ViewModels;

namespace SophiApp.Views;

public sealed partial class SystemPage : Page
{
    public SystemViewModel ViewModel
    {
        get;
    }

    public SystemPage()
    {
        ViewModel = App.GetService<SystemViewModel>();
        InitializeComponent();
    }
}
