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
        private bool mainWindowAllowClosing = true;

        /// <summary>
        /// Determines whether the window can be closed with the close button
        /// </summary>
        public bool MainWindowAllowClosing
        {
            get => mainWindowAllowClosing;
            private set
            {
                mainWindowAllowClosing = value;
                OnPropertyChanged("MainWindowAllowClosing");
            }
        }

        /// <summary>
        /// Program name and version, first 5 characters only
        /// </summary>
        public string AppTitle { get => AppHelper.GetFullName(); }

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
            Application.Current.MainWindow.ContentRendered += MainWindow_ContentRendered;
        }

        /// <summary>
        /// App logical entry point
        /// </summary>        
        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            //HACK: Simulate data initialization
            Thread.Sleep(5000);
            StatusPagesVisibility = Tags.StatusPageContent;

        }                
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}
