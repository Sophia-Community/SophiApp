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
                            UIModelType.SquareCheckBox => BuildSquareCheckBox(dto),
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
                "Microsoft.DesktopAppInstaller", "Microsoft.StorePurchaseApp", "Microsoft.WindowsNotepad", "Microsoft.WindowsStore", "Microsoft.WindowsTerminal",
                "Microsoft.WindowsTerminalPreview", "Microsoft.WebMediaExtensions", "Microsoft.AV1VideoExtension", "Microsoft.HEVCVideoExtension",
                "Microsoft.RawImageExtension", "Microsoft.HEIFImageExtension", "windows.immersivecontrolpanel", "AdvancedMicroDevicesInc-2.AMDRadeonSoftware",
                "AppUp.IntelGraphicsControlPanel", "ELANMicroelectronicsCorpo.ELANTouchpadforThinkpad", "ELANMicroelectronicsCorpo.ELANTrackPointforThinkpa",
                "AppUp.IntelGraphicsExperience", "Microsoft.ApplicationCompatibilityEnhancements", "Microsoft.AVCEncoderVideoExtension", "Microsoft.DesktopAppInstaller",
                "Microsoft.StorePurchaseApp", "MicrosoftWindows.CrossDevice", "Microsoft.WindowsNotepad", "Microsoft.WindowsStore", "Microsoft.WindowsTerminal",
                "Microsoft.WindowsTerminalPreview", "Microsoft.WebMediaExtensions", "Microsoft.AV1VideoExtension", "MicrosoftCorporationII.WindowsSubsystemForLinux",
                "Microsoft.HEVCVideoExtensions", "Microsoft.RawImageExtension", "Microsoft.HEIFImageExtension", "Microsoft.MPEG2VideoExtension", "Microsoft.VP9VideoExtensions",
                "Microsoft.WebpImageExtension", "Microsoft.PowerShell", "NVIDIACorp.NVIDIAControlPanel", "RealtekSemiconductorCorp.RealtekAudioControl", "SynapticsIncorporated.SynapticsUtilities",
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
        public async Task<List<UIModel>> GetModelsContainsAsync(ConcurrentBag<UIModel> models, string text)
        {
            var timer = Stopwatch.StartNew();
            var foundModels = await Task.WhenAll(
                ContainsTextByTagAsync(models, text, UICategoryTag.Privacy),
                ContainsTextByTagAsync(models, text, UICategoryTag.Personalization),
                ContainsTextByTagAsync(models, text, UICategoryTag.System),
                ContainsTextByTagAsync(models, text, UICategoryTag.UWP),
                ContainsTextByTagAsync(models, text, UICategoryTag.Gaming),
                ContainsTextByTagAsync(models, text, UICategoryTag.TaskScheduler),
                ContainsTextByTagAsync(models, text, UICategoryTag.Security),
                ContainsTextByTagAsync(models, text, UICategoryTag.ContextMenu));
            timer.Stop();
            var result = foundModels.SelectMany(model => model).ToList();
            App.Logger.LogStopTextSearch(timer, result.Count);
            return result;
        }

        /// <inheritdoc/>
        public async Task GetStateAsync(ConcurrentBag<UIModel> models)
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
        public async Task GetStateAsync(IEnumerable<UIModel> enumerable, Action getStateCallback)
        {
            var models = new ConcurrentBag<UIModel>(enumerable);
            App.Logger.LogStartModelsGetState();
            var timer = Stopwatch.StartNew();
            await Task.WhenAll(
                GetStateByTagAsync(models, UICategoryTag.Privacy, getStateCallback),
                GetStateByTagAsync(models, UICategoryTag.Personalization, getStateCallback),
                GetStateByTagAsync(models, UICategoryTag.System, getStateCallback),
                GetStateByTagAsync(models, UICategoryTag.UWP, getStateCallback),
                GetStateByTagAsync(models, UICategoryTag.Gaming, getStateCallback),
                GetStateByTagAsync(models, UICategoryTag.TaskScheduler, getStateCallback),
                GetStateByTagAsync(models, UICategoryTag.Security, getStateCallback),
                GetStateByTagAsync(models, UICategoryTag.ContextMenu, getStateCallback));
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
                SetStateByTagAsync(models, UICategoryTag.Privacy, setStateCallback, token),
                SetStateByTagAsync(models, UICategoryTag.Personalization, setStateCallback, token),
                SetStateByTagAsync(models, UICategoryTag.System, setStateCallback, token),
                SetStateByTagAsync(models, UICategoryTag.UWP, setStateCallback, token),
                SetStateByTagAsync(models, UICategoryTag.Gaming, setStateCallback, token),
                SetStateByTagAsync(models, UICategoryTag.TaskScheduler, setStateCallback, token),
                SetStateByTagAsync(models, UICategoryTag.Security, setStateCallback, token),
                SetStateByTagAsync(models, UICategoryTag.ContextMenu, setStateCallback, token));
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
            var groupModel = new UIExpandingRadioGroupModel(dto, title, description, accessor, mutator);

            groupModel.Items = Enumerable.Range(1, dto.NumberOfItems)
                .Select(id =>
                {
                    var itemTitle = GetTitle(dto.Name, id);
                    return new UIRadioGroupItemModel(itemTitle, dto.Name, id, groupModel.ViewId);
                })
                .ToList();

            return groupModel;
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

        private UIModel BuildSquareCheckBox(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var accessor = GetAccessor<bool>(dto.Name);
            var mutator = GetMutator<bool>(dto.Name);
            return new UISquareCheckBoxModel(dto, title, accessor, mutator);
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

        private async Task<IEnumerable<UIModel>> ContainsTextByTagAsync(ConcurrentBag<UIModel> models, string text, UICategoryTag tag)
            => await Task.Run(() => models.Where(model => model.Tag == tag && model.ContainsText(text)));

        private Task GetStateByTagAsync(ConcurrentBag<UIModel> models, UICategoryTag tag, Action? getStateCallback = null)
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

        private Task SetStateByTagAsync(ConcurrentBag<UIModel> models, UICategoryTag tag, Action? getStateCallback = null, CancellationToken? token = null)
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
