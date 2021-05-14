namespace SophiApp.Commons
{
    internal enum LogType
    {
        APP_STARTUP_DIR, // The directory from which the app is launched
        INIT_APP_LOCALIZATION, // Localization used at startup
        APP_LOCALIZATION_CHANGED, // Localization changed to another
        INIT_TEXTED_ELEMENT_MODELS, // Start init texted element model collection
        DONE_INIT_TEXTED_ELEMENT_MODELS, // Completing init texted element model collection
        TEXTED_ELEMENT_ID, // Texted element id
        TEXTED_ELEMENT_ID_HAS_ERROR, // Texted element id that caused the error
        TEXTED_ELEMENT_STATE, // Changing the state of an texted element
        TEXTED_ELEMENT_ERROR_TARGET, // Name of the method that caused the error in texted element
        TEXTED_ELEMENT_ERROR_MESSAGE, // Message of the method that caused the error in the texted element
        INIT_RADIO_BUTTONS_GROUP_MODELS, // Start init radio buttons group models collection
        RADIO_BUTTON_ID_HAS_ERROR, // Radio button element id that caused the error
        RADIO_BUTTON_ERROR_TARGET, // Name of the method that caused the error in texted element
        RADIO_BUTTON_ERROR_MESSAGE, // Message of the method that caused the error in the texted element
        INIT_RADIO_BUTTONS_GROUP_ID, // Start init the radio buttons group models collection
        RADIO_BUTTONS_GROUP_ID_HAS_ERROR, // Radio buttons group id that caused the error
        RADIO_BUTTONS_GROUP_ERROR_TARGET, // Name of the method that caused the error in radio buttons group
        RADIO_BUTTONS_GROUP_ERROR_MESSAGE, // Message of the method that caused the error in the radio buttons group
        DONE_INIT_RADIO_BUTTONS_GROUP_MODELS, // Completing init radio buttons group models collection

        INIT_EXPANDING_GROUP_MODELS, // Init expanding group models collection
        EXPANDING_GROUP_ID, // Expanding group id
        DONE_INIT_EXPANDING_GROUP_MODELS, // Done init expanding group models collection
        INIT_VIEW, // Init view tag
        VISIBLE_VIEW_CHANGED, // The tag is currently active view
        INIT_THEME, // Init app theme
        THEME_CHANGED, // Currently selected theme
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
        DEBUG_SAVE_OK, // Debug log saved successfully
        DEBUG_SAVE_HAS_ERROR, // An error occurred while saving the debug log
        INIT_APPLYING_SETTINGS, // Apply settings clicked
        TOTAL_SELECTED_ELEMENTS, // Total number of selected items
        DONE_APPLYING_SETTINGS, // Settings applied
        INIT_RESET_SETTINGS, // Reset settings clicked
        DONE_RESET_SETTINGS, // Settings reseted
        CURRENT_VERSION,
    }
}