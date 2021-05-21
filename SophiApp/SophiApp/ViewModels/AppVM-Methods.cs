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
        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void InitFields()
        {
            //TODO: logManager = new LogManager();
            localizationsHelper = new LocalizationsHelper();
            themesHelper = new ThemesHelper();
        }
    }
}
