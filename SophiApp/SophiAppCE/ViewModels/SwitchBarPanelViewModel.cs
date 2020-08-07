using SophiAppCE.Classes;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.ViewModels
{
    class SwitchBarPanelViewModel
    {
        public ObservableCollection<SwitchBar> EvenSwitchBars { get; set; } = new ObservableCollection<SwitchBar>();
        public ObservableCollection<SwitchBar> OddSwitchBars { get; set; } = new ObservableCollection<SwitchBar>();

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


            //TODO: Задать вопрос на тостере!
            //jsonData.ForEach(j =>
            //{



            //    SwitchBars.Add(new SwitchBar()
            //    {
            //        Id = j.Id,
            //        Path = j.Path,
            //        HeaderEn = j.HeaderEn,
            //        HeaderRu = j.HeaderRu,
            //        DescriptionEn = j.DescriptionEn,
            //        DescriptionRu = j.DescriptionRu,
            //        Type = j.Type,
            //        Sha256 = j.Sha256,
            //        Tag = j.Tag
            //    });

        }
    }
}
