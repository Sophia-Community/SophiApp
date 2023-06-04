using Microsoft.UI.Xaml.Controls;

using SophiApp.ViewModels;

namespace SophiApp.Views;

public sealed partial class ProVersionPage : Page
{
    public ProVersionViewModel ViewModel
    {
        get;
    }

    public ProVersionPage()
    {
        ViewModel = App.GetService<ProVersionViewModel>();
        InitializeComponent();
    }
}
