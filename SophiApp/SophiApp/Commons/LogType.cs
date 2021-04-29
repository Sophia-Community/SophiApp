namespace SophiApp.Commons
{
    internal enum LogType
    {
        APP_STARTUP_DIR, // Directory from which the app is launched
        INIT_APP_LOCALIZATION, // Init app localization
        APP_LOCALIZATION_CHANGED, // App localization changed
        INIT_TEXTED_ELEMENT_MODELS, // Init texted element model collection
        DONE_INIT_TEXTED_ELEMENT_MODELS, // Texted element model collection init done
        TEXTED_ELEMENT_ID, // Texted element id
        TEXTED_ELEMENT_STATE_CHANGED, // Texted element state changed
        TEXTED_ELEMENT_ERROR_TARGET, // The ID of the method that caused the error in the texted element
        TEXTED_ELEMENT_ERROR_MESSAGE, // The message of the method that caused the error in the texted element
        INIT_CONTAINERS_MODELS, // Init containers model collection
        DONE_INIT_CONTAINERS_MODELS, // Texted containers collection init done
        INIT_VIEW, // Init view tag
        VISIBLE_VIEW_CHANGED, // The tag is currently active view
        INIT_THEME, // Init app theme
        THEME_CHANGED, // Currently selected theme
        VISIBLE_INFOPANEL, // Visible info panel: loading panel, applying panel, etc
        HIDE_INFOPANEL, // All info panel is hide
        HYPERLINK_OPEN, // Hyperlink is clicked
        UPDATE_HAS_ERROR, // Checking for an update caused an error
        UPDATE_RESPONSE_NULL, // GitHub release server not responding
        UPDATE_RESPONSE_OK, // GitHub release server responds
        UPDATE_RESPONSE_LENGTH, // In bytes
        UPDATE_VERSION_FOUND, // Has new version release
        UPDATE_VERSION_IS_PRERELEASE, // Is prerelease version
        UPDATE_VERSION_IS_DRAFT, // Is draft release version
        UPDATE_VERSION_REQUIRED, // Need update
        UPDATE_VERSION_NOT_REQUIRED, // Not need update
        UPDATE_AVAILABLE_CHANGED, // Is update property changed
        ADVANCED_SETTINGS_IS_VISIBLE, // Changing the visibility of advanced settings
    }
}