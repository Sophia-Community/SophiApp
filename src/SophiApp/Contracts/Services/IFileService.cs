// <copyright file="IFileService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

/// <summary>
/// A file management service.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Reads the file.
    /// </summary>
    /// <typeparam name="T">The data type returned from the file.</typeparam>
    /// <param name="folderPath">The path to the file to be read.</param>
    /// <param name="fileName">File name.</param>
    T? Read<T>(string folderPath, string fileName);

    /// <summary>
    /// Save the data to a file.
    /// </summary>
    /// <typeparam name="T">The type of data saved to the file.</typeparam>
    /// <param name="folderPath">Path to the file to be saved.</param>
    /// <param name="fileName">File name.</param>
    /// <param name="content">Data to save to a file.</param>
    void Save<T>(string folderPath, string fileName, T content);

    /// <summary>
    /// Deletes the file.
    /// </summary>
    /// <param name="folderPath">Path to the file to be deleted.</param>
    /// <param name="fileName">File name.</param>
    void Delete(string folderPath, string fileName);
}
