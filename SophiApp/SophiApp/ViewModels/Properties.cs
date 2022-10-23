using SophiApp.Commons;
using SophiApp.Customisations;
using SophiApp.Helpers;
using SophiApp.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private bool advancedSettingsVisibility;
        private string applyingSettingsError;
        private string applyingSettingsErrorHasException;
        private string applyingSettingsErrorInApplying;
        private string buildName;
        private string conditionsHelperError;
        private List<Customisation> customActions;
        private bool debugMode;
        private List<TextedElement> foundTextedElement;
        private bool hamburgerHitTest;
        private InfoPanelVisibility infoPanelVisibility;
        private LocalizationsHelper localizationsHelper;
        private SearchState search;
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

        public string ApplyingSettingsError
        {
            get => applyingSettingsError;
            private set
            {
                applyingSettingsError = value;
                OnPropertyChanged(ApplyingSettingsErrorPropertyName);
            }
        }

        public string ApplyingSettingsErrorHasException
        {
            get => applyingSettingsErrorHasException;
            private set
            {
                applyingSettingsErrorHasException = value;
                OnPropertyChanged(ApplyingSettingsErrorHasExceptionPropertyName);
            }
        }

        public string ApplyingSettingsErrorInApplying
        {
            get => applyingSettingsErrorInApplying;
            private set
            {
                applyingSettingsErrorInApplying = value;
                OnPropertyChanged(ApplyingSettingsErrorInApplyingPropertyName);
            }
        }

        public string AppName { get => AppHelper.AppName; }

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
        public string BuildName { get => buildName; }

        public string ConditionsHelperError
        {
            get => conditionsHelperError;
            set
            {
                conditionsHelperError = value;
                OnPropertyChanged(ConditionsHelperErrorPropertyName);
            }
        }

        public List<Customisation> CustomActions
        {
            get => customActions;
            private set
            {
                customActions = value;
                OnPropertyChanged(CustomActionsPropertyName);
            }
        }

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

        public List<TextedElement> FoundTextedElement
        {
            get => foundTextedElement;
            private set
            {
                foundTextedElement = value;
                OnPropertyChanged(FoundTextedElementPropertyName);
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

        public InfoPanelVisibility InfoPanelVisibility
        {
            get => infoPanelVisibility;
            private set
            {
                infoPanelVisibility = value;
                OnPropertyChanged(InfoPanelVisibilityPropertyName);
            }
        }

        public bool IsRelease { get; } = AppHelper.IsRelease;

        public bool IsWindows11 { get; } = OsHelper.IsWindows11();
        public bool IsWindows11InsiderPreview { get; } = OsHelper.GetBuild() >= OsHelper.WIN11_INSIDER_BUILD_PATTERN;

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

        public SearchState Search
        {
            get => search;
            set
            {
                search = value;
                OnPropertyChanged(SearchPropertyName);
            }
        }

        public ConcurrentBag<TextedElement> TextedElements { get; private set; }

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

        public string Version { get => AppHelper.ShortVersion.ToString(); }

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