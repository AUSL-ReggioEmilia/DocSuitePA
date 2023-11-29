using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(VecompSoftware.DocSuite.Public.WebAPI.UnityWebApiActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(VecompSoftware.DocSuite.Public.WebAPI.UnityWebApiActivator), "Shutdown")]

namespace VecompSoftware.DocSuite.Public.WebAPI
{
    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET</summary>
    public static class UnityWebApiActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            try
            {
                IDependencyResolver resolver = UnityConfig.GetConfiguredContainer();
                GlobalConfiguration.Configuration.DependencyResolver = resolver;
            }
            catch (Exception ex)
            {
                if (ex is System.Reflection.ReflectionTypeLoadException)
                {
                    ReflectionTypeLoadException typeLoadException = ex as ReflectionTypeLoadException;
                    if (typeLoadException.LoaderExceptions != null)
                    {
                        throw new Exception(string.Join(", ", typeLoadException.LoaderExceptions.ToList()), ex);
                    }
                }
                throw ex;
            }
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}
