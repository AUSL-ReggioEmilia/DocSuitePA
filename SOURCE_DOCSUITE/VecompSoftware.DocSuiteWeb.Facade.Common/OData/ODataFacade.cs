using System;
using System.Linq;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.OData;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Helpers.WebAPI;
using VecompSoftware.Services.Logging;
using DocumentUnitEntities = VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.OData
{
    public class ODataFacade
    {
        #region [ Fields ]
        public const string ODATA_DOCUMENTUNIT_ENDPOINT_NAME = "DocumentUnit";
        public const string ODATA_FASCICLE_ENDPOINT_NAME = "Fascicle";
        public const string ODATA_WORKFLOW_REPOSITORY_ENDPOINT_NAME = "WorkflowRepository";
        private IWebAPIHelper _webAPIHelper; 
        #endregion

        #region [ Properties ]
        protected IWebAPIHelper WebAPIHelper
        {
            get
            {
                if (_webAPIHelper == null)
                {
                    _webAPIHelper = new WebAPIHelper();
                }
                return _webAPIHelper;
            }
        }
        #endregion

        #region [ Constructor ]
        public ODataFacade()
        {

        }
        #endregion

        #region [ Methods ]
        public bool HasViewableRight(Guid idDocumentUnit, string username, string domain)
        {
            string odataFilter = string.Concat("/DocumentUnitService.HasViewableDocument(idDocumentUnit=", idDocumentUnit, ",username='", username.ToLower(), 
                                               "',domain='", domain, "')");
            IBaseAddress webApiAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.Single(x => x.AddressName.Eq(WebApiHttpClient.ODATA_ADDRESS_NAME));
            TenantEntityConfiguration documentUnitEndpoint = DocSuiteContext.Current.CurrentTenant.Entities.Single(x => x.Key.Eq(ODATA_DOCUMENTUNIT_ENDPOINT_NAME)).Value;

            HttpClientConfiguration customHttpConfiguration = new HttpClientConfiguration();
            customHttpConfiguration.Addresses.Add(webApiAddress);
            WebApiControllerEndpoint endpoint = new WebApiControllerEndpoint
            {
                AddressName = webApiAddress.AddressName,
                ControllerName = documentUnitEndpoint.ODATAControllerName,
                EndpointName = ODATA_DOCUMENTUNIT_ENDPOINT_NAME
            };
            customHttpConfiguration.EndPoints.Add(endpoint);

            ODataModel<bool> result = WebAPIHelper.GetRawRequest<DocumentUnitEntities.DocumentUnit, ODataModel<bool>>(customHttpConfiguration, customHttpConfiguration, odataFilter);

            if (result != null)
            {
                return result.Value;
            }
            else
            {
                FileLogger.Warn(LogName.FileLog, string.Concat("HasViewableRight -> Document unit con id ", idDocumentUnit, " non trovata."));
                return false;
            }            
        }

        public bool HasFascicleDocumentViewableRight(Guid idFascicle, string username, string domain)
        {
            string odataFilter = string.Concat("/FascicleService.HasViewableDocument(idFascicle=", idFascicle, ",username='", username.ToLower(), "',domain='", domain, "')");
            IBaseAddress webApiAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.Single(x => x.AddressName.Eq(WebApiHttpClient.ODATA_ADDRESS_NAME));
            TenantEntityConfiguration fascicleEndpoint = DocSuiteContext.Current.CurrentTenant.Entities.Single(x => x.Key.Eq(ODATA_FASCICLE_ENDPOINT_NAME)).Value;

            HttpClientConfiguration customHttpConfiguration = new HttpClientConfiguration();
            customHttpConfiguration.Addresses.Add(webApiAddress);
            WebApiControllerEndpoint endpoint = new WebApiControllerEndpoint
            {
                AddressName = webApiAddress.AddressName,
                ControllerName = fascicleEndpoint.ODATAControllerName,
                EndpointName = ODATA_FASCICLE_ENDPOINT_NAME
            };
            customHttpConfiguration.EndPoints.Add(endpoint);

            ODataModel<bool> result = WebAPIHelper.GetRawRequest<Fascicle, ODataModel<bool>>(customHttpConfiguration, customHttpConfiguration, odataFilter);

            return result.Value;
        }

        public bool HasFascicleViewableRight(Guid idFascicle)
        {
            string odataFilter = $"/FascicleService.HasViewableRight(idFascicle={idFascicle})";
            IBaseAddress webApiAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.Single(x => x.AddressName.Eq(WebApiHttpClient.ODATA_ADDRESS_NAME));
            TenantEntityConfiguration fascicleEndpoint = DocSuiteContext.Current.CurrentTenant.Entities.Single(x => x.Key.Eq(ODATA_FASCICLE_ENDPOINT_NAME)).Value;

            HttpClientConfiguration customHttpConfiguration = new HttpClientConfiguration();
            customHttpConfiguration.Addresses.Add(webApiAddress);
            WebApiControllerEndpoint endpoint = new WebApiControllerEndpoint
            {
                AddressName = webApiAddress.AddressName,
                ControllerName = fascicleEndpoint.ODATAControllerName,
                EndpointName = ODATA_FASCICLE_ENDPOINT_NAME
            };
            customHttpConfiguration.EndPoints.Add(endpoint);

            ODataModel<bool> result;
            System.Security.Principal.WindowsIdentity wi = (System.Security.Principal.WindowsIdentity)System.Web.HttpContext.Current.User.Identity;
            {
                using (System.Security.Principal.WindowsImpersonationContext wic = wi.Impersonate())
                {
                    using (System.Threading.ExecutionContext.SuppressFlow())
                    {
                        result = WebAPIHelper.GetRawRequest<Fascicle, ODataModel<bool>>(customHttpConfiguration, customHttpConfiguration, odataFilter);
                    }
                }
            }            

            return result.Value;
        }

        public bool HasFascicleInsertRight(FascicleType fascicleType)
        {
            string odataFilter = $"/FascicleService.HasInsertRight(fascicleType=VecompSoftware.DocSuiteWeb.Entity.Fascicles.FascicleType'{(int)fascicleType}')";
            IBaseAddress webApiAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.Single(x => x.AddressName.Eq(WebApiHttpClient.ODATA_ADDRESS_NAME));
            TenantEntityConfiguration fascicleEndpoint = DocSuiteContext.Current.CurrentTenant.Entities.Single(x => x.Key.Eq(ODATA_FASCICLE_ENDPOINT_NAME)).Value;

            HttpClientConfiguration customHttpConfiguration = new HttpClientConfiguration();
            customHttpConfiguration.Addresses.Add(webApiAddress);
            WebApiControllerEndpoint endpoint = new WebApiControllerEndpoint
            {
                AddressName = webApiAddress.AddressName,
                ControllerName = fascicleEndpoint.ODATAControllerName,
                EndpointName = ODATA_FASCICLE_ENDPOINT_NAME
            };
            customHttpConfiguration.EndPoints.Add(endpoint);

            ODataModel<bool> result;
            System.Security.Principal.WindowsIdentity wi = (System.Security.Principal.WindowsIdentity)System.Web.HttpContext.Current.User.Identity;
            {
                using (System.Security.Principal.WindowsImpersonationContext wic = wi.Impersonate())
                {
                    using (System.Threading.ExecutionContext.SuppressFlow())
                    {
                        result = WebAPIHelper.GetRawRequest<Fascicle, ODataModel<bool>>(customHttpConfiguration, customHttpConfiguration, odataFilter);
                    }
                }
            }

            return result.Value;
        }

        public bool HasProcedureDistributionInsertRight(int idCategory)
        {
            string odataFilter = $"/FascicleService.HasProcedureDistributionInsertRight(idCategory={idCategory})";
            IBaseAddress webApiAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.Single(x => x.AddressName.Eq(WebApiHttpClient.ODATA_ADDRESS_NAME));
            TenantEntityConfiguration fascicleEndpoint = DocSuiteContext.Current.CurrentTenant.Entities.Single(x => x.Key.Eq(ODATA_FASCICLE_ENDPOINT_NAME)).Value;

            HttpClientConfiguration customHttpConfiguration = new HttpClientConfiguration();
            customHttpConfiguration.Addresses.Add(webApiAddress);
            WebApiControllerEndpoint endpoint = new WebApiControllerEndpoint
            {
                AddressName = webApiAddress.AddressName,
                ControllerName = fascicleEndpoint.ODATAControllerName,
                EndpointName = ODATA_FASCICLE_ENDPOINT_NAME
            };
            customHttpConfiguration.EndPoints.Add(endpoint);

            ODataModel<bool> result;
            System.Security.Principal.WindowsIdentity wi = (System.Security.Principal.WindowsIdentity)System.Web.HttpContext.Current.User.Identity;
            {
                using (System.Security.Principal.WindowsImpersonationContext wic = wi.Impersonate())
                {
                    using (System.Threading.ExecutionContext.SuppressFlow())
                    {
                        result = WebAPIHelper.GetRawRequest<Fascicle, ODataModel<bool>>(customHttpConfiguration, customHttpConfiguration, odataFilter);
                    }
                }
            }

            return result.Value;
        }
        #endregion
    }
}
