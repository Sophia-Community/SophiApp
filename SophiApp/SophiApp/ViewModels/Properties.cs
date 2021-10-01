using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private bool advancedSettingsVisibility;
        private List<CustomActionDto> customActions;
        private DebugHelper debugger;
        private bool debugMode;
        private bool hamburgerHitTest;
        private bool loadingPanelVisibility;
        private LocalizationsHelper localizationsHelper;
        private ThemesHelper themesHelper;
        private bool updateAvailable;
        private bool viewsHitTest;
        private string visibleViewByTag;
        private bool windowCloseHitTest;

        public bool AdvancedSettingsVisibility
        {
            get => advancedSettingsVisibility;
            set
            {
                advancedSettingsVisibility = value;
                debugger.StatusEntry($"Advanced settings is visible: {value}");
                OnPropertyChanged(AdvancedSettingsVisibilityPropertyName);
            }
        }

        public Theme AppSelectedTheme
        {
            get => themesHelper.SelectedTheme;
            private set
            {
                debugger.StatusEntry($"Theme selected: {value.Alias}");
                OnPropertyChanged(AppSelectedThemePropertyName);
            }
        }

        public List<string> AppThemes => themesHelper.Themes.Select(theme => theme.Name).ToList();

        public int CustomActionsCounter => customActions.Count;

        public bool DebugMode
        {
            get => debugMode;
            set
            {
                debugMode = value;
                debugger.StatusEntry($"Debug mode is: {value}");
                OnPropertyChanged(DebugModePropertyName);
            }
        }

        public bool HamburgerHitTest
        {
            get => hamburgerHitTest;
            private set
            {
                hamburgerHitTest = value;
                OnPropertyChanged(HamburgerHitTestPropertyName);
            }
        }

        public bool LoadingPanelVisibility
        {
            get => loadingPanelVisibility;
            set
            {
                loadingPanelVisibility = value;
                OnPropertyChanged(LoadingPanelVisibilityPropertyName);
            }
        }

        public Localization Localization
        {
            get => localizationsHelper.Selected;
            private set
            {
                debugger.StatusEntry($"Localization selected: {value.Language}");
                OnPropertyChanged(LocalizationPropertyName);
            }
        }

        public List<string> LocalizationList => localizationsHelper.GetNames();

        public List<TextedElement> TextedElements { get; private set; }

        public bool UpdateAvailable
        {
            get => updateAvailable;
            set
            {
                updateAvailable = value;
                OnPropertyChanged(UpdateAvailablePropertyName);
            }
        }

        public bool ViewsHitTest
        {
            get => viewsHitTest;
            private set
            {
                viewsHitTest = value;
                OnPropertyChanged(ViewsHitTestPropertyName);
            }
        }

        public string VisibleViewByTag
        {
            get => visibleViewByTag;
            private set
            {
                visibleViewByTag = value;
                debugger.StatusEntry($"Active view is: {value}");
                OnPropertyChanged(VisibleViewByTagPropertyName);
            }
        }

        public bool WindowCloseHitTest
        {
            get => windowCloseHitTest;
            private set
            {
                windowCloseHitTest = value;
                OnPropertyChanged(WindowCloseHitTestPropertyName);
            }
        }
    }
}