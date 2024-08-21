// <copyright file="ModelService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.ViewModels;
    using Windows.ApplicationModel.Resources.Core;

    /// <inheritdoc/>
    public class ModelService : IModelService
    {
        private readonly ICommonDataService commonDataService;
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        private readonly IAppxPackagesService appxPackagesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelService"/> class.
        /// </summary>
        /// <param name="appxPackagesService">A service for working with appx packages API.</param>
        /// <param name="commonDataService">A service for transferring app data between layers of DI.</param>
        public ModelService(IAppxPackagesService appxPackagesService, ICommonDataService commonDataService)
        {
            this.appxPackagesService = appxPackagesService;
            this.commonDataService = commonDataService;
        }

        /// <inheritdoc/>
        public async Task<List<UIModel>> BuildJsonModelsAsync()
        {
            return await Task.Run(() =>
            {
                App.Logger.LogStartModelsBuild();
                var json = Encoding.UTF8.GetString(Properties.Resources.UIMarkup);
                var models = JsonExtensions.ToObject<IEnumerable<UIModelDto>>(json)
                    .Where(dto => commonDataService.IsWindows11 ? dto.Windows11Support : dto.Windows10Support)
                    .Select(dto =>
                    {
                        return dto.Type switch
                        {
                            UIModelType.CheckBox => BuildCheckBoxModel(dto),
                            UIModelType.ExpandingRadioGroup => BuildExpandingRadioGroupModel(dto),
                            UIModelType.ExpandingCheckBox => BuildExpandingCheckBox(dto),
                            _ => throw new TypeAccessException($"An invalid type is specified: {dto.Type}"),
                        };
                    })
                    .OrderByDescending(model => model.ViewId)
                    .ToList();
                App.Logger.LogAllModelsBuilt(models.Count);
                return models;
            });
        }

        /// <inheritdoc/>
        public async Task<List<UIModel>> BuildUwpAppModelsAsync(bool forAllUsers)
        {
            return await Task.Run(() =>
            {
                var models = new List<UIModel>();
                var initialViewId = 400;
                var packages = appxPackagesService.GetPackages(forAllUsers);
                var excludedAppx = new List<string>()
            {
                "Microsoft.DesktopAppInstaller", "Microsoft.StorePurchaseApp", "Microsoft.WindowsNotepad", "Microsoft.WindowsStore",
                "Microsoft.WindowsTerminal", "Microsoft.WindowsTerminalPreview", "Microsoft.WebMediaExtensions", "Microsoft.AV1VideoExtension",
                "Microsoft.HEVCVideoExtension", "Microsoft.RawImageExtension", "Microsoft.HEIFImageExtension", "windows.immersivecontrolpanel",
                "AdvancedMicroDevicesInc-2.AMDRadeonSoftware", "AppUp.IntelGraphicsControlPanel", "AppUp.IntelGraphicsExperience", "Microsoft.ApplicationCompatibilityEnhancements",
                "Microsoft.AVCEncoderVideoExtension", "Microsoft.DesktopAppInstaller", "Microsoft.StorePurchaseApp", "MicrosoftWindows.CrossDevice",
                "Microsoft.WindowsNotepad", "Microsoft.WindowsStore", "Microsoft.WindowsTerminal", "Microsoft.WindowsTerminalPreview",
                "Microsoft.WebMediaExtensions", "Microsoft.AV1VideoExtension", "MicrosoftCorporationII.WindowsSubsystemForLinux", "Microsoft.HEVCVideoExtensions",
                "Microsoft.RawImageExtension", "Microsoft.HEIFImageExtension", "Microsoft.MPEG2VideoExtension", "Microsoft.VP9VideoExtensions",
                "Microsoft.WebpImageExtension", "Microsoft.PowerShell", "NVIDIACorp.NVIDIAControlPanel", "RealtekSemiconductorCorp.RealtekAudioControl",
            };

                for (int i = 0; i < packages.Count; i++)
                {
                    try
                    {
                        if (!excludedAppx.Contains(packages[i].Id.Name) && File.Exists(packages[i].Logo.LocalPath) && packages[i].DisplayName != string.Empty)
                        {
                            var dto = new UIModelDto(Name: packages[i].DisplayName, Type: UIModelType.UwpApp, Tag: UICategoryTag.UWP, ViewId: initialViewId + i, Windows10Support: true, Windows11Support: true, NumberOfItems: 0);
                            models.Add(new UIUwpAppModel(dto, packages[i].Id.Name, packages[i].Logo));
                        }
                    }
                    catch (Exception)
                    {
                        // Do nothing.
                    }
                }

                return models;
            });
        }

        /// <inheritdoc/>
        public async Task GetStateAsync(ConcurrentBag<UIModel> models)
        {
            App.Logger.LogStartModelsGetState();
            var timer = Stopwatch.StartNew();
            await Task.WhenAll(
                GetStateByTag(models, UICategoryTag.Privacy),
                GetStateByTag(models, UICategoryTag.Personalization),
                GetStateByTag(models, UICategoryTag.System),
                GetStateByTag(models, UICategoryTag.TaskScheduler),
                GetStateByTag(models, UICategoryTag.Security),
                GetStateByTag(models, UICategoryTag.ContextMenu));
            timer.Stop();
            App.Logger.LogAllModelsGetState(timer, models.Count);
        }

        /// <inheritdoc/>
        public async Task GetStateAsync(IEnumerable<UIModel> enumerable, Action getStateCallback)
        {
            var models = new ConcurrentBag<UIModel>(enumerable);
            App.Logger.LogStartModelsGetState();
            var timer = Stopwatch.StartNew();
            await Task.WhenAll(
                GetStateByTag(models, UICategoryTag.Privacy, getStateCallback),
                GetStateByTag(models, UICategoryTag.Personalization, getStateCallback),
                GetStateByTag(models, UICategoryTag.System, getStateCallback),
                GetStateByTag(models, UICategoryTag.UWP, getStateCallback),
                GetStateByTag(models, UICategoryTag.TaskScheduler, getStateCallback),
                GetStateByTag(models, UICategoryTag.Security, getStateCallback),
                GetStateByTag(models, UICategoryTag.ContextMenu, getStateCallback));
            timer.Stop();
            App.Logger.LogAllModelsGetState(timer, models.Count);
        }

        /// <inheritdoc/>
        public async Task SetStateAsync(IEnumerable<UIModel> enumerable, Action setStateCallback, CancellationToken token)
        {
            var models = new ConcurrentBag<UIModel>(enumerable);
            App.Logger.LogStartApplicableModelsSetState();
            var timer = Stopwatch.StartNew();
            await Task.WhenAll(
                SetStateByTag(models, UICategoryTag.Privacy, setStateCallback, token),
                SetStateByTag(models, UICategoryTag.Personalization, setStateCallback, token),
                SetStateByTag(models, UICategoryTag.System, setStateCallback, token),
                SetStateByTag(models, UICategoryTag.UWP, setStateCallback, token),
                SetStateByTag(models, UICategoryTag.TaskScheduler, setStateCallback, token),
                SetStateByTag(models, UICategoryTag.Security, setStateCallback, token),
                SetStateByTag(models, UICategoryTag.ContextMenu, setStateCallback, token));
            timer.Stop();

            if (!token.IsCancellationRequested)
            {
                App.Logger.LogAllModelsSetState(timer, models.Count);
            }
        }

        private UIModel BuildCheckBoxModel(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetDescription(dto.Name);
            var accessor = GetAccessor<bool>(dto.Name);
            var mutator = GetMutator<bool>(dto.Name);
            return new UICheckBoxModel(dto, title, description, accessor, mutator);
        }

        private UIModel BuildExpandingRadioGroupModel(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetDescription(dto.Name);
            var accessor = GetAccessor<int>(dto.Name);
            var mutator = GetMutator<int>(dto.Name);
            var shellViewModel = App.GetService<ShellViewModel>();
            var model = new UIExpandingRadioGroupModel(dto, title, description, accessor, mutator);
            model.Items = Enumerable.Range(1, dto.NumberOfItems)
                .Select(id =>
                {
                    var itemTitle = GetTitle(dto.Name, id);
                    return new UIRadioGroupItemModel(itemTitle, dto.Name, id, shellViewModel, model);
                })
                .ToList();
            return model;
        }

        private UIModel BuildExpandingCheckBox(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetDescription(dto.Name);
            var imageSource = GetImageSource(dto.Name);
            var accessor = GetAccessor<bool>(dto.Name);
            var mutator = GetMutator<bool>(dto.Name);
            return new UIExpandingCheckBoxModel(dto, title, description, imageSource, accessor, mutator);
        }

        private string GetTitle(string name, int? id = null)
        {
            var title = id is null ? $"UIModel_{name}_Title" : $"UIModel_{name}_Title_{id}";
            return resourceMap.GetValue(title).ValueAsString;
        }

        private string GetDescription(string name)
        {
            if (resourceMap.TryGetValue($"UIModel_{name}_Description", out var value))
            {
                return value.Resolve().ValueAsString;
            }

            return string.Empty;
        }

        private string GetImageSource(string name)
        {
            return name switch
            {
                var source when source == "CleanupTask" => "/Assets/Windows.svg",
                _ => "/Assets/Folder.svg"
            };
        }

        private Func<T> GetAccessor<T>(string name)
            where T : struct
        {
            var type = Type.GetType(typeName: "SophiApp.Customizations.Accessors", throwOnError: true) !;
            var method = type.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
            return (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), method!);
        }

        private Action<T> GetMutator<T>(string name)
            where T : struct
        {
            var type = Type.GetType(typeName: "SophiApp.Customizations.Mutators", throwOnError: true) !;
            var method = type.GetMethod(name, BindingFlags.Static | BindingFlags.Public);
            return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), method!);
        }

        private Task GetStateByTag(ConcurrentBag<UIModel> models, UICategoryTag tag, Action? getStateCallback = null)
        {
            return Task.Run(() => models.Where(model => model.Tag == tag)
                .ToList()
                .ForEach(model =>
                {
                    var timer = Stopwatch.StartNew();
                    model.GetState();
                    timer.Stop();
                    App.Logger.LogModelGetState(model.Name, timer);
                    getStateCallback?.Invoke();
                }));
        }

        private Task SetStateByTag(ConcurrentBag<UIModel> models, UICategoryTag tag, Action? getStateCallback = null, CancellationToken? token = null)
        {
            return Task.Run(() =>
            {
                var taggedModels = models.Where(model => model.IsEnabled && model.Tag == tag).ToList();

                foreach (var model in taggedModels)
                {
                    if (token?.IsCancellationRequested ?? false)
                    {
                        App.Logger.LogAllModelsSetStateCanceled();
                        break;
                    }

                    var timer = Stopwatch.StartNew();
                    model.SetState();
                    timer.Stop();
                    App.Logger.LogModelSetState(model.Name, timer);
                    getStateCallback?.Invoke();
                }
            });
        }
    }
}
