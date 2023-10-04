// <copyright file="FileService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using System.Text;
using Newtonsoft.Json;
using SophiApp.Contracts.Services;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class FileService : IFileService
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    /// <param name="folderPath"><inheritdoc/></param>
    /// <param name="fileName"><inheritdoc/></param>
    public T? Read<T>(string folderPath, string fileName)
    {
        var path = Path.Combine(folderPath, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        return default;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    /// <param name="folderPath"><inheritdoc/></param>
    /// <param name="fileName"><inheritdoc/></param>
    /// <param name="content"><inheritdoc/></param>
    public void Save<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = JsonConvert.SerializeObject(content, Formatting.Indented);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="folderPath"><inheritdoc/></param>
    /// <param name="fileName"><inheritdoc/></param>
    public void Delete(string folderPath, string fileName)
    {
        if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }
}
