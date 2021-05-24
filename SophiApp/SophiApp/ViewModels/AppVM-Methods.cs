using SophiApp.Commons;
using SophiApp.Helpers;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private void HamburgerClicked(object args) => SetVisibleViewByTagProperty(args as string);

        private void InitCommands()
        {
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClickedAsync));
        }

        private void InitProps()
        {
            debugger = new Debugger();
            localizationsHelper = new LocalizationsHelper();
            themesHelper = new ThemesHelper();
            IsHitTestVisible = true;
            VisibleViewByTag = Tags.ViewGames; //TODO: Change to Tags.ViewLoading

            debugger.AddRecord(DebuggerRecord.LOCALIZATION, $"{Localization.Language}");
            debugger.AddRecord(DebuggerRecord.THEME, $"{AppTheme.Alias}");
            //debugger.AddRecord(DebuggerRecord.VIEW, $"{VisibleViewByTag}");
            debugger.AddRecord();
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private async void SearchClickedAsync(object args) => await SearchClickedAsync(args as string);

        private async Task SearchClickedAsync(string search)
        {
            await Task.Run(() =>
            {
                //TODO: Search not implemented
            });
        }

        private void SetVisibleViewByTagProperty(string tag) => VisibleViewByTag = tag;
    }
}