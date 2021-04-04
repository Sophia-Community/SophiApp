using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SophiApp.ViewModels
{
    internal class AppVM : INotifyPropertyChanged
    {
        private UILanguage currentLocalization;
        private bool hamburgerIsEnabled = true;
        private string viewVisibilityByTag = Tags.ViewPrivacy;

        public AppVM()
        {
            UIElementClickedCommand = new RelayCommand(new Action<object>(UIElementClickedAsync));
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClicked));

            CurrentLocalization = Localizator.Initializing();
            InitializingUIModels();
            SetUIModelsSystemState();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string AppName => Application.Current.FindResource("CONST.AppName") as string;

        public UILanguage CurrentLocalization
        {
            get => currentLocalization;
            private set
            {
                currentLocalization = value;
                OnPropertyChanged("CurrentLocalization");
            }
        }

        public RelayCommand HamburgerClickedCommand { get; }

        //TODO: Deprecated?
        /// <summary>
        /// Determines the Hamburger state
        /// </summary>
        public bool HamburgerIsEnabled
        {
            get => hamburgerIsEnabled;
            private set
            {
                hamburgerIsEnabled = value;
                OnPropertyChanged("HamburgerIsEnabled");
            }
        }

        public RelayCommand SearchClickedCommand { get; }
        public RelayCommand UIElementClickedCommand { get; }
        public List<IUIElementModel> UIModels { get; set; }

        /// <summary>
        /// Tags define the displayed View
        /// </summary>
        public string ViewVisibilityByTag
        {
            get => viewVisibilityByTag;
            private set
            {
                viewVisibilityByTag = value;
                OnPropertyChanged("ViewVisibilityByTag");
            }
        }

        private void HamburgerClicked(object args)
        {
            var tag = args as string;

            if (ViewVisibilityByTag == tag)
                return;

            ViewVisibilityByTag = tag;
        }

        private void InitializingUIModels()
        {
            var parsedJsons = Parser.ParseJson(Properties.Resources.UIElementsData);
            UIModels = parsedJsons.Select(dto => Fabric.CreateElementModel(dto, CurrentLocalization)).ToList();
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void SearchClicked(object args)
        {
            var searchString = args as string;
            //TODO : Search not implemented !!!
        }

        private void SetUIModelsSystemState()
        {
            UIModels.ForEach(m =>
            {
                if (m.Id % 2 == 0) m.SetSystemState();
            });
        }

        private async void UIElementClickedAsync(object args)
        {
            await Task.Run(() =>
            {
                var id = Convert.ToInt32(args);
                UIModels.Where(m => m.Id == id).First().SetUserState();
            });
        }
    }
}