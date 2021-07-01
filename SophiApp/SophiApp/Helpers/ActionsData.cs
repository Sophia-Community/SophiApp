namespace SophiApp.Helpers
{
    internal static class ActionsData
    {
        internal const string _100_DIAGTRACK_NAME = "DiagTrack";
        internal const string _265_COMMAND_NAME = "Command";
        internal const string _265_COMMAND_PATH = @"Msi.Package\shell\Extract\Command";
        internal const string _265_EXTRACT_PATH = @"Msi.Package\shell\Extract";
        internal const string _265_EXTRACT_VALUE = "msiexec.exe /a \"%1\" /qb TARGETDIR=\"%1 extracted\"";
        internal const string _265_ICON_NAME = "Icon";
        internal const string _265_ICON_VALUE = "shell32.dll,-16817";
        internal const string _265_MUIVERB_NAME = "MUIVerb";
        internal const string _265_MUIVERB_VALUE = "@shell32.dll,-37514";
        internal const string _267_EXTENDED_NAME = "Extended";
        internal const string _267_RUNASUSER_PATH = @"exefile\shell\runasuser";
        internal const string _268_CAST_TO_DEVICE_NAME = "{7AD84985-87B4-4a16-BE58-8B72A5B390F7}";
        internal const string _268_CAST_TO_DEVICE_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        internal const string _268_CAST_TO_DEVICE_VALUE = "Play to menu";
        internal const string _269_SHARE_CONTENT_NAME = "{E2BF9676-5F8F-435C-97EB-11607A5BEDF7}";
        internal const string _269_SHARE_CONTENT_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        internal const string _271_3D_EDIT_BMP_PATH = @"SystemFileAssociations\.bmp\Shell\3D Edit";
        internal const string _272_3D_EDIT_GIF_PATH = @"SystemFileAssociations\.gif\Shell\3D Edit";
        internal const string _273_3D_EDIT_JPE_PATH = @"SystemFileAssociations\.jpe\Shell\3D Edit";
        internal const string _274_3D_EDIT_JPEG_PATH = @"SystemFileAssociations\.jpeg\Shell\3D Edit";
        internal const string _275_3D_EDIT_JPG_PATH = @"SystemFileAssociations\.jpg\Shell\3D Edit";
        internal const string _276_3D_EDIT_PNG_PATH = @"SystemFileAssociations\.png\Shell\3D Edit";
        internal const string _277_3D_EDIT_TIF_PATH = @"SystemFileAssociations\.tif\Shell\3D Edit";
        internal const string _278_3D_EDIT_TIFF_PATH = @"SystemFileAssociations\.tiff\Shell\3D Edit";
        internal const string _279_SHELL_EDIT_PATH = @"AppX43hnxtbyyps62jhe9sqpdzxn1790zetc\Shell\ShellEdit";
        internal const string _279_UWP_PHOTOS_NAME = "Microsoft.Windows.Photos";
        internal const string _280_SHELL_CREATE_PATH = @"AppX43hnxtbyyps62jhe9sqpdzxn1790zetc\Shell\ShellCreateVideo";
        internal const string _281_SHELL_EDIT_PATH = @"SystemFileAssociations\image\shell\edit";
        internal const string _281_CAPABILITY_PAINT_NAME = @"Microsoft.Windows.MSPaint";
        internal const string _282_BAT_PRINT_PATH = @"batfile\shell\print";
        internal const string _282_CMD_PRINT_PATH = @"cmdfile\shell\print";
        internal const string _283_LIBRARY_LOCATION_PATH = @"Folder\ShellEx\ContextMenuHandlers\Library Location";
        internal const string _283_LIBRARY_LOCATION_VALUE = "{3dad6c5d-2167-4cae-9914-f99e41c12cfa}";
        internal static readonly string _283_LIBRARY_LOCATION_MINUS_VALUE = $"-{ActionsData._283_LIBRARY_LOCATION_VALUE}";
        internal const string _284_SEND_TO_PATH = @"AllFilesystemObjects\shellex\ContextMenuHandlers\SendTo";
        internal const string _284_SEND_TO_VALUE = "{7BA4C740-9E81-11CF-99D3-00AA004AE837}";
        internal static readonly string _284_SEND_TO_MINUS_VALUE = $"-{ActionsData._284_SEND_TO_VALUE}";
        internal const string CURRENT_VERSION = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";        
        internal const string PRODUCT_NAME = "ProductName";
        internal const string DISPLAY_VERSION_NAME = "DisplayVersion";
        internal const string REGISTRED_ORGANIZATION_NAME = "RegisteredOrganization";
        internal const string REGISTRED_OWNER_NAME = "RegisteredOwner";
        internal const string PROGRAMMATIC_ACCESS_ONLY_NAME = "ProgrammaticAccessOnly";
        internal const string UWP_3D_PAINT_NAME = "Microsoft.MSPaint";
        internal const string EXCEPTION_UWP_NOT_FOUND = "not found in uwp package list";
        internal const string EXCEPTION_CAPABILITY_NOT_FOUND = "not found in fod list";
    }
}