// <copyright file="ModelBuilderService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CSharpFunctionalExtensions;
    using Newtonsoft.Json;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using Windows.ApplicationModel.Resources.Core;

    /// <inheritdoc/>
    public class ModelBuilderService : IModelBuilderService
    {
        private readonly ResourceMap resourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
        private readonly List<UIModel> models;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBuilderService"/> class.
        /// </summary>
        public ModelBuilderService()
        {
            models = BuildModels();
        }

        /// <inheritdoc/>
        public List<UIModel> GetModels(UICategoryTag tag)
        {
            return models.Where(m => tag == m.Tag)
                .ToList();
        }

        private List<UIModel> BuildModels()
        {
            var json = Encoding.UTF8.GetString(Properties.Resources.UIMarkup);
            return JsonConvert.DeserializeObject<IEnumerable<UIModelDto>>(json)?
                .Select(dto =>
                {
                    return dto.Type switch
                    {
                        UIModelType.ExpandingGroup => BuildExpandingGroupModel(dto),
                        UIModelType.RadioGroup => BuildRadioGroupModel(dto),
                        UIModelType.CheckBox => BuildCheckBoxModel(dto),
                        _ => throw new ArgumentOutOfRangeException(paramName: dto.Type.ToString(), message: "An invalid type is specified."),
                    };
                })
                .ToList() ?? new List<UIModel>();
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
