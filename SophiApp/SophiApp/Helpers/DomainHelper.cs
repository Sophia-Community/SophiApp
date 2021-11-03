using System;
using System.DirectoryServices.ActiveDirectory;

namespace SophiApp.Helpers
{
    internal class DomainHelper
    {
        internal static bool PcInDomain()
        {
            bool result;

            try
            {
                result = Domain.GetComputerDomain() != null;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
    }
}