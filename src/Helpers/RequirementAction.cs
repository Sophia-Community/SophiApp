// <copyright file="RequirementAction.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using SophiApp.Contracts.Services;

    /// <summary>
    /// Implements the requirement action logic for <see cref="IRequirementsService"/>.
    /// </summary>
    public class RequirementAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementAction"/> class.
        /// </summary>
        /// <param name="action">Gets requirement action to execute.</param>
        /// <param name="displayKey">The resource key of the action description text. An action without it has no line on the startup page.</param>
        /// <param name="details">Returns what the action has found, to show under its text.</param>
        public RequirementAction(Func<RequirementsResult> action, string? displayKey = null, Func<IReadOnlyList<string>>? details = null)
        {
            Execute = action;
            DisplayKey = displayKey;
            Details = details;
        }

        /// <summary>
        /// Gets requirement action execute.
        /// </summary>
        public Func<RequirementsResult> Execute { get; }

        /// <summary>
        /// Gets the resource key of the action description text. The text is read by the page, so it follows the language chosen there.
        /// </summary>
        public string? DisplayKey { get; }

        /// <summary>
        /// Gets the method that returns what the action has found. Null if the action has nothing to show.
        /// </summary>
        public Func<IReadOnlyList<string>>? Details { get; }
    }
}
