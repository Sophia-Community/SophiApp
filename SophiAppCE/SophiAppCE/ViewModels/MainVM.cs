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
        private AppLocalization appLocalization = new AppLocalization();
        private bool statusPageIsBusy = false;
        private string statusPageText = string.Empty;

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
        /// Current App language
        /// </summary>
        public LanguageName AppLocalization
        {
            get => appLocalization.Language;
            private set
            {
                appLocalization.Language = value;
                OnPropertyChanged("AppLocalization");
            }

        }

        /// <summary>
        /// Defines the visibility of the ProgressBar on the page
        /// </summary>
        public bool StatusPageIsBusy
        {
            get => statusPageIsBusy;
            private set
            {
                statusPageIsBusy = value;
                OnPropertyChanged("StatusPageIsBusy");
            }
        }
        
        /// <summary>
        /// Defines the text on the status page
        /// </summary>
        public string StatusPageText
        {
            get => statusPageText;
            private set
            {
                statusPageText = value;
                OnPropertyChanged("StatusPageText");
            }
        }

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