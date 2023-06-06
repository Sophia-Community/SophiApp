// <copyright file="IPageService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);
}
