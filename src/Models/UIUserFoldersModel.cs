// <copyright file="UIUserFoldersModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <summary>
    /// The locations of the user folders. A new location can be chosen for each folder.
    /// </summary>
    public class UIUserFoldersModel : UIModel
    {
        private readonly Func<UserFolder, string> accessor;
        private readonly Action<UserFolder, string> mutator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIUserFoldersModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="folders">The user folders.</param>
        /// <param name="accessor">Method that returns the location of a folder.</param>
        /// <param name="mutator">Method that changes the location of a folder.</param>
        public UIUserFoldersModel(UIModelDto dto, string title, string description, IReadOnlyList<UserFolderItem> folders, Func<UserFolder, string> accessor, Action<UserFolder, string> mutator)
            : base(dto, title)
        {
            Description = description;
            Folders = folders;
            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the user folders.
        /// </summary>
        public IReadOnlyList<UserFolderItem> Folders { get; }

        /// <summary>
        /// Gets a value indicating whether a new location is chosen for any folder.
        /// </summary>
        public override bool IsChanged => Folders.Any(folder => folder.IsChanged);

        /// <summary>
        /// Gets the chosen locations as a text for the log.
        /// </summary>
        public string ChosenLocations => string.Join(", ", Folders.Where(folder => folder.IsChanged).Select(folder => $"{folder.Folder}: {folder.Path}"));

        /// <inheritdoc/>
        public override string ChosenValue => ChosenLocations;

        /// <summary>
        /// Chooses a location the user typed. Quotes and spaces around it are dropped, and variables such as %USERPROFILE% are replaced by their values.
        /// A text that is not a full path to a folder is not chosen, and the reason is shown.
        /// </summary>
        /// <param name="folder">One of <see cref="Folders"/>.</param>
        /// <param name="text">What the user typed.</param>
        public void ChooseTyped(UserFolderItem folder, string text)
        {
            var path = Environment.ExpandEnvironmentVariables(text.Trim().Trim('"').Trim());

            // A full path starts with a drive or a network share; nothing but the drive may have a colon
            var isFullPath = path.Length >= 3 && Path.IsPathFullyQualified(path) && path.AsSpan(2).IndexOfAny("<>\"|?*:") < 0;

            try
            {
                path = isFullPath ? Path.GetFullPath(path) : string.Empty;
            }
            catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
            {
                path = string.Empty;
            }

            if (path.Length == 0)
            {
                ShowNotice("UIModel_UserShellFolderLocation_PathNotValid".GetLocalized());
                return;
            }

            Choose(folder, path, isTyped: true);
        }

        /// <summary>
        /// Chooses a new location for a folder. A location that cannot be used is not chosen, and the reason is shown.
        /// </summary>
        /// <param name="folder">One of <see cref="Folders"/>.</param>
        /// <param name="path">The new location.</param>
        /// <param name="isTyped">The location was typed, not chosen with a button.</param>
        public void Choose(UserFolderItem folder, string path, bool isTyped = false)
        {
            // The location the folder has in Windows can always be taken back
            if (UserFolderHelper.IsRoot(path) && !UserFolderHelper.IsSamePath(path, folder.CurrentPath))
            {
                ShowNotice("UIModel_UserShellFolderLocation_RootNotAllowed".GetLocalized());
                return;
            }

            // The screenshots can be saved to any folder, the desktop too: only the folders of the user profile cannot share a location
            if (folder.Folder != UserFolder.Screenshots
                && Folders.FirstOrDefault(other => other != folder && other.Folder != UserFolder.Screenshots && UserFolderHelper.IsSamePath(other.Path, path)) is UserFolderItem taken)
            {
                ShowNotice(string.Format("UIModel_UserShellFolderLocation_FolderIsTaken".GetLocalized(), taken.Title));
                return;
            }

            ShowNotice(string.Empty);
            folder.SetChosenLocation(path, isTyped);

            // The default place of the screenshots is in the pictures folder, wherever that one is going to be
            if (folder.Folder == UserFolder.Pictures && Folders.FirstOrDefault(other => other.Folder == UserFolder.Screenshots) is UserFolderItem screenshots)
            {
                screenshots.SetDefaultPath(Path.Combine(path, UserFolder.Screenshots.ToString()));
            }
        }

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                var notices = new List<string>();

                foreach (var folder in Folders)
                {
                    try
                    {
                        folder.SetLocation(accessor.Invoke(folder.Folder), UserFolderHelper.GetDefaultPath(folder.Folder));
                        folder.IsEnabled = true;
                    }
                    catch (CustomizationException ex)
                    {
                        // Only this folder cannot be changed: its location is shown, and the reason stands under the setting
                        folder.SetLocation(UserFolderHelper.GetPath(folder.Folder), UserFolderHelper.GetDefaultPath(folder.Folder));
                        folder.IsEnabled = false;
                        notices.Add(ex.GetText());
                        App.Logger.LogModelGetStateException(Name, ex);
                    }
                }

                App.Logger.LogModelState(Name, Folders.Count(folder => folder.CanBeReset));
                IsEnabled = true;
                ShowNotice(string.Join(" ", notices));
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelGetStateException(Name, ex);
            }
        }

        /// <inheritdoc/>
        public override void SetState()
        {
            var notices = new List<string>();
            var foldersWithFiles = new List<string>();

            foreach (var folder in Folders.Where(folder => folder.IsChanged))
            {
                var oldPath = folder.CurrentPath;

                try
                {
                    // The pictures folder is moved by now, so the default place of the screenshots is read anew
                    var newPath = folder.Folder == UserFolder.Screenshots && folder.DefaultIsChosen ? UserFolderHelper.GetDefaultPath(UserFolder.Screenshots) : folder.Path;
                    mutator.Invoke(folder.Folder, newPath);

                    // The files are not moved: the user is told where they are left
                    if (UserFolderHelper.HasFiles(oldPath))
                    {
                        foldersWithFiles.Add($"\"{oldPath}\"");
                    }
                }
                catch (CustomizationException ex)
                {
                    notices.Add(ex.GetText());
                    App.Logger.LogModelSetStateException(ex, Name, $"{folder.Folder}: {folder.Path}");
                }
                catch (Exception ex)
                {
                    notices.Add(ApplyFailedText);
                    App.Logger.LogModelSetStateException(ex, Name, $"{folder.Folder}: {folder.Path}");
                }
            }

            if (foldersWithFiles.Count == 1)
            {
                notices.Add(string.Format("UIModel_UserShellFolderLocation_FilesLeft".GetLocalized(), foldersWithFiles[0].Trim('"')));
            }
            else if (foldersWithFiles.Count > 1)
            {
                notices.Add(string.Format("UIModel_UserShellFolderLocation_FilesLeftInSeveral".GetLocalized(), string.Join(", ", foldersWithFiles)));
            }

            if (notices.Count > 0)
            {
                // The text is shown under the setting when the locations are read again
                SetApplyFailure(string.Join(" ", notices.Distinct()));
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text)
            => base.ContainsText(text)
            || FormattedText.ToPlainText(Description).Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || Folders.Any(folder => folder.Title.Contains(text, StringComparison.CurrentCultureIgnoreCase) || folder.Path.Contains(text, StringComparison.CurrentCultureIgnoreCase));
    }
}
