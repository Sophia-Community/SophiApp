namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        public enum SearchState
        {
            Running,
            Stopped
        }

        private const string AdvancedSettingsVisibilityPropertyName = "AdvancedSettingsVisibility";
        private const string AppSelectedThemePropertyName = "AppSelectedTheme";
        private const string AppThemesPropertyName = "AppThemes";
        private const string ConditionsHelperErrorPropertyName = "ConditionsHelperError";
        private const string CustomActionsCounterPropertyName = "CustomActionsCounter";
        private const string DebugModePropertyName = "DebugMode";
        private const string FoundTextedElementPropertyName = "FoundTextedElement";
        private const string HamburgerHitTestPropertyName = "HamburgerHitTest";
        private const string LoadingPanelVisibilityPropertyName = "LoadingPanelVisibility";
        private const string LocalizationPropertyName = "Localization";
        private const string SearchPropertyName = "Search";
        private const string UwpElementsAllUsersPropertyName = "UwpElementsAllUsers";
        private const string UwpElementsCurrentUserPropertyName = "UwpElementsCurrentUser";
        private const string UwpForAllUsersStatePropertyName = "UwpForAllUsersState";
        private const string ViewsHitTestPropertyName = "ViewsHitTest";
        private const string VisibleViewByTagPropertyName = "VisibleViewByTag";
        private const string WindowCloseHitTestPropertyName = "WindowCloseHitTest";
    }
}