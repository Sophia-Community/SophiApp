using SophiApp.Dto;

namespace SophiApp.Models
{
    internal class UwpElement
    {
        public UwpElement(UwpElementDto dto)
        {
            DisplayName = dto.DisplayName;
            Logo = dto.Logo;
            Name = dto.Name;
            PackageFullName = dto.PackageFullName;
        }

        public string DisplayName { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string PackageFullName { get; set; }
    }
}