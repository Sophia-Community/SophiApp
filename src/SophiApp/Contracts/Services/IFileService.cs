// <copyright file="IFileService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

/// <summary>
/// A service for working with file API.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Reads and return deserialize objects from json file.
    /// </summary>
    /// <typeparam name="T">The data type returned from the file.</typeparam>
    /// <param name="folderPath">The path to the file to be read.</param>
    /// <param name="fileName">File name.</param>
    T? ReadFromJson<T>(string folderPath, string fileName);

    /// <summary>
    /// Serialize and save the data to json file.
    /// </summary>
    /// <typeparam name="T">The type of data saved to the file.</typeparam>
    /// <param name="folderPath">Path to the file to be saved.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="content">Data to save to a file.</param>
    void SaveToJson<T>(string folderPath, string fileName, T content);

    /// <summary>
    /// Save and create a path if it does not exist the data to a file.
    /// </summary>
    /// <param name="file">The file to write to.</param>
    /// <param name="content">The lines to write to the file.</param>
    void Save(string file, string content);
}
