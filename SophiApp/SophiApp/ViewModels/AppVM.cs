using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SophiApp.ViewModels
{
    internal class AppVM : INotifyPropertyChanged
    {
        private UILanguage appLocalization;
        private Logger logger = new Logger();
        private uint uielementsChangedCounter = default;
        private string viewVisibilityByTag = Tags.ViewPrivacy;
        private bool waitingPanelIsVisible = default;

        public AppVM()
        {
            UIElementClickedCommand = new RelayCommand(new Action<object>(UIElementClickedAsync));
            UIElementInListClickedCommand = new RelayCommand(new Action<object>(UIElementInListClickedAsync));
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClicked));

            AppLocalization = Localizator.Initializing();
            InitializingUIModelsAsync();
            SetUIModelsSystemStateAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public UILanguage AppLocalization
        {
            get => appLocalization;
            private set
            {
                appLocalization = value;
                logger.Collect(LogType.LOC, value.ToString());
                OnPropertyChanged("AppLocalization");
            }
        }

        public string AppName => Application.Current.FindResource("CONST.AppName") as string;
        public RelayCommand HamburgerClickedCommand { get; }

        public RelayCommand SearchClickedCommand { get; }

        public RelayCommand UIElementClickedCommand { get; }

        public RelayCommand UIElementInListClickedCommand { get; }

        public uint UIElementsChangedCounter
        {
            get => uielementsChangedCounter;
            private set
            {
                uielementsChangedCounter = value;
                OnPropertyChanged("UIElementsChangedCounter");
            }
        }

        public List<IUIElementModel> UIModels { get; set; }

        public string ViewVisibilityByTag
        {
            get => viewVisibilityByTag;
            private set
            {
                viewVisibilityByTag = value;
                OnPropertyChanged("ViewVisibilityByTag");
            }
        }

        public bool WaitingPanelIsVisible
        {
            get => waitingPanelIsVisible;
            private set
            {
                waitingPanelIsVisible = value;
                logger.Collect(LogType.WPNL, value.ToString());
                OnPropertyChanged("WaitingPanelIsVisible");
            }
        }

        private void HamburgerClicked(object args)
        {
            var tag = args as string;
            if (ViewVisibilityByTag == tag)
            {
                return;
            }
            logger.Collect(LogType.HMBRGCL, tag);
            ViewVisibilityByTag = tag;
        }

        private void InitializingUIModelsAsync()
        {
            logger.Collect(LogType.INTMDL);
            var task = Task.Run(() =>
            {
                var parsedJsons = Parser.ParseJson(Properties.Resources.UIElementsData);
                UIModels = parsedJsons.Select(dto => Fabric.CreateElementModel(dto)).ToList();
                UIModels.ForEach(model => model.SetLocalizationTo(AppLocalization));
            });
            task.Wait();
            logger.Collect(LogType.INTDON);
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void SearchClicked(object args)
        {
            var searchString = args as string;
            //TODO : Search and logger not implemented !!!
        }

        private void SetUIModelsSystemStateAsync()
        {
            logger.Collect(LogType.INTSTA);
            var task = Task.Run(() =>
            {
                UIModels.ForEach(m =>
                {
                    if (m.Id % 2 == 0) m.SetSystemState();
                });
            });
            task.Wait();
            logger.Collect(LogType.INTDON);
        }

        private void UIElementClickedAsync(object args)
        {
            var task = Task.Run(() =>
            {
                var id = Convert.ToInt32(args);
                var element = UIModels.Where(m => m.Id == id).First();
                element.SetUserState();
                //TODO: Get element state to logger !!!
                UIElementHasChangedAsync(element.UserState, id);
            });
            task.Wait();
        }

        private void UIElementHasChangedAsync(bool userState, int id)
        {
            var task = Task.Run(() =>
            {
                if (userState)
                {
                    UIElementsChangedCounter++;
                    return;
                }

                UIElementsChangedCounter--;
            });
            task.Wait();
        }

        private void UIElementInListClickedAsync(object args)
        {
            var task = Task.Run(() =>
            {
                var ids = args as List<int>;
                var listId = ids.First();
                var elementId = ids.Last();
                var list = UIModels.Where(m => m.Id == listId).First() as IItemsListModel;

                if (list.SelectOnce)
                {
                    UIModels.Where(m => list.ChildId.Contains(m.Id)).ToList().ForEach(m =>
                    {
                        if (m.Id == elementId && m.IsChecked == false)
                        {
                            m.SystemState = false;
                            m.UserState = true;
                            m.IsChecked = true;
                            //TODO: Get element state to logger !!!
                            UIElementHasChangedAsync(m.UserState, m.Id);
                        }
                        else if (m.Id != elementId)
                        {
                            m.SystemState = false;
                            m.UserState = false;
                            m.IsChecked = false;
                            //TODO: Get element state to logger !!!
                        }
                    });
                }
                else
                {
                    var element = UIModels.Where(m => m.Id == elementId).First();
                    element.SetUserState();
                    //TODO: Get element state to logger !!!
                    UIElementHasChangedAsync(element.UserState, element.Id);
                }
            });
            task.Wait();
        }
    }
}