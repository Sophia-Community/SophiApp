// <copyright file="ModelService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Windows.ApplicationModel.Resources;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
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
        public async Task<ObservableCollection<UIModel>> BuildJsonModelsAsync()
        {
            return new ObservableCollection<UIModel>(await Task.Run(() =>
            {
                var json = Encoding.UTF8.GetString(Properties.Resources.UIMarkup);
                var models = Json.ToObject<IEnumerable<UIModelDto>>(json)
                    .Where(dto => dataService.OsProperties.IsLTSC ? dto.Windows11LTSC : dto.Windows11)
                    .Select(dto =>
                    {
                        return dto.Type switch
                        {
                            UIModelType.CheckBox => BuildCheckBoxModel(dto),
                            UIModelType.IconCheckBox => BuildIconCheckBoxModel(dto),
                            UIModelType.Install => BuildInstallModel(dto, canUninstall: false),
                            UIModelType.InstallUninstall => BuildInstallModel(dto, canUninstall: true),
                            UIModelType.RadioButtonsGroup2 => BuildRadioButtonsGroup2(dto),
                            UIModelType.RadioButtonsGroup3 => BuildRadioButtonsGroup3(dto),
                            UIModelType.RadioButtonsGroup4 => BuildRadioButtonsGroup4(dto),
                            _ => throw new TypeAccessException($"An invalid type is specified: {dto.Type}"),
                        };
                    })
                    .OrderBy(model => model.ViewId);
                return models;
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
                    App.Status.SetOperation(models.Count == 1
                        ? string.Format("Panel_Status_ApplyingOne".GetLocalized(), model.Title)
                        : string.Format("Panel_Status_Applying".GetLocalized(), number, models.Count, model.Title));
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

        private UIModel BuildCheckBoxModel(UIModelDto dto)
        {
            var title = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title");
            var description = GetResourceValueOrDefault($"UIModel_{dto.Name}_Description");
            var accessor = GetAccessor<bool>(dto.Name);
            var mutator = GetMutator<bool>(dto.Name);

            // A setting can have a method that returns a notice for the current state of the system
            return new UICheckBoxModel(dto, title, description, accessor, mutator) { NoticeAccessor = GetNoticeAccessorOrDefault(dto.Name) };
        }

        private UIModel BuildIconCheckBoxModel(UIModelDto dto)
        {
            var title = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title");
            var description = GetResourceValueOrDefault($"UIModel_{dto.Name}_Description");
            var accessor = GetAccessor<bool>(dto.Name);
            var mutator = GetMutator<bool>(dto.Name);
            return new UIIconCheckBoxModel(dto, title, description, accessor, mutator);
        }

        private UIModel BuildInstallModel(UIModelDto dto, bool canUninstall)
        {
            var title = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title");
            var description = GetResourceValueOrDefault($"UIModel_{dto.Name}_Description");
            var accessor = GetAccessor<InstallState>(dto.Name);
            var mutator = GetMutator<bool>(dto.Name);

            // A package that can be installed for all users has a second method for that
            var allUsersMutator = GetMutatorOrDefault<bool>($"{dto.Name}_AllUsers");
            return new UIInstallModel(dto, title, description, accessor, mutator) { CanUninstall = canUninstall, AllUsersMutator = allUsersMutator };
        }

        private UIModel BuildRadioButtonsGroup2(UIModelDto dto)
        {
            var title = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title");
            var description = GetResourceValueOrDefault($"UIModel_{dto.Name}_Description");
            var title_1 = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title_1");
            var title_2 = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title_2");
            var accessor = GetAccessor<int>(dto.Name);
            var mutator = GetMutator<int>(dto.Name);
            return new UIRadioButtonsGroup2Model(dto, title, description, title_1, title_2, accessor, mutator);
        }

        private UIModel BuildRadioButtonsGroup3(UIModelDto dto)
        {
            var title = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title");
            var description = GetResourceValueOrDefault($"UIModel_{dto.Name}_Description");
            var title_1 = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title_1");
            var title_2 = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title_2");
            var title_3 = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title_3");
            var accessor = GetAccessor<int>(dto.Name);
            var mutator = GetMutator<int>(dto.Name);
            return new UIRadioButtonsGroup3Model(dto, title, description, title_1, title_2, title_3, accessor, mutator);
        }

        private UIModel BuildRadioButtonsGroup4(UIModelDto dto)
        {
            var title = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title");
            var description = GetResourceValueOrDefault($"UIModel_{dto.Name}_Description");
            var title_1 = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title_1");
            var title_2 = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title_2");
            var title_3 = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title_3");
            var title_4 = GetResourceValueOrDefault($"UIModel_{dto.Name}_Title_4");
            var accessor = GetAccessor<int>(dto.Name);
            var mutator = GetMutator<int>(dto.Name);
            return new UIRadioButtonsGroup4Model(dto, title, description, title_1, title_2, title_3, title_4, accessor, mutator);
        }

        private string GetResourceValueOrDefault(string pattern) => resourceMap.TryGetValue(pattern)?.ValueAsString ?? string.Empty;

        private Func<T> GetAccessor<T>(string name)
            where T : struct
        {
            var type = Type.GetType(typeName: "SophiApp.Customizations.Accessors", throwOnError: true)!;
            var method = type.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
            return (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), method!);
        }

        private Func<string?>? GetNoticeAccessorOrDefault(string name)
        {
            var type = Type.GetType(typeName: "SophiApp.Customizations.Accessors", throwOnError: true)!;
            var method = type.GetMethod($"{name}_Notice", BindingFlags.Static | BindingFlags.Public);
            return method is null ? null : (Func<string?>)Delegate.CreateDelegate(typeof(Func<string?>), method);
        }

        private Action<T>? GetMutatorOrDefault<T>(string name)
            where T : struct
        {
            var type = Type.GetType(typeName: "SophiApp.Customizations.Mutators", throwOnError: true)!;
            var method = type.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
            return method is null ? null : (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), method);
        }

        private Action<T> GetMutator<T>(string name)
            where T : struct
        {
            var type = Type.GetType(typeName: "SophiApp.Customizations.Mutators", throwOnError: true)!;
            var method = type.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
            return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), method!);
        }

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
