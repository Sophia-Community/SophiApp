using SophiAppCE.Classes;
using SophiAppCE.Controls;
using SophiAppCE.General;
using SophiAppCE.Managers;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.ViewModels
{
    class AppViewModel : INotifyPropertyChanged
    {
        private int activeSwitchBars = default(int);
        private string categoryPanelVisible = TagManager.Privacy;
        private double hamburgerMarkerVerticalLocation = default(double);
        private string categoryPanelScrollToUp = string.Empty;
        private UiLanguage uiLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper() == nameof(UiLanguage.RU) ? UiLanguage.RU : UiLanguage.EN;

        public ObservableCollection<SwitchBarModel> SwitchBarModelCollection { get; set; }
        private readonly Dictionary<UiLanguage, ResourceDictionary> localizedDictionaries = new Dictionary<UiLanguage, ResourceDictionary>()
        {
            { UiLanguage.EN, new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localization/EN.xaml", UriKind.Absolute)} },
            { UiLanguage.RU, new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localization/RU.xaml", UriKind.Absolute)} }
        };

        public AppViewModel()
        {
            SetUiLanguage();
            InitializationCollections();
        }

        private void SetUiLanguage()
        {
            Application.Current.Resources.MergedDictionaries.Add(localizedDictionaries[UiLanguage]);
        }

        private void InitializationCollections()
        {
            IEnumerable<JsonData> jsonRaw = AppManager.ParseJsonData();
            IEnumerable<JsonData> jsonParsed = jsonRaw.Where(j => AppManager.FileExistsAndHashed(filePath: j.Path, hashValue: j.Sha256) == true);
            IEnumerable<SwitchBarModel> switchBars = AppManager.CreateControlsByType<SwitchBarModel>(controlsCollections: jsonParsed, controlType: ControlType.Switch);
            SwitchBarModelCollection = new ObservableCollection<SwitchBarModel>(switchBars);
            SwitchBarModelCollection.ToList().ForEach(s => s.PropertyChanged += SwitchBarModel_PropertyChanged);
        }

        private void SwitchBarModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "State":
                    {
                        if ((sender as SwitchBarModel).State == true)
                            ActiveSwitchBars++;

                        else
                            ActiveSwitchBars--;

                        break;
                    }
                
                default:
                    break;
            }            
        }        

        public int ActiveSwitchBars
        {
            get => activeSwitchBars;
            set
            {
                activeSwitchBars = value;
                OnPropertyChanged("ActiveSwitchBars");
            }
        }
        
        public string CategoryPanelsVisibility
        {
            get => categoryPanelVisible;
            set
            {
                categoryPanelVisible = value;
                OnPropertyChanged("CategoryPanelsVisibility");
            }
        }
        
        public double HamburgerMarkerVerticalLocation
        {
            get => hamburgerMarkerVerticalLocation;
            set
            {
                hamburgerMarkerVerticalLocation = value;
                OnPropertyChanged("HamburgerMarkerVerticalLocation");
            }
        }
        
        public string CategoryPanelScrollToUp
        {
            get => categoryPanelScrollToUp;
            set
            {
                categoryPanelScrollToUp = value;
                OnPropertyChanged("CategoryPanelScrollToUp");
            }
        }        

        public UiLanguage UiLanguage
        {
            get => uiLanguage;
            set
            {
                uiLanguage = value;
                OnPropertyChanged("UiLanguage");
            }
        }

        private RelayCommand selectAllCommand;
        public RelayCommand SelectAllCommand => selectAllCommand ?? (selectAllCommand = new RelayCommand(SelectAll));

        private void SelectAll(object args)
        {
            string tag = (args as string[]).First();
            bool state = Convert.ToBoolean((args as string[]).Last());

            SwitchBarModelCollection.Where(s => s.Tag == tag && s.State != state)
                                    .ToList()
                                    .ForEach(s => s.State = state);
        }

        private RelayCommand hamburgerMenuButtonClickCommand;
        public RelayCommand HamburgerMenuButtonClickCommand => hamburgerMenuButtonClickCommand ?? (hamburgerMenuButtonClickCommand = new RelayCommand(HamburgerMenuButtonClick));

        private void HamburgerMenuButtonClick(object args)
        {
            HamburgerMenuButton button = args as HamburgerMenuButton;
            CategoryPanelsVisibility = Convert.ToString(button.Tag);
            HamburgerMarkerVerticalLocation = AppManager.GetParentElementRelativePoints(button).Y;
            CategoryPanelScrollToUp = Convert.ToString(button.Tag);
        }

        private RelayCommand changeUiLanguageCommand;

        public RelayCommand ChangeUiLanguageCommand => changeUiLanguageCommand ?? (changeUiLanguageCommand = new RelayCommand(ChangeUiLanguage));

        private void ChangeUiLanguage(object args)
        {
            UiLanguage = UiLanguage == UiLanguage.RU ? UiLanguage.EN : UiLanguage.RU;
            Application.Current.Resources.MergedDictionaries.Add(localizedDictionaries[UiLanguage]);
        }

        private RelayCommand applyingSettingsCommand;

        public RelayCommand ApplyingSettingsCommand => applyingSettingsCommand ?? (applyingSettingsCommand = new RelayCommand(ApplyingSettingsAsync));

        private async void ApplyingSettingsAsync(object args)
        {
            string pastTag = CategoryPanelsVisibility;
            CategoryPanelsVisibility = TagManager.ApplyingSettings;
            List<string> selectedItems = SwitchBarModelCollection.Where(s => s.State == true && AppManager.FileExistsAndHashed(s.Path, s.Sha256) == true)
                                                                 .Select(s => s.Path)
                                                                 .ToList();
            await PsExecutor.Execute(selectedItems);
            CategoryPanelsVisibility = pastTag;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}
