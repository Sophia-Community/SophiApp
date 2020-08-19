using SophiAppCE.Classes;
using SophiAppCE.Managers;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.ViewModels
{
    class AppViewModel
    {
        public ObservableCollection<SwitchBarModel> SwitchBarModelCollection { get; set; } = new ObservableCollection<SwitchBarModel>(GuiManager.ParseJsonData());       

        private RelayCommand selectAllCommand;
        public RelayCommand SelectAllCommand
        {
            get => selectAllCommand ?? (selectAllCommand = new RelayCommand(SelectAll));
        }

        private void SelectAll(object args)
        {
            string[] argsArray = Array.ConvertAll(object args, Convert.ToString);
            SwitchBarModelCollection.Where(s => s.Tag == Convert.ToString(args[0]))
                                    .ToList()
                                    .ForEach(s => s.State = !s.State);                                    
        }
    }
}
