// <copyright file="FileService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;

using System.Runtime.CompilerServices;
using System.Text;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;

/// <inheritdoc/>
public class FileService : IFileService
{
    /// <inheritdoc/>
    public T? ReadFromJson<T>(string folderPath, string fileName)
    {
        var path = Path.Combine(folderPath, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return Json.ToObject<T>(json);
        }

        return default;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void SaveToJson<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = Json.Stringify(content!);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.Default);
    }

    /// <inheritdoc/>
    public void Save(string file, string content, Encoding encoding)
    {
        var foldersPath = Path.GetDirectoryName(file) ?? throw new ArgumentNullException(paramName: nameof(file), message: "The file path cannot be empty");

        if (!Directory.Exists(foldersPath))
        {
            Directory.CreateDirectory(foldersPath);
        }

        File.WriteAllText(file, content, encoding);
    }
}
