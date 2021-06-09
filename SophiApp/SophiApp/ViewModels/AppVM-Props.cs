using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Models;
using System.Collections.Generic;
using Debugger = SophiApp.Helpers.Debugger;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        //TODO: Check all controls IsEnabled property!

        private const string AdvancedSettingsVisibilityPropertyName = "AdvancedSettingsVisibility";
        private const string AppThemePropertyName = "AppTheme";
        private const string IsHitTestVisiblePropertyName = "IsHitTestVisible";
        private const string LocalizationPropertyName = "Localization";
        private const string TextedElementsChangedCounterPropertyName = "TextedElementsChangedCounter";
        private const string UpdateAvailablePropertyName = "UpdateAvailable";
        private const string VisibleViewByTagPropertyName = "VisibleViewByTag";

        private bool advancedSettingsVisibility;
        private Debugger debugger;
        private bool isHitTestVisible;
        private LocalizationsHelper localizationsHelper;
        private uint textedElementsChangedCounter;
        private ThemesHelper themesHelper;
        private bool updateAvailable;
        private string visibleViewByTag;

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

        public Theme AppTheme
        {
            get => themesHelper.Selected;
            private set
            {
                debugger.Write(DebuggerRecord.THEME, $"{value.Alias}");
                OnPropertyChanged(AppThemePropertyName);
            }
        }

        public RelayCommand AppThemeChangeCommand { get; private set; }
        public List<string> AppThemeList => themesHelper.GetNames();
        public RelayCommand ExpandingGroupClickedCommand { get; private set; }
        public RelayCommand ExportSettingsCommand { get; private set; }
        public RelayCommand HamburgerClickedCommand { get; private set; }
        public RelayCommand HyperLinkClickedCommand { get; private set; }
        public RelayCommand ImportSettingsCommand { get; private set; }

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
                debugger.Write(DebuggerRecord.LOCALIZATION, $"{value.Language}");
                OnPropertyChanged(LocalizationPropertyName);
            }
        }

        public RelayCommand LocalizationChangeCommand { get; private set; }
        public List<string> LocalizationList => localizationsHelper.GetNames();
        public RelayCommand RadioButtonGroupClickedCommand { get; private set; }
        public RelayCommand ResetOsToDefaultStateCommand { get; private set; }
        public RelayCommand SaveDebugLogCommand { get; private set; }
        public RelayCommand SearchClickedCommand { get; private set; }
        public RelayCommand TextedElementClickedCommand { get; private set; }
        public List<BaseTextedElement> TextedElements { get; private set; }

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
    }
}