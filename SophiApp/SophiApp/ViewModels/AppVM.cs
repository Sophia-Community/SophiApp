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
        private string activeViewTag = Tags.ViewPrivacy;
        private RelayCommand uielementClickedCommand;
        private RelayCommand hamburgerButtonClickedCommand;
        private bool hamburgerIsEnabled = true;
        private RelayCommand searchClickedCommand;
        private UILocalization uiLocalization = new UILocalization();      

        public List<IUIElementModel> UIModels { get; private set; }
        public AppVM()
        {            
            InitializingModels();            
        }

        private void InitializingModels()
        {
            var parsedJsons = Parser.ParseJson(Properties.Resources.UIElementsData);
            UIModels = parsedJsons.Select(dto => Fabric.CreateElementModel(dto, UILocalization)).ToList();
        }
                
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Tags define the displayed View
        /// </summary>
        public string ActiveViewTag
        {
            get => activeViewTag;
            private set
            {
                activeViewTag = value;
                OnPropertyChanged("ActiveViewTag");
            }
        }

        public UILanguage UILocalization 
        {
            get => uiLocalization.Current;
            private set => OnPropertyChanged("UILocalization");
        }

        public string AppName => Application.Current.FindResource("CONST.AppName") as string;

        public RelayCommand UIElementClickedCommand => uielementClickedCommand ?? new RelayCommand(UIElementClicked);

        public RelayCommand HamburgerButtonClickedCommand => hamburgerButtonClickedCommand ?? new RelayCommand(HamburgerButtonClicked);

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
            //TODO : Click not implemented !!!
        }

        private void HamburgerButtonClicked(object args)
        {
            var tag = args as string;
            if (ActiveViewTag != tag) 
                ActiveViewTag = tag;
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void SearchClicked(object args)
        {
            var searchString = args as string;
            //TODO : Search not implemented !!!            
        }
    }
}