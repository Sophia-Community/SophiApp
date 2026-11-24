// <copyright file="ModelService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Windows.ApplicationModel.Resources;
    using SophiApp.Contracts.Services;
    using SophiApp.Customizations;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <inheritdoc/>
    public class ModelService : IModelService
    {
        private readonly ICommonDataService dataService;
        private readonly ResourceMap resourceMap = new ResourceManager().MainResourceMap.GetSubtree("Resources");
        private readonly IAppxPackagesService packagesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelService"/> class.
        /// </summary>
        /// <param name="packagesService">A service for working with appx packages API.</param>
        /// <param name="dataService">A service for transferring app data between layers of DI.</param>
        public ModelService(IAppxPackagesService packagesService, ICommonDataService dataService)
        {
            this.packagesService = packagesService;
            this.dataService = dataService;
        }

        /// <inheritdoc/>
        public async Task<ObservableCollection<UIModel>> BuildSettingModelsAsync()
        {
            return new ObservableCollection<UIModel>(await Task.Run(() =>
            {
                // The settings are shown in the order of the table
                return SettingsCatalog.All
                    .Select((setting, index) => (Setting: setting, Index: index))
                    .Where(item => dataService.OsProperties.IsLTSC ? item.Setting.Windows11LTSC : item.Setting.Windows11)
                    .Select(item => BuildModel(item.Setting, item.Index))
                    .ToList();
            }));
        }

        /// <inheritdoc/>
        public async Task<List<UIModel>> BuildUwpAppModelsAsync(bool forAllUsers)
        {
            return await Task.Run(() => packagesService.GetRemovablePackages(forAllUsers)
                .Select((package, index) =>
                {
                    var dto = new UIModelDto(name: package.DisplayName, type: UIModelType.UwpApp, tag: UICategoryTag.UWP, viewId: 400 + index, windows11LTSC: true, windows11: true);
                    return (UIModel)new UIUwpAppModel(dto, package.Name, package.Logo) { PackageFullNames = package.FullNames };
                })
                .ToList());
        }

        /// <inheritdoc/>
        public async Task GetModelsState(ObservableCollection<UIModel> models)
        {
            App.Logger.LogStartModelsGetState();
            var timer = Stopwatch.StartNew();
            await Task.WhenAll(
                GetStateByTagAsync(models, UICategoryTag.Privacy),
                GetStateByTagAsync(models, UICategoryTag.Personalization),
                GetStateByTagAsync(models, UICategoryTag.System),
                GetStateByTagAsync(models, UICategoryTag.UWP),
                GetStateByTagAsync(models, UICategoryTag.Gaming),
                GetStateByTagAsync(models, UICategoryTag.TaskScheduler),
                GetStateByTagAsync(models, UICategoryTag.Security),
                GetStateByTagAsync(models, UICategoryTag.ContextMenu));
            timer.Stop();
            App.Logger.LogAllModelsGetState(timer, models.Count);
        }

        /// <inheritdoc/>
        public async Task GetModelsStateAsync(ObservableCollection<UIModel> models)
        {
            await Task.Run(() =>
            {
                foreach (var model in models)
                {
                    model.IsSelected = false;
                    var timer = Stopwatch.StartNew();
                    model.GetState();
                    timer.Stop();
                    App.Logger.LogModelGetState(model.Name, timer);
                }

                return Task.CompletedTask;
            });
        }

        /// <inheritdoc/>
        public async Task SetModelsStateAsync(ObservableCollection<UIModel> models)
        {
            await Task.Run(() =>
            {
                var number = 0;

                foreach (var model in models)
                {
                    // The user sees which setting is being applied and how many are left
                    number++;

                    // A package says what is done to it: "Installing OneDrive"
                    var package = model as UIInstallModel;
                    var title = package?.ActionTitle ?? model.Title;
                    var single = package is null ? string.Format("Panel_Status_ApplyingOne".GetLocalized(), title) : title;
                    App.Status.SetOperation(models.Count == 1
                        ? single
                        : string.Format("Panel_Status_Applying".GetLocalized(), number, models.Count, title));
                    model.IsSelected = false;
                    var timer = Stopwatch.StartNew();
                    model.SetState();
                    timer.Stop();
                    App.Logger.LogModelSetState(model.Name, timer);
                }
            });
        }

        /// <inheritdoc/>
        public async Task<ObservableCollection<UIModel>> GetModelsContainsTextAsync(ObservableCollection<UIModel> models, string text)
        {
            var timer = Stopwatch.StartNew();
            var found = await Task.Run(() => models.Where(model => model.ContainsText(text)));
            var foundModels = new ObservableCollection<UIModel>(found);
            timer.Stop();
            App.Logger.LogStopTextSearch(text, timer, foundModels.Count);
            return foundModels;
        }

        // The kind of a definition tells the types of its methods
        private UIModel BuildModel(SettingDefinition setting, int index)
        {
            var dto = new UIModelDto(setting.Name, setting.Type, setting.Tag, viewId: index, setting.Windows11LTSC, setting.Windows11, setting.IconIndex, setting.OptionIcons, setting.Group);
            var title = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title");
            var description = GetResourceValueOrDefault($"UIModel_{dto.Name}_Description");

            // A setting without a title has a mistake in its name or in the names of its strings
            if (title.Length == 0)
            {
                App.Logger.LogSettingWithoutTitle(dto.Name);
            }

            return setting.Type switch
            {
                UIModelType.CheckBox => new UICheckBoxModel(dto, title, description, (Func<bool>)setting.Accessor, (Action<bool>)setting.Mutator) { NoticeAccessor = setting.Notice },
                UIModelType.IconCheckBox => new UIIconCheckBoxModel(dto, title, description, (Func<bool>)setting.Accessor, (Action<bool>)setting.Mutator),
                UIModelType.Install or UIModelType.InstallUninstall => new UIInstallModel(dto, title, description, (Func<InstallState>)setting.Accessor, (Action<bool>)setting.Mutator)
                {
                    CanUninstall = setting.CanUninstall,
                    AllUsersMutator = setting.AllUsersMutator,
                },
                UIModelType.RadioButtonsGroup => new UIRadioButtonsGroupModel(dto, title, description, GetOptionTitles(dto.Name), (Func<int>)setting.Accessor, (Action<int>)setting.Mutator)
                {
                    HasOptionIcons = dto.OptionIcons,
                    ThirdPartyAccessor = setting.ThirdParty,
                    ThirdPartyTitle = GetResourceValueOrDefault($"UIModel_{dto.Name}_ThirdParty"),
                },
                UIModelType.IconRadioButtons => BuildIconRadioButtons(dto, title, description, setting),
                UIModelType.Action => new UIActionModel(dto, title, description, GetResourceValueOrDefault($"UIModel_{dto.Name}_Button"), (Func<bool>)setting.Accessor, (Action<bool>)setting.Mutator),
                UIModelType.Choice => new UIChoiceModel(dto, title, description, GetResourceValueOrDefault($"UIModel_{dto.Name}_NoChoice"), (Func<IReadOnlyList<SettingChoice>>)setting.Accessor, (Action<string>)setting.Mutator),
                UIModelType.UserFolders => BuildUserFoldersModel(dto, title, description, setting),
                UIModelType.Associations => new UIAssociationsModel(dto, title, description, (Func<string, string>)setting.Accessor, (Func<IReadOnlyList<AssociationRecord>, IReadOnlyList<string>>)setting.Mutator)
                {
                    ImportReader = setting.ImportReader,
                    Importer = setting.Importer,
                    Exporter = setting.Exporter,
                },
                UIModelType.CheckList => new UICheckListModel(dto, title, description, (Func<IReadOnlyList<SettingCheckItem>>)setting.Accessor, (Action<IReadOnlyDictionary<string, bool>>)setting.Mutator),
                _ => throw new TypeAccessException($"An invalid type is specified: {setting.Type}"),
            };
        }

        private UIModel BuildUserFoldersModel(UIModelDto dto, string title, string description, SettingDefinition setting)
        {
            // The icons are symbols of the icon font
            var folders = new (UserFolder Folder, string Glyph)[]
            {
                (UserFolder.Desktop, ""),
                (UserFolder.Documents, ""),
                (UserFolder.Downloads, ""),
                (UserFolder.Music, ""),
                (UserFolder.Pictures, ""),
                (UserFolder.Videos, ""),
                (UserFolder.Screenshots, ""),
            };
            var items = folders.Select(folder => new UserFolderItem(folder.Folder, GetResourceValueOrDefault($"UserFolder_{folder.Folder}"), folder.Glyph)).ToList();
            return new UIUserFoldersModel(dto, title, description, items, (Func<UserFolder, string>)setting.Accessor, (Action<UserFolder, string>)setting.Mutator);
        }

        private UIModel BuildIconRadioButtons(UIModelDto dto, string title, string description, SettingDefinition setting)
        {
            var options = GetOptionTitles(dto.Name);

            // An option may have a web page about it
            var links = Enumerable.Range(1, options.Count).Select(number => GetResourceValueOrDefault($"UIModel_{dto.Name}_Link_{number}")).ToList();
            return new UIIconRadioButtonsModel(dto, title, description, options, links, (Func<int>)setting.Accessor, (Action<int>)setting.Mutator)
            {
                ThirdPartyAccessor = setting.ThirdParty,
                ThirdPartyTitle = GetResourceValueOrDefault($"UIModel_{dto.Name}_ThirdParty"),
            };
        }

        // The options are numbered from 1, and there are as many of them as there are titles
        private List<string> GetOptionTitles(string name)
        {
            var options = new List<string>();

            while (GetResourceValueOrDefault($"UIModel_{name}_Title_{options.Count + 1}") is { Length: > 0 } option)
            {
                options.Add(option);
            }

            return options;
        }

        private string GetResourceValueOrDefault(string pattern) => resourceMap.TryGetValue(pattern)?.ValueAsString ?? string.Empty;

        private Task GetStateByTagAsync(ObservableCollection<UIModel> models, UICategoryTag tag)
        {
            return Task.Run(() =>
            {
                foreach (var model in models.Where(m => m.Tag == tag))
                {
                    var timer = Stopwatch.StartNew();
                    model.GetState();
                    timer.Stop();
                    App.Logger.LogModelGetState(model.Name, timer);
                }
            });
        }
    }
}
