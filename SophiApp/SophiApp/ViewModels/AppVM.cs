using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiApp.ViewModels
{
    internal class AppVM : INotifyPropertyChanged
    {
        private string viewVisibilityByTag = Tags.ViewPrivacy;
        private RelayCommand uiElementClickedCommand;
        private RelayCommand hamburgerClickedCommand;
        private bool hamburgerIsEnabled = true;
        private RelayCommand searchClickedCommand;
        private UILanguage currentLocalization;

        public List<IUIElementModel> UIModels { get; private set; }
        public AppVM()
        {
            CurrentLocalization = Localizator.Initializing();
            InitializingModels();            
        }

        private void InitializingModels()
        {
            var parsedJsons = Parser.ParseJson(Properties.Resources.UIElementsData);
            UIModels = parsedJsons.Select(dto => Fabric.CreateElementModel(dto, CurrentLocalization)).ToList();
        }
                
        public event PropertyChangedEventHandler PropertyChanged;

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

        public UILanguage CurrentLocalization 
        {
            get => currentLocalization;
            private set
            {
                currentLocalization = value;
                OnPropertyChanged("CurrentLocalization");
            }
        }

        public string AppName => Application.Current.FindResource("CONST.AppName") as string;

        public RelayCommand UIElementClickedCommand => uiElementClickedCommand ?? new RelayCommand(UIElementClicked);

        public RelayCommand HamburgerClickedCommand => hamburgerClickedCommand ?? new RelayCommand(HamburgerClicked);

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

        public RelayCommand SearchClickedCommand => searchClickedCommand ?? new RelayCommand(SearchClicked);

        private void UIElementClicked(object args)
        {
            var id = Convert.ToInt32(args);
            var model = UIModels.Where(m => m.Id == id).First();
            model.ChangeActualState();            
        }

        private void HamburgerClicked(object args)
        {
            var tag = args as string;

            if (ViewVisibilityByTag == tag) 
                return;

            ViewVisibilityByTag = tag;
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void SearchClicked(object args)
        {
            var searchString = args as string;
            //TODO : Search not implemented !!!            
        }
    }
}