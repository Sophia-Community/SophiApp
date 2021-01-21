using SophiAppCE.Commons;
using SophiAppCE.EventsArgs;
using SophiAppCE.Helpers;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SophiAppCE.ViewModels
{
    internal class MainVM : INotifyPropertyChanged
    {
        private byte statusPagesVisibility = Tags.StatusPageStart;
        private AppLocalization appLocalization = new AppLocalization();
        private bool statusPageStartIsBusy = false;
        private string statusPageStartText = string.Empty;
        private RequirementsHelper requirementHelper = new RequirementsHelper();

        private void OnResultTextChanged(object sender, TestsResultEventArgs e)
        {
            StatusPageStartText = e.Text;
        }

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
        /// Defines the visibility of the ProgressBar on the Start page
        /// </summary>
        public bool StatusPageStartIsBusy
        {
            get => statusPageStartIsBusy;
            private set
            {
                statusPageStartIsBusy = value;
                OnPropertyChanged("StatusPageStartIsBusy");
            }
        }
        
        /// <summary>
        /// Defines the text on the Start page
        /// </summary>
        public string StatusPageStartText
        {
            get => statusPageStartText;
            private set
            {
                statusPageStartText = value;
                OnPropertyChanged("StatusPageStartText");
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
                
        private async Task DataInitializationAsync()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            await Task.Run(() =>
            {                
                StatusPageStartIsBusy = true;
                requirementHelper.ResultTextChanged += OnResultTextChanged;
                requirementHelper.TestsRun();                
                StatusPageStartIsBusy = false;

                if (requirementHelper.TestsResult) StatusPagesVisibility = Tags.StatusPageContent;
            });

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// App logical entry point
        /// </summary>
        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            DataInitializationAsync();
        }

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}