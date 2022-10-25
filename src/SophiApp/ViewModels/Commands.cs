using SophiApp.Commons;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        public RelayCommand AdvancedSettingsClickedCommand { get; private set; }
        public RelayCommand ApplyingSettingsCommand { get; private set; }
        public RelayCommand AppThemeChangeCommand { get; private set; }
        public RelayCommand DebugModeClickedCommand { get; private set; }
        public RelayCommand HamburgerClickedCommand { get; private set; }
        public RelayCommand HyperLinkClickedCommand { get; private set; }
        public RelayCommand LocalizationChangeCommand { get; private set; }
        public RelayCommand OpenUpdateCenterWindowCommand { get; private set; }
        public RelayCommand RadioGroupClickedCommand { get; private set; }
        public RelayCommand ResetTextedElementsStateCommand { get; private set; }
        public RelayCommand RiskAgreeClickedCommand { get; private set; }
        public RelayCommand SaveDebugLogCommand { get; private set; }
        public RelayCommand SearchClickedCommand { get; private set; }
        public RelayCommand SwitchUwpForAllUsersClickedCommand { get; private set; }
        public RelayCommand TaskSchedulerOpenClickedCommand { get; private set; }
        public RelayCommand TextedElementClickedCommand { get; private set; }
        public RelayCommand UwpButtonClickedCommand { get; private set; }
    }
}