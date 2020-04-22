using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Vecompsoftware.FileServer.Services;

namespace VecompSoftware.JeepDashboard.Code
{
    static class ServicesHelper
    {
        /// <summary>
        /// Definisce se fisicamente il servizio esiste
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceFullPath"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool Exists(string serviceName, string serviceFullPath, IFileRepositoryService client)
        {
            if (client != null && !string.IsNullOrEmpty(serviceFullPath))
            {
                return client.ServiceStatus(serviceFullPath, serviceName).Exists;
            }
            var services = ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
            return File.Exists(serviceFullPath) && service != null;
        }

        /// <summary>
        /// Definisce se il servizio è installato [è necessario che esista]
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceFullPath"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static bool Installed(string serviceName, string serviceFullPath, IFileRepositoryService client)
        {
            if (!string.IsNullOrEmpty(serviceFullPath) && client != null)
            {
                return client.ServiceStatus(serviceFullPath, serviceName).Installed;
            }
            if (File.Exists(serviceFullPath))
            {
                var services = ServiceController.GetServices();
                var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
                return service != null;
            }
            return false;
        }

        /// <summary>
        /// Definisce lo status dello servizio
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceFullPath"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static ServiceControllerStatus? ServiceStatus(string serviceName, string serviceFullPath, IFileRepositoryService client)
        {
            if (!string.IsNullOrEmpty(serviceFullPath) && client != null)
            {
                return client.ServiceStatus(serviceFullPath, serviceName).Status;
            }
            if (File.Exists(serviceFullPath))
            {
                return new ServiceController(serviceName).Status;
            }
            return null;
        }

        /// <summary>
        /// Definisce la versione del servizio
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="serviceFullPath"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static string ServiceVersion(string serviceName, string serviceFullPath, IFileRepositoryService client)
        {
            if (!string.IsNullOrEmpty(serviceFullPath) && client != null)
            {
                return client.ServiceStatus(serviceFullPath, serviceName).Version;
            }
            if (File.Exists(serviceFullPath))
            {
                var serviceAssembly = Assembly.LoadFile(serviceFullPath);
                var serviceAssemblyFileVersioneInfo = FileVersionInfo.GetVersionInfo(serviceAssembly.Location);
                return serviceAssemblyFileVersioneInfo.FileVersion;
            }
            return string.Empty;
        }
    }
}
