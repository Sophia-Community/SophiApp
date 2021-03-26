using SophiApp.Commons;
using SophiApp.Helpers;
using System;
using System.ComponentModel;
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

            if (ActiveViewTag != tag) ActiveViewTag = tag;
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void SearchClicked(object args)
        {
            var searchString = args as string;
            //TODO : Search not implemented !!!            
        }
    }
}