using SophiApp.Commons;
using SophiApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SophiApp.ViewModels
{
    class AppVM : INotifyPropertyChanged
    {
        private string activeViewTag = Tags.Privacy;
        private bool hamburgerIsEnabled = true;

        private RelayCommand hamburgerClickedCommand;

        public string AppName => Application.Current.FindResource("CONST.AppName") as string;

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

        /// <summary>
        /// Determines the Hamburger state
        /// </summary>
        public bool HamburgerIsEnabled { 
            get => hamburgerIsEnabled; 
            private set
            {
                hamburgerIsEnabled = value;
                OnPropertyChanged("HamburgerIsEnabled");
            }
        }

        public RelayCommand HamburgerClickedCommand => hamburgerClickedCommand ?? new RelayCommand(HamburgerButtonClicked);

        private void HamburgerButtonClicked(object args)
        {
            var tag = args as string;

            if (ActiveViewTag != tag) ActiveViewTag = tag;
        }        

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
    }
}
