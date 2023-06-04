using Microsoft.UI.Xaml.Controls;

using SophiApp.ViewModels;

namespace SophiApp.Views;

public sealed partial class TaskSchedulerPage : Page
{
    public TaskSchedulerViewModel ViewModel
    {
        get;
    }

    public TaskSchedulerPage()
    {
        ViewModel = App.GetService<TaskSchedulerViewModel>();
        InitializeComponent();
    }
}
