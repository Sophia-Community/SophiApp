using Microsoft.UI.Xaml.Controls;

using SophiApp.ViewModels;

namespace SophiApp.Views;

public sealed partial class PrivacyPage : Page
{
    public PrivacyViewModel ViewModel
    {
        get;
    }

    public PrivacyPage()
    {
        ViewModel = App.GetService<PrivacyViewModel>();
        InitializeComponent();
    }
}
