using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SophiApp.ViewModels
{
    class AppVM : INotifyPropertyChanged
    {
        public string AppName => Application.Current.FindResource("CONST.AppName") as string;



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

    }
}
