namespace SophiApp.ViewModel
{
    using System;
    using System.Reflection;

    /// <summary>
    /// View model for a <see cref="MainWindow"/>.
    /// </summary>
    public partial class MainVM
    {
        private readonly string name = Assembly.GetExecutingAssembly().GetName().Name!;
        private readonly Version version = Assembly.GetExecutingAssembly().GetName().Version!;

        /// <summary>
        /// Gets app name and version.
        /// </summary>
        public string FullName => $"{name} {version}";
    }
}