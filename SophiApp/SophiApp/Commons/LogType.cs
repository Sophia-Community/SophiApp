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
    }
}