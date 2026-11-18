// <copyright file="StatusService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using System.Globalization;

    /// <inheritdoc/>
    public class StatusService : IStatusService
    {
        private string operation = string.Empty;

        /// <inheritdoc/>
        public event Action<string>? Changed;

        /// <inheritdoc/>
        public void SetOperation(string text)
        {
            operation = text;
            App.Logger.LogUserNotice(text);
            Changed?.Invoke(text);
        }

        /// <inheritdoc/>
        public void SetStep(string resourceKey, params object[] arguments)
        {
            var step = string.Format(CultureInfo.CurrentCulture, resourceKey.GetLocalized(), arguments);

            // A step is shown after the operation it belongs to
            var text = operation.Length > 0 ? $"{operation} — {step}" : step;

            // The download percent changes too often to be logged
            if (arguments.Length == 0)
            {
                App.Logger.LogUserNotice(text);
            }

            Changed?.Invoke(text);
        }
    }
}
