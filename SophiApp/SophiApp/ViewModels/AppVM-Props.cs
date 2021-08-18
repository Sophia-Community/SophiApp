using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Models;
using System.Collections.Generic;
using System.Linq;
using Debugger = SophiApp.Helpers.Debugger;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private const string AdvancedSettingsVisibilityPropertyName = "AdvancedSettingsVisibility";
        private const string AppSelectedThemePropertyName = "AppSelectedTheme";
        private const string AppThemesPropertyName = "AppThemes";
        private const string HamburgerHitTestPropertyName = "HamburgerHitTest";
        private const string LoadingPanelVisibilityPropertyName = "LoadingPanelVisibility";
        private const string LocalizationPropertyName = "Localization";
        private const string TextedElementsChangedCounterPropertyName = "TextedElementsChangedCounter";
        private const string UpdateAvailablePropertyName = "UpdateAvailable";
        private const string ViewsHitTestPropertyName = "ViewsHitTest";
        private const string VisibleViewByTagPropertyName = "VisibleViewByTag";
        private const string WindowCloseHitTestPropertyName = "WindowCloseHitTest";

        private bool advancedSettingsVisibility;
        private Debugger debugger;
        private bool hamburgerHitTest;
        private bool loadingPanelVisibility;
        private LocalizationsHelper localizationsHelper;
        private uint textedElementsChangedCounter;
        private ThemesHelper themesHelper;
        private bool updateAvailable;
        private bool viewsHitTest;
        private string visibleViewByTag;
        private bool windowCloseHitTest;

        public RelayCommand AdvancedSettingsClickedCommand { get; private set; }

        public bool AdvancedSettingsVisibility
        {
            get => advancedSettingsVisibility;
            set
            {
                advancedSettingsVisibility = value;
                debugger.Write(DebuggerRecord.ADVANCED_SETTINGS_VISIBILITY, $"{value}");
                OnPropertyChanged(AdvancedSettingsVisibilityPropertyName);
            }
        }

        public RelayCommand ApplyingSettingsCommand { get; private set; }

        public Theme AppSelectedTheme
        {
            get => themesHelper.SelectedTheme;
            private set
            {
                debugger.Write(DebuggerRecord.THEME, $"{value.Alias}");
                OnPropertyChanged(AppSelectedThemePropertyName);
            }
        }

        public RelayCommand AppThemeChangeCommand { get; private set; }

        public List<string> AppThemes => themesHelper.Themes.Select(theme => theme.Name).ToList();

        public RelayCommand ExpandingGroupClickedCommand { get; private set; }

        public RelayCommand ExportSettingsCommand { get; private set; }

        public RelayCommand HamburgerClickedCommand { get; private set; }

        public bool HamburgerHitTest
        {
            get => hamburgerHitTest;
            private set
            {
                hamburgerHitTest = value;
                OnPropertyChanged(HamburgerHitTestPropertyName);
            }
        }

        public RelayCommand HyperLinkClickedCommand { get; private set; }

        public RelayCommand ImportSettingsCommand { get; private set; }

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
                debugger.Write(DebuggerRecord.LOCALIZATION, $"{value.Language}");
                OnPropertyChanged(LocalizationPropertyName);
            }
        }

        public RelayCommand LocalizationChangeCommand { get; private set; }

        public List<string> LocalizationList => localizationsHelper.GetNames();

        public RelayCommand RadioGroupClickedCommand { get; private set; }

        public RelayCommand ResetTextedElementsStateCommand { get; private set; }

        public RelayCommand SaveDebugLogCommand { get; private set; }

        public RelayCommand SearchClickedCommand { get; private set; }

        public RelayCommand TextedElementClickedCommand { get; private set; }

        public List<TextedElement> TextedElements { get; private set; }

        public uint TextedElementsChangedCounter
        {
            get => textedElementsChangedCounter;
            set
            {
                textedElementsChangedCounter = value;
                OnPropertyChanged(TextedElementsChangedCounterPropertyName);
            }
        }

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
                debugger.Write(DebuggerRecord.VIEW, $"{value}");
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