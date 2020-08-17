using SophiAppCE.Classes;
using SophiAppCE.Managers;
using SophiAppCE.Models;
using SophiAppCE.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using SophiAppCE.Controls;

namespace SophiAppCE.ViewModels
{
    class SwitchBarPanelViewModel
    {
        public ObservableCollection<SwitchBarModel> OddSwitchBars { get; set; } = new ObservableCollection<SwitchBarModel>();
        public ObservableCollection<SwitchBarModel> EvenSwitchBars { get; set; } = new ObservableCollection<SwitchBarModel>();

        public SwitchBarPanelViewModel(string tag)
        {
            InitializeCollections(AppManager.GetJsonDataByTag(tag));
        }

        private RelayCommand selectAllCommand;
        public RelayCommand SelectAllCommand => selectAllCommand ?? (selectAllCommand = new RelayCommand(obj =>
                                                              {
                                                                  OddSwitchBars.Where(s => s.SwitchState == false)
                                                                               .ToList()
                                                                               .ForEach(s =>
                                                                               {
                                                                                   s.SwitchState = true;
                                                                               });                                                                  
                                                              }));

        internal void InitializeCollections(List<JsonData> jsonData)
        {
            for (int i = 1; i <= jsonData.Count; i++)
            {
                dynamic control = AppManager.GetControlByType(jsonData[i - 1]);

                if (i % 2 == 0)
                    EvenSwitchBars.Add(control);
                else
                    OddSwitchBars.Add(control);
            }
        }
    }
}
