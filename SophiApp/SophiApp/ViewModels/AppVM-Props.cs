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

        private const string AppThemePropertyName = "AppTheme";
        private const string IsHitTestVisiblePropertyName = "IsHitTestVisible";
        private const string LocalizationPropertyName = "Localization";
        private const string TextedElementsChangedCounterPropertyName = "TextedElementsChangedCounter";
        private const string UpdateAvailablePropertyName = "UpdateAvailable";
        private const string VisibleViewByTagPropertyName = "VisibleViewByTag";
        private Debugger debugger;
        private bool isHitTestVisible;
        private LocalizationsHelper localizationsHelper;
        private uint textedElementsChangedCounter;
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
                debugger.AddRecord(DebuggerRecord.VIEW, $"{value}");
                OnPropertyChanged(VisibleViewByTagPropertyName);
            }
        }
    }
}