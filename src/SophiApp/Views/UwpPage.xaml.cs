using Microsoft.UI.Xaml.Controls;

using SophiApp.ViewModels;

namespace SophiApp.Views;

public sealed partial class UwpPage : Page
{
    public UwpViewModel ViewModel
    {
        get;
    }

    public UwpPage()
    {
        ViewModel = App.GetService<UwpViewModel>();
        InitializeComponent();
    }
}
