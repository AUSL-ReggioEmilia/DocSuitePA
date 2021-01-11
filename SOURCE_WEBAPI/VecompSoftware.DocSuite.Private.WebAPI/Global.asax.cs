using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using VecompSoftware.DocSuite.Private.WebAPI.AssemblyResolvers;
using VecompSoftware.DocSuite.WebAPI.Common.Configurations;

namespace VecompSoftware.DocSuite.Private.WebAPI
{
    public class WebApiApplication : HttpApplication
    {
        #region [ Fields ]

        internal static readonly string UDSAssemblyFullName = ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.UDS.Assembly.FullName"];
        internal static readonly string UDSAssemblyFileName = Path.Combine(HttpRuntime.BinDirectory, ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.UDS.FileName"]);
        internal const string ACCESS_CONTROL_ALLOW_ORIGIN = "Access-Control-Allow-Origin";
        internal const string ACCESS_CONTROL_ALLOW_CREDENTIALS = "Access-Control-Allow-Credentials";
        internal const string HTTP_METHOD_OPTIONS = "OPTIONS";
        internal const string HTTP_METHOD_GET = "GET";
        internal const string HTTP_METHOD_POST = "POST";
        internal const string HTTP_METHOD_PUT = "PUT";
        internal const string HTTP_REQUEST_PARAM_ORIGIN = "HTTP_ORIGIN";

        #endregion

        protected void Application_Start()
        {
            AppDomain myDomain = Thread.GetDomain();
            myDomain.AssemblyResolve += MyDomain_AssemblyResolve;

            GlobalConfiguration.Configuration.Services.Replace(typeof(IAssembliesResolver), new UDSAssemblyResolver());
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(GlobalConfiguration.Configuration.Routes);
            ValidationFactory.SetDefaultConfigurationValidatorFactory(new SystemConfigurationSource());
            JsonSerializerConfig.RegisterConfigure(GlobalConfiguration.Configuration);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.HttpMethod == HTTP_METHOD_OPTIONS)
            {
                string httpOrigin = Request.Params[HTTP_REQUEST_PARAM_ORIGIN];
                HttpContext.Current.Response.AddHeader(ACCESS_CONTROL_ALLOW_ORIGIN, httpOrigin);
                HttpContext.Current.Response.AddHeader(ACCESS_CONTROL_ALLOW_CREDENTIALS, "true");

                HttpContext.Current.Response.StatusCode = 200;
                HttpApplication httpApplication = sender as HttpApplication;
                httpApplication.CompleteRequest();
            }

            if (Request.Params.AllKeys.Any(f => !string.IsNullOrEmpty(f) && f.Equals(HTTP_REQUEST_PARAM_ORIGIN)) &&
                (Request.HttpMethod == HTTP_METHOD_GET || Request.HttpMethod == HTTP_METHOD_POST || Request.HttpMethod == HTTP_METHOD_PUT))
            {
                Response.Headers.Remove(ACCESS_CONTROL_ALLOW_ORIGIN);
                Response.Headers.Remove(ACCESS_CONTROL_ALLOW_CREDENTIALS);
                string httpOrigin = Request.Params[HTTP_REQUEST_PARAM_ORIGIN];
                if (WebApiConfiguration.AllowCrossOrigin ||
                    WebApiConfiguration.CrossOriginLists.Any(f => f.Equals(httpOrigin, StringComparison.InvariantCulture)))
                {
                    HttpContext.Current.Response.AddHeader(ACCESS_CONTROL_ALLOW_ORIGIN, httpOrigin);
                    HttpContext.Current.Response.AddHeader(ACCESS_CONTROL_ALLOW_CREDENTIALS, "true");
                }
            }
        }

        private Assembly MyDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AppDomain myDomain = Thread.GetDomain();
            Assembly udsExternalAssembly = myDomain.GetAssemblies().SingleOrDefault(f => f.FullName == UDSAssemblyFullName);
            if (args.RequestingAssembly == null && args.Name.Equals(UDSAssemblyFullName))
            {
                return udsExternalAssembly == null
                    ? File.Exists(UDSAssemblyFileName) ? Assembly.LoadFile(UDSAssemblyFileName) : null
                    : udsExternalAssembly;
            }
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(f => f.FullName.Equals(args.Name));
        }
    }
}
