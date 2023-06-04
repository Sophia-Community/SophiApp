using Microsoft.UI.Xaml.Controls;

using SophiApp.ViewModels;

namespace SophiApp.Views;

public sealed partial class SecurityPage : Page
{
    public SecurityViewModel ViewModel
    {
        get;
    }

    public SecurityPage()
    {
        ViewModel = App.GetService<SecurityViewModel>();
        InitializeComponent();
    }
}
