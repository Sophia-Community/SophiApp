using SophiAppCE.Commons;
using SophiAppCE.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.ViewModels
{
    class MainVM : INotifyPropertyChanged
    {
        private byte statusPagesVisibility = Tags.StatusPageStart;

        /// <summary>
        /// Defines the currently visible status page
        /// </summary>
        public byte StatusPagesVisibility
        {
            get => statusPagesVisibility;
            private set
            {
                statusPagesVisibility = value;
                OnPropertyChanged("StatusPagesVisibility");
            }
        }

        public MainVM()
        {
            
        }

        public string AppTitle { get => AppHelper.GetFullName(); }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}
