using SophiAppCE.Classes;
using SophiAppCE.Controls;
using SophiAppCE.Managers;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.ViewModels
{
    class AppViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SwitchBarModel> SwitchBarModelCollection { get; set; } = new ObservableCollection<SwitchBarModel>(GuiManager.ParseJsonData());

        public AppViewModel()
        {
            //TODO: Need refactoring!
            SwitchBarModelCollection.ToList()
                                    .ForEach(s =>
                                    {
                                        s.PropertyChanged += SwitchBarModel_PropertyChanged;
                                    });
        }

        private void SwitchBarModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                if ((sender as SwitchBarModel).State == true)
                {
                    ActiveSwitchBars++;
                }

                else
                {
                    ActiveSwitchBars--;
                }
            }
        }

        private int activeSwitchBars = default(int);

        public int ActiveSwitchBars
        {
            get => activeSwitchBars;
            set
            {
                activeSwitchBars = value;
                OnPropertyChanged("ActiveSwitchBars");
            }
        }

        private string categoryPanelVisibility = TagManager.Privacy;
        public string CategoryPanelsVisibility
        {
            get => categoryPanelVisibility;
            set
            {
                categoryPanelVisibility = value;
                OnPropertyChanged("CategoryPanelsVisibility");
            }
        }

        private double hamburgerMarkerVerticalLocation = default(double);
        public double HamburgerMarkerVerticalLocation
        { get => hamburgerMarkerVerticalLocation;
            set
            {
                hamburgerMarkerVerticalLocation = value;
                OnPropertyChanged("HamburgerMarkerVerticalLocation");
            }
        }

        private string categoryPanelScrollToUp = string.Empty;
        public string CategoryPanelScrollToUp
        {
            get => categoryPanelScrollToUp;
            set
            {
                categoryPanelScrollToUp = value;
                OnPropertyChanged("CategoryPanelScrollToUp");
            }
        }

        private RelayCommand selectAllCommand;
        public RelayCommand SelectAllCommand
        {
            get => selectAllCommand ?? (selectAllCommand = new RelayCommand(SelectAll));
        }

        private void SelectAll(object args)
        {
            string tag = (args as string[]).FirstOrDefault();
            bool state = Convert.ToBoolean((args as string[]).LastOrDefault());

            SwitchBarModelCollection.Where(s => s.Tag == tag && s.State != state)
                                    .ToList()
                                    .ForEach(s => s.State = state);
        }

        private RelayCommand hamburgerMenuButtonClickCommand;
        public RelayCommand HamburgerMenuButtonClickCommand
        {
            get => hamburgerMenuButtonClickCommand ?? (hamburgerMenuButtonClickCommand = new RelayCommand(HamburgerMenuButtonClick));
        }

        private void HamburgerMenuButtonClick(object args)
        {
            HamburgerMenuButton button = args as HamburgerMenuButton;
            CategoryPanelsVisibility = Convert.ToString(button.Tag);
            HamburgerMarkerVerticalLocation = AppManager.GetParentElementRelativePoint(button).Y;
            CategoryPanelScrollToUp = Convert.ToString(button.Tag);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}
