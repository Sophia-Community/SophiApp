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
        /// <param name="displayText">Action localized description text.</param>
        public RequirementAction(Func<RequirementsResult> action, string? displayText = null)
        {
            Execute = action;
            DisplayText = displayText;
        }

        /// <summary>
        /// Gets requirement action execute.
        /// </summary>
        public Func<RequirementsResult> Execute { get; }

        /// <summary>
        /// Gets action localized description text.
        /// </summary>
        public string? DisplayText { get; }
    }
}
