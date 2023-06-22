// <copyright file="Json.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Core.Helpers;
using Newtonsoft.Json;

public static class Json
{
    public static async Task<T> ToObjectAsync<T>(string value)
    {
        return await Task.Run<T>(() =>
        {
            return JsonConvert.DeserializeObject<T>(value);
        });
    }

    public static async Task<string> StringifyAsync(object value)
    {
        return await Task.Run(() =>
        {
            return JsonConvert.SerializeObject(value);
        });
    }
}
