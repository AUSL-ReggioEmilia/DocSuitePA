using Microsoft.Practices.Unity;
using System;
using System.Web.Hosting;
using System.Web.Http.Dependencies;
using VecompSoftware.Commons.Interfaces.ServiceLocator;
using VecompSoftware.DocSuite.Public.WebAPI.Scope;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;
using VecompSoftware.DocSuite.WebAPI.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Data;
using Security = VecompSoftware.DocSuiteWeb.Security;

namespace VecompSoftware.DocSuite.Public.WebAPI
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig : ILocator
    {
        #region Unity Container
        private static readonly Lazy<IDependencyResolver> container = new Lazy<IDependencyResolver>(() =>
        {
            UnityContainer container = new UnityContainer();
            return RegisterTypes(container);
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IDependencyResolver GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static IDependencyResolver RegisterTypes(IUnityContainer container)
        {
            IDependencyResolver resolver = container.Initialize<UnityConfig, WebAPICurrentIdentity>(string.Empty, string.Empty,
                WebApiConfiguration.ServiceBusConnectionString, HostingEnvironment.MapPath(WebApiConfiguration.MESSAGE_CONFIGURATION_FILE_PATH), 
                WebApiConfiguration.CustomInstanceName,
                WebApiConfiguration.AutoDeleteOnIdle, WebApiConfiguration.DefaultMessageTimeToLive, WebApiConfiguration.LockDuration,
                WebApiConfiguration.MaxDeliveryCount);

            IDataUnitOfWork unitOfWork = (IDataUnitOfWork)resolver.BeginScope().GetService(typeof(IDataUnitOfWork));
            Security.ISecurity security = (Security.ISecurity)resolver.BeginScope().GetService(typeof(Security.ISecurity));
            container.RegisterInstance(AutoMapperConfig.RegisterMappings(unitOfWork, security));
            return resolver;
        }

        public T GetService<T>()
        {
            return (T)GetConfiguredContainer().GetService(typeof(T));
        }
    }
}
