using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Models
{
    public class SwitchBarModel
    {
    }

    public class SwitchBar : INotifyPropertyChanged
    {
        private bool isChecked = default(bool);

        public string Id { get; set; }
        public string Path { get; set; }
        public string HeaderEn { get; set; }
        public string HeaderRu { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionRu { get; set; }
        public string Type { get; set; }
        public string Sha256 { get; set; }
        public string Tag { get; set; }
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    RaiseCheckedChanged("IsChecked");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaiseCheckedChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
