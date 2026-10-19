// <copyright file="ModelService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
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
    using Windows.ApplicationModel.Resources.Core;

    /// <inheritdoc/>
    public class ModelService : IModelService
    {
        private readonly ICommonDataService dataService;
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
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
            return await Task.Run(() =>
            {
                var models = new List<UIModel>();
                var viewId = 400;
                var packages = packagesService.GetPackages(forAllUsers);
                var excludedAppx = new List<string>()
            {
                // Dolby Access
                "DolbyLaboratories.DolbyAccess",
                "DolbyLaboratories.DolbyDigitalPlusDecoderOEM",

                // AMD Radeon Software
                "AdvancedMicroDevicesInc-2.AMDRadeonSoftware",

                // Intel Graphics Control Center
                "AppUp.IntelGraphicsControlPanel",
                "AppUp.IntelGraphicsExperience",

                // ELAN Touchpad
                "ELANMicroelectronicsCorpo.ELANTouchpadforThinkpad",
                "ELANMicroelectronicsCorpo.ELANTrackPointforThinkpa",

                // Microsoft Application Compatibility Enhancements
                "Microsoft.ApplicationCompatibilityEnhancements",

                // AVC Encoder Video Extension
                "Microsoft.AVCEncoderVideoExtension",

                // Microsoft Desktop App Installer
                "Microsoft.DesktopAppInstaller",

                // Store Experience Host
                "Microsoft.StorePurchaseApp",

                // Cross Device Experience Host
                "MicrosoftWindows.CrossDevice",

                // Notepad
                "Microsoft.WindowsNotepad",

                // Microsoft Store
                "Microsoft.WindowsStore",

                // Windows Terminal
                "Microsoft.WindowsTerminal",
                "Microsoft.WindowsTerminalPreview",

                // Web Media Extensions
                "Microsoft.WebMediaExtensions",

                // AV1 Video Extension
                "Microsoft.AV1VideoExtension",

                // Windows Subsystem for Linux
                "MicrosoftCorporationII.WindowsSubsystemForLinux",

                // HEVC Video Extensions from Device Manufacturer
                "Microsoft.HEVCVideoExtension",
                "Microsoft.HEVCVideoExtensions",

                // Raw Image Extension
                "Microsoft.RawImageExtension",

                // HEIF Image Extensions
                "Microsoft.HEIFImageExtension",

                // MPEG-2 Video Extension
                "Microsoft.MPEG2VideoExtension",

                // VP9 Video Extensions
                "Microsoft.VP9VideoExtensions",

                // Webp Image Extensions
                "Microsoft.WebpImageExtension",

                // PowerShell
                "Microsoft.PowerShell",

                // NVIDIA Control Panel
                "NVIDIACorp.NVIDIAControlPanel",

                // Realtek Audio Console
                "RealtekSemiconductorCorp.RealtekAudioControl",

                // Synaptics
                "SynapticsIncorporated.SynapticsControlPanel",
                "SynapticsIncorporated.241916F58D6E7",
                "ELANMicroelectronicsCorpo.ELANTrackPointforThinkpa",
                "ELANMicroelectronicsCorpo.TrackPoint",
            };
                for (int i = 0; i < packages.Count; i++)
                {
                    if (!excludedAppx.Contains(packages[i].Id.Name) && File.Exists(packages[i].Logo.LocalPath) && packages[i].DisplayName != string.Empty)
                    {
                        var dto = new UIModelDto(name: packages[i].DisplayName, type: UIModelType.UwpApp, tag: UICategoryTag.UWP, viewId: viewId + i, windows11LTSC: true, windows11: true);
                        models.Add(new UIUwpAppModel(dto, packages[i].Id.Name, packages[i].Logo));
                    }
                }

                return models;
            });
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
                foreach (var model in models)
                {
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
            var found = await Task.Run(() => models.Where(model => model.IsEnabled && model.ContainsText(text)));
            var foundModels = new ObservableCollection<UIModel>(found);
            timer.Stop();
            App.Logger.LogStopTextSearch(text, timer, foundModels.Count);
            return foundModels;
        }

        private UIModel BuildCheckBoxModel(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetDescription(dto.Name);
            var accessor = GetAccessor<bool>(dto.Name);
            var mutator = GetMutator<bool>(dto.Name);
            return new UICheckBoxModel(dto, title, description, accessor, mutator);
        }

        private UIModel BuildIconCheckBoxModel(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetDescription(dto.Name);
            var accessor = GetAccessor<bool>(dto.Name);
            var mutator = GetMutator<bool>(dto.Name);
            return new UIIconCheckBoxModel(dto, title, description, accessor, mutator);
        }

        private UIModel BuildRadioButtonsGroup2(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetDescription(dto.Name);
            var title_1 = GetTitle(dto.Name, 1);
            var title_2 = GetTitle(dto.Name, 2);
            var accessor = GetAccessor<int>(dto.Name);
            var mutator = GetMutator<int>(dto.Name);
            return new UIRadioButtonsGroup2Model(dto, title, description, title_1, title_2, accessor, mutator);
        }

        private UIModel BuildRadioButtonsGroup3(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetDescription(dto.Name);
            var title_1 = GetTitle(dto.Name, 1);
            var title_2 = GetTitle(dto.Name, 2);
            var title_3 = GetTitle(dto.Name, 3);
            var accessor = GetAccessor<int>(dto.Name);
            var mutator = GetMutator<int>(dto.Name);
            return new UIRadioButtonsGroup3Model(dto, title, description, title_1, title_2, title_3, accessor, mutator);
        }

        private UIModel BuildRadioButtonsGroup4(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetDescription(dto.Name);
            var title_1 = GetTitle(dto.Name, 1);
            var title_2 = GetTitle(dto.Name, 2);
            var title_3 = GetTitle(dto.Name, 3);
            var title_4 = GetTitle(dto.Name, 4);
            var accessor = GetAccessor<int>(dto.Name);
            var mutator = GetMutator<int>(dto.Name);
            return new UIRadioButtonsGroup4Model(dto, title, description, title_1, title_2, title_3, title_4, accessor, mutator);
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
