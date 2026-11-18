// <copyright file="IStatusService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service that tells the user what the app is doing while it is busy.
    /// </summary>
    public interface IStatusService
    {
        /// <summary>
        /// Occurs when the text to show has changed. It can be raised from any thread.
        /// </summary>
        event Action<string>? Changed;

        /// <summary>
        /// Sets what the app is doing now, for example which setting is being applied.
        /// </summary>
        /// <param name="text">A localized text.</param>
        void SetOperation(string text);

        /// <summary>
        /// Sets the current step of the operation, for example downloading or installing.
        /// </summary>
        /// <param name="resourceKey">Localized string resource key of the step.</param>
        /// <param name="arguments">Values for the placeholders of the step text.</param>
        void SetStep(string resourceKey, params object[] arguments);
    }
}
