using System.Linq;
using Windows.Management.Deployment;

namespace SophiApp.Helpers
{
    internal class UwpHelper
    {
        internal static bool PackageExist(string packageName) => new PackageManager().FindPackages()
                                                                                    .Select(package => package.Id.Name)
                                                                                    .Contains(packageName);
    }
}