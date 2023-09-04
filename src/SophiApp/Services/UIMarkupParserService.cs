// <copyright file="UIMarkupParserService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Collections.Generic;
    using System.Text;
    using SophiApp.Contracts.Services;
    using SophiApp.Core.Helpers;
    using SophiApp.Helpers;
    using Windows.ApplicationModel.Resources.Core;

    /// <inheritdoc/>
    public class UIMarkupParserService : IUIMarkupParserService
    {
        /// <inheritdoc/>
        public async Task ParseAsync()
        {
            var markupResource = Encoding.UTF8.GetString(Properties.Resources.UIMarkup);
            var a = await Json.ToObjectAsync<IEnumerable<UIControlDto>>(markupResource);

            var stringResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
            var str = stringResourceMap.GetValue("Action_DiagnosticDataLevel/ChildTitle1").ValueAsString;
        }
    }
}
