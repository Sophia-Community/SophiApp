using SophiApp.Commons;
using SophiApp.Helpers;
using System;
using System.ComponentModel;
using System.Windows;

namespace SophiApp.ViewModels
{
    internal class AppVM : INotifyPropertyChanged
    {
        private string activeViewTag = Tags.Privacy;
        private RelayCommand controlsClickedCommand;
        private RelayCommand hamburgerClickedCommand;
        private bool hamburgerIsEnabled = true;
        private RelayCommand searchClickedCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Using tags defines the displayed View
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
        public RelayCommand ControlsClickedCommand => controlsClickedCommand ?? new RelayCommand(ControlsClicked);

        public RelayCommand HamburgerClickedCommand => hamburgerClickedCommand ?? new RelayCommand(HamburgerButtonClicked);

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

        private void ControlsClicked(object args)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}