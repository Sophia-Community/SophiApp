// <copyright file="FileService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using System.Text;
using Newtonsoft.Json;
using SophiApp.Contracts.Services;

/// <inheritdoc/>
public class FileService : IFileService
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    /// <param name="folderPath"><inheritdoc/></param>
    /// <param name="fileName"><inheritdoc/></param>
    public T? ReadFromJson<T>(string folderPath, string fileName)
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
    public void SaveToJson<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = JsonConvert.SerializeObject(content, Formatting.Indented);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.Default);
    }

    /// <inheritdoc/>
    public void Save(string file, string content)
    {
        var foldersPath = Path.GetDirectoryName(file) ?? throw new ArgumentNullException(paramName: nameof(file), message: "The file path cannot be empty");

        if (!Directory.Exists(foldersPath))
        {
            Directory.CreateDirectory(foldersPath);
        }

        File.WriteAllText(file, content, Encoding.Default);
    }
}
