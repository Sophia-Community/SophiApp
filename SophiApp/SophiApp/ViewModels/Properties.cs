using SophiApp.Commons;
using SophiApp.Customisations;
using SophiApp.Helpers;
using SophiApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private bool advancedSettingsVisibility;
        private string conditionsHelperError;
        private List<Customisation> CustomActions;
        private bool debugMode;
        private bool hamburgerHitTest;
        private bool loadingPanelVisibility;
        private LocalizationsHelper localizationsHelper;
        private ThemesHelper themesHelper;
        private List<UwpElement> uwpElementsAllUsers;
        private List<UwpElement> uwpElementsCurrentUser;
        private ElementStatus uwpForAllUsersState;
        private bool viewsHitTest;
        private string visibleViewByTag;
        private bool windowCloseHitTest;

        public bool AdvancedSettingsVisibility
        {
            get => advancedSettingsVisibility;
            set
            {
                advancedSettingsVisibility = value;
                DebugHelper.AdvancedSettinsVisibility(value);
                OnPropertyChanged(AdvancedSettingsVisibilityPropertyName);
            }
        }

        public Theme AppSelectedTheme
        {
            get => themesHelper.SelectedTheme;
            private set
            {
                DebugHelper.SelectedTheme(value.Alias);
                OnPropertyChanged(AppSelectedThemePropertyName);
            }
        }

        public List<string> AppThemes => themesHelper.Themes.Select(theme => theme.Name).ToList();

        public string ConditionsHelperError
        {
            get => conditionsHelperError;
            set
            {
                conditionsHelperError = value;
                OnPropertyChanged(ConditionsHelperErrorPropertyName);
            }
        }

        public int CustomActionsCounter => CustomActions.Count;

        public bool DebugMode
        {
            get => debugMode;
            set
            {
                debugMode = value;
                DebugHelper.DebugMode(value);
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
                DebugHelper.SelectedLocalization($"{value.Language}");
                OnPropertyChanged(LocalizationPropertyName);
            }
        }

        public List<string> LocalizationList => localizationsHelper.GetNames();

        public List<TextedElement> TextedElements { get; private set; }

        public List<UwpElement> UwpElementsAllUsers
        {
            get => uwpElementsAllUsers;
            private set
            {
                uwpElementsAllUsers = value;
                OnPropertyChanged(UwpElementsAllUsersPropertyName);
            }
        }

        public List<UwpElement> UwpElementsCurrentUser
        {
            get => uwpElementsCurrentUser;
            private set
            {
                uwpElementsCurrentUser = value;
                OnPropertyChanged(UwpElementsCurrentUserPropertyName);
            }
        }

        public ElementStatus UwpForAllUsersState
        {
            get => uwpForAllUsersState;
            set
            {
                uwpForAllUsersState = value;
                DebugHelper.UwpForAllUsersState(value);
                OnPropertyChanged(UwpForAllUsersStatePropertyName);
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
                DebugHelper.VisibleViewChanged(value);
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