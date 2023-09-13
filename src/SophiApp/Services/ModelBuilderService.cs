// <copyright file="ModelBuilderService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using CSharpFunctionalExtensions;
    using SophiApp.Contracts.Services;
    using SophiApp.Core.Helpers;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using Windows.ApplicationModel.Resources.Core;

    /// <inheritdoc/>
    public class ModelBuilderService : IModelBuilderService
    {
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");

        /// <inheritdoc/>
        public ObservableCollection<UIModel> BuildUIModels()
        {
            var markupJson = Encoding.UTF8.GetString(Properties.Resources.UIMarkup);
            var models = Task.Run(async () =>
            {
                return await Json.ToObjectAsync<IEnumerable<UIModelDto>>(markupJson)
                .ContinueWith(task => task.Result.Select(BuildUIModel));
            })
                .Result;

            return new ObservableCollection<UIModel>(models);
        }

        private UIModel BuildUIModel(UIModelDto dto)
        {
            return dto.Type switch
            {
                UIModelType.ExpandingGroup => BuildExpandingGroupModel(dto),
                UIModelType.RadioGroup => BuildRadioGroupModel(dto),
                UIModelType.CheckBox => BuildCheckBoxModel(dto),
                _ => throw new ArgumentOutOfRangeException(paramName: dto.Type.ToString(), message: "An invalid type is specified."),
            };
        }

        private UIModel BuildExpandingGroupModel(UIModelDto dto)
        {
            var title = GetModelTitle(dto.Name);
            var description = GetModelDescription(dto.Name);
            var items = Enumerable.Range(1, dto.NumberOfItems)
                .Select(i =>
                {
                    var title = GetModelItemTitle(dto.Name, i);
                    return new UIItemModel(title);
                })
                .ToList();

            return new UIExpandingGroupModel(dto, title, description, items);
        }

        private UIModel BuildRadioGroupModel(UIModelDto dto)
        {
            var title = GetModelTitle(dto.Name);
            var items = Enumerable.Range(1, dto.NumberOfItems)
                .Select(i =>
                {
                    var title = GetModelItemTitle(dto.Name, i);
                    return new UIItemModel(title);
                })
                .ToList();

            return new UIRadioGroupModel(dto, title, items);
        }

        private UIModel BuildCheckBoxModel(UIModelDto dto)
        {
            var title = GetModelTitle(dto.Name);
            return new UICheckBoxModel(dto, title);
        }

        private string GetModelTitle(string name)
        {
            return resourceMap.GetValue($"UIModel_{name}_Title").ValueAsString;
        }

        private string GetModelDescription(string name)
        {
            return resourceMap.GetValue($"UIModel_{name}_Description").ValueAsString;
        }

        private string GetModelItemTitle(string name, int id)
        {
            return resourceMap.GetValue($"UIModel_{name}_Title_{id}").ValueAsString;
        }
    }
}
