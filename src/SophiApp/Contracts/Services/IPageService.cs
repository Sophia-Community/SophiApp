// <copyright file="IPageService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

using Microsoft.UI.Xaml.Controls;

/// <summary>
/// A service for working with app <see cref="Page"/>.
/// </summary>
public interface IPageService
{
    /// <summary>
    /// Gets page type by key.
    /// </summary>
    /// <param name="key">Page key.</param>
    Type GetPageType(string key);
}
