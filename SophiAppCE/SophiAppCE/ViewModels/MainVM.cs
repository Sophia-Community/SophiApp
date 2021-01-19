using SophiAppCE.Commons;
using SophiAppCE.Helpers;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.ViewModels
{
    internal class MainVM : INotifyPropertyChanged
    {
        private byte statusPagesVisibility = Tags.StatusPageStart;

        public MainVM()
        {
            Application.Current.MainWindow.ContentRendered += MainWindow_ContentRendered;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        //HACK: Simulate data initialization
        private async Task DataInitializationAsync()
        {
            await Task.Run(async () =>
            {                
                Thread.Sleep(5000);
                StatusPagesVisibility = Tags.StatusPageContent;
            });
        }

        /// <summary>
        /// App logical entry point
        /// </summary>
        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            //HACK: Simulate data initialization
            //DataInitializationAsync();
        }

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}