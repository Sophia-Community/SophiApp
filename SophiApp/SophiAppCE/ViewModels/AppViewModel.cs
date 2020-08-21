using SophiAppCE.Classes;
using SophiAppCE.Managers;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.ViewModels
{
    class AppViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SwitchBarModel> SwitchBarModelCollection { get; set; } = new ObservableCollection<SwitchBarModel>(GuiManager.ParseJsonData());

        private string switchBarPanelVisibility = TagManager.Privacy;
        public string SwitchBarPanelVisibility
        {
            get => switchBarPanelVisibility;
            set
            {
                switchBarPanelVisibility = value;
                OnPropertyChanged("SwitchBarPanelVisibility");                
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

        private RelayCommand hamburgerClickCommand;
        public RelayCommand HamburgerClickCommand
        {
            get => hamburgerClickCommand ?? (hamburgerClickCommand = new RelayCommand(HamburgerClick));
        }

        private void HamburgerClick(object args)
        {
            SwitchBarPanelVisibility = Convert.ToString(args);            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}
