using SophiApp.Commons;
using SophiApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private const string AppThemePropertyName = "AppTheme";
        private const string LocalizationPropertyName = "Localization";

        private LocalizationsHelper localizationsHelper;
        private ThemesHelper themesHelper;

        public Localization Localization
        {
            get => localizationsHelper.Selected;
            private set
            {
                //TODO: logManager.AddDateTimeValueString(LogType.APP_LOCALIZATION_CHANGED, $"{value.Language}");
                OnPropertyChanged(LocalizationPropertyName);
            }
        }

        public Theme AppTheme
        {
            get => themesHelper.Selected;
            private set
            {
                //TODO: logManager.AddDateTimeValueString(LogType.THEME_CHANGED, $"{value.Alias}");
                OnPropertyChanged(AppThemePropertyName);
            }
        }
    }
}
