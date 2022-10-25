using System;
using System.DirectoryServices.ActiveDirectory;

namespace SophiApp.Helpers
{
    internal class DomainHelper
    {
        private static bool hasDomain = PcHasDomain();
        public static bool PcInDomain { get => hasDomain; }

        private static bool PcHasDomain()
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