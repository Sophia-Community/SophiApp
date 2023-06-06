// <copyright file="IActivationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
