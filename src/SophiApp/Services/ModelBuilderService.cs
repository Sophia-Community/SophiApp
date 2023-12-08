// <copyright file="ModelBuilderService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CSharpFunctionalExtensions;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using Windows.ApplicationModel.Resources.Core;

    /// <inheritdoc/>
    public class ModelBuilderService : IModelBuilderService
    {
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        private List<UIModel> models = new ();

        /// <inheritdoc/>
        public async Task BuildModelsAsync()
        {
            var json = Encoding.UTF8.GetString(Properties.Resources.UIMarkup);
            models = await JsonExtensions.ToObjectAsync<IEnumerable<UIModelDto>>(json)
                .ContinueWith(task => task.Result
                .Select(dto =>
                {
                    return dto.Type switch
                    {
                        UIModelType.CheckBox => BuildCheckBoxModel(dto),
                        UIModelType.ExpandingRadioGroup => BuildExpandingRadioGroupModel(dto),
                        UIModelType.ExpandingGroup => BuildExpandingGroupModel(dto),
                        UIModelType.RadioGroup => BuildRadioGroupModel(dto),
                        _ => throw new ArgumentOutOfRangeException(paramName: dto.Type.ToString(), message: "An invalid type is specified."),
                    };
                })
                .ToList());
        }

        /// <inheritdoc/>
        public List<UIModel> GetModels(UICategoryTag tag)
        {
            return models.Where(m => tag == m.Tag)
                .ToList();
        }

        private UIModel BuildCheckBoxModel(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            return new UICheckBoxModel(dto, title);
        }

        private UIModel BuildExpandingRadioGroupModel(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var items = Enumerable.Range(1, dto.NumberOfItems)
                .Select(i =>
                {
                    var itemTitle = GetTitle(dto.Name, i);
                    return new UIRadioGroupItemModel(Title: itemTitle, GroupName: dto.Name);
                }).ToList();

            return new UIExpandingRadioGroupModel(dto, title, items);
        }

        private UIModel BuildExpandingGroupModel(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var description = GetModelDescription(dto.Name);
            var items = Enumerable.Range(1, dto.NumberOfItems)
                .Select(i =>
                {
                    var title = GetTitle(dto.Name, i);
                    return new UIRadioGroupItemModel(Title: title, GroupName: dto.Name);
                })
                .ToList();

            return new UIExpandingGroupModel(dto, title, description, items);
        }

        private UIModel BuildRadioGroupModel(UIModelDto dto)
        {
            var title = GetTitle(dto.Name);
            var items = Enumerable.Range(1, dto.NumberOfItems)
                .Select(i =>
                {
                    var title = GetTitle(dto.Name, i);
                    return new UIRadioGroupItemModel(Title: title, GroupName: dto.Name);
                })
                .ToList();

            return new UIRadioGroupModel(dto, title, items);
        }

        private string GetTitle(string name, int? id = null)
        {
            var title = id is null ? $"UIModel_{name}_Title" : $"UIModel_{name}_Title_{id}";
            return resourceMap.GetValue(title).ValueAsString;
        }

        private string GetModelDescription(string name)
        {
            return resourceMap.GetValue($"UIModel_{name}_Description").ValueAsString;
        }
    }
}
