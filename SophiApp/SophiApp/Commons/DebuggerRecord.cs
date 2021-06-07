namespace SophiApp.Commons
{
    internal enum DebuggerRecord
    {
        STARTUP_DIR,
        VERSION,
        COMPUTER_NAME,
        OS_VERSION,
        USER_DOMAIN,
        USER_NAME,
        LOCALIZATION,
        THEME,
        VIEW,
        UPDATE_HAS_ERROR, // Checking for an update caused an error
        UPDATE_RESPONSE_NULL, // GitHub release server not responding
        UPDATE_RESPONSE_OK, // GitHub release server responds
        UPDATE_RESPONSE_LENGTH, // In bytes
        UPDATE_VERSION_FOUND, // Has new version release
        UPDATE_VERSION_IS_PRERELEASE, // Is prerelease version
        UPDATE_VERSION_IS_DRAFT, // Is draft release version
        UPDATE_VERSION_REQUIRED, // Need update
        UPDATE_VERSION_NOT_REQUIRED, // Not need update
        INIT_TEXTED_ELEMENT,
        ELEMENT_STATE,
        ELEMENT_HAS_ERROR,
        ELEMENT_ERROR_TARGET,
        ELEMENT_ERROR_MESSAGE,
        HYPERLINK_OPEN,
    }
}