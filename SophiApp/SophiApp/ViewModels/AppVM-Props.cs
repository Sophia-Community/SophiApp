using SophiApp.Commons;
using SophiApp.Helpers;
using Debugger = SophiApp.Helpers.Debugger;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private const string AppThemePropertyName = "AppTheme";
        private const string IsHitTestVisiblePropertyName = "IsHitTestVisible";
        private const string LocalizationPropertyName = "Localization";
        private const string UpdateAvailablePropertyName = "UpdateAvailable";
        private const string VisibleViewByTagPropertyName = "VisibleViewByTag";
        private Debugger debugger;
        private bool isHitTestVisible;
        private LocalizationsHelper localizationsHelper;
        private ThemesHelper themesHelper;
        private bool updateAvailable;
        private string visibleViewByTag;

        public Theme AppTheme
        {
            get => themesHelper.Selected;
            private set
            {
                debugger.AddRecord(DebuggerRecord.THEME, $"{value.Alias}");
                OnPropertyChanged(AppThemePropertyName);
            }
        }

        public RelayCommand HamburgerClickedCommand { get; private set; }

        public bool IsHitTestVisible
        {
            get => isHitTestVisible;
            private set
            {
                isHitTestVisible = value;
                OnPropertyChanged(IsHitTestVisiblePropertyName);
            }
        }

        public Localization Localization
        {
            get => localizationsHelper.Selected;
            private set
            {
                debugger.AddRecord(DebuggerRecord.LOCALIZATION, $"{value.Language}");
                OnPropertyChanged(LocalizationPropertyName);
            }
        }

        public RelayCommand SearchClickedCommand { get; private set; }

        public bool UpdateAvailable
        {
            get => updateAvailable;
            set
            {
                updateAvailable = value;
                OnPropertyChanged(UpdateAvailablePropertyName);
            }
        }

        public string VisibleViewByTag
        {
            get => visibleViewByTag;
            private set
            {
                visibleViewByTag = value;
                debugger.AddRecord(DebuggerRecord.VIEW, $"{value}");
                OnPropertyChanged(VisibleViewByTagPropertyName);
            }
        }
    }
}