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
        private byte viewVisibility = Tags.LoadingView;
        private AppLocalization appLocalization = new AppLocalization();        
        private string loadingViewText = string.Empty;
        private RequirementsHelper requirementHelper = new RequirementsHelper();

        private void OnRequirementHelperTextChanged(object sender, TestsResultEventArgs e)
        {
            LoadingViewText = e.Text;
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
        /// Defines the text on the Loading View
        /// </summary>
        public string LoadingViewText
        {
            get => loadingViewText;
            private set
            {
                loadingViewText = value;
                OnPropertyChanged("LoadingViewText");
            }
        }

        /// <summary>
        /// Defines the currently visible View
        /// </summary>
        public byte ViewVisibility
        {
            get => viewVisibility;
            private set
            {
                viewVisibility = value;
                OnPropertyChanged("ViewVisibility");
            }
        }
                
        private async Task DataInitializationAsync()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            await Task.Run(() =>
            {   
                requirementHelper.ResultTextChanged += OnRequirementHelperTextChanged;
                requirementHelper.Run();                

                if (requirementHelper.Result)
                    ViewVisibility = Tags.ContentView;
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