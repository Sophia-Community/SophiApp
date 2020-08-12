using SophiAppCE.Classes;
using SophiAppCE.Managers;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.ViewModels
{
    class SwitchBarPanelViewModel
    {
        public ObservableCollection<SwitchBar> EvenSwitchBars { get; set; } = new ObservableCollection<SwitchBar>();
        public ObservableCollection<SwitchBar> OddSwitchBars { get; set; } = new ObservableCollection<SwitchBar>();

        public SwitchBarPanelViewModel()
        {
            Loaded(AppManager.GetJsonDataByTag(TagManager.Privacy));           
        }

        internal void Loaded(List<JsonData> jsonData)
        {
            for (int i = 0; i < jsonData.Count; i++)
            {
                SwitchBar switchBar = new SwitchBar()
                {
                    Id = jsonData[i].Id,
                    Path = jsonData[i].Path,
                    HeaderEn = jsonData[i].HeaderEn,
                    HeaderRu = jsonData[i].HeaderRu,
                    DescriptionEn = jsonData[i].DescriptionEn,
                    DescriptionRu = jsonData[i].DescriptionRu,
                    Type = jsonData[i].Type,
                    Sha256 = jsonData[i].Sha256,
                    Tag = jsonData[i].Tag
                };

                if (i % 2 == 0)
                    EvenSwitchBars.Add(switchBar);
                else
                    OddSwitchBars.Add(switchBar);
            }
        }
    }
}
