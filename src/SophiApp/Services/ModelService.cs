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
    using CSharpFunctionalExtensions;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelService"/> class.
        /// </summary>
        /// <param name="commonDataService">A service for transferring common app data between layers of abstractions.</param>
        public ModelService(ICommonDataService commonDataService) => this.commonDataService = commonDataService;

        /// <inheritdoc/>
        public List<UIModel> BuildModels()
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
                        UIModelType.ExpandingCheckBoxGroup => BuildExpandingCheckBoxGroupModel(dto),
                        _ => throw new TypeAccessException($"An invalid type is specified: {dto.Type}"),
                    };
                })
                .OrderByDescending(model => model.ViewId)
                .ToList();
            App.Logger.LogBuildModels(models.Count);
            return models;
        }

        /// <inheritdoc/>
        public async Task GetStateAsync(ConcurrentBag<UIModel> models)
        {
            App.Logger.LogStartAllModelGetState();
            var timer = Stopwatch.StartNew();
            await Task.WhenAll(
                GetStateByTag(models, UICategoryTag.Privacy),
                GetStateByTag(models, UICategoryTag.Personalization),
                GetStateByTag(models, UICategoryTag.System),
                GetStateByTag(models, UICategoryTag.UWP),
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
            App.Logger.LogStartAllModelGetState();
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
            var items = Enumerable.Range(1, dto.NumberOfItems)
                .Select(id =>
                {
                    var itemTitle = GetTitle(dto.Name, id);
                    return new UIRadioGroupItemModel(itemTitle, dto.Name, id);
                })
                .ToList();

            return new UIExpandingRadioGroupModel(dto, title, description, accessor, items);
        }

        private UIModel BuildExpandingCheckBoxGroupModel(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetDescription(dto.Name);
            var items = Enumerable.Range(1, dto.NumberOfItems)
                .Select(i =>
                {
                    var accessorName = $"{dto.Name}_{i}";
                    var accessor = GetAccessor<bool>(accessorName);
                    var itemTitle = GetTitle(dto.Name, i);
                    return new UICheckBoxGroupItemModel(accessor, itemTitle);
                })
                .ToList();

            return new UIExpandingCheckBoxGroupModel(dto, title, description, items);
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
                foreach (var model in models.Where(model => model.Tag == tag))
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
