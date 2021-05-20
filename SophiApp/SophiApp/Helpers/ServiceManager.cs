using System.ServiceProcess;

namespace SophiApp.Helpers
{
    internal class ServiceManager
    {
        internal static ServiceController GetService(string name) => new ServiceController(name);

        internal static ServiceControllerStatus GetState(string name) => new ServiceController(name).Status;
    }
}