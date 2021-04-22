namespace SophiApp.Commons
{
    internal enum LogType
    {
        INIT_APP_LOCALIZATION, // Init app localization
        INIT_TEXTED_ELEMENT_MODELS, // Init texted element model collection
        DONE_INIT_TEXTED_ELEMENT_MODELS, // Texted element model collection init done
        TEXTED_ELEMENT_ID, // Texted element id
        TEXTED_ELEMENT_STATE_CHANGED, // Texted element state changed
        TEXTED_ELEMENT_ERROR_TARGET, // The ID of the method that caused the error in the texted element
        TEXTED_ELEMENT_ERROR_MESSAGE // The message of the method that caused the error in the texted element
    }
}