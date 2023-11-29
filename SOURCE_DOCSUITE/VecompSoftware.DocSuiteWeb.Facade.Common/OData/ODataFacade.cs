using System;
using System.Linq;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.Commons;
using VecompSoftware.DocSuiteWeb.DTO.OData;
using VecompSoftware.DocSuiteWeb.Model.Parameters;
using VecompSoftware.DocSuiteWeb.Model.Securities;
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
        public const string ODATA_COLLABORATION_ENDPOINT_NAME = "Collaboration";
        public const string ODATA_DOMAINUSER_ENDPOINT_NAME = "DomainUser";
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
            string odataFilter = $"/DocumentUnitService.HasViewableDocument(idDocumentUnit={idDocumentUnit},username='{username.ToLower()}',domain='{domain}')";
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

            ODataModel<bool> result = WebAPIImpersonatorFacade.ImpersonateRawRequest<DocumentUnitEntities.DocumentUnit, ODataModel<bool>>(WebAPIHelper, odataFilter, customHttpConfiguration);

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

            ODataModel<bool> result = WebAPIImpersonatorFacade.ImpersonateRawRequest<Fascicle, ODataModel<bool>>(WebAPIHelper, odataFilter, customHttpConfiguration);

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

            ODataModel<bool> result = WebAPIImpersonatorFacade.ImpersonateRawRequest<Fascicle, ODataModel<bool>>(WebAPIHelper, odataFilter, customHttpConfiguration);
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

            ODataModel<bool> result = WebAPIImpersonatorFacade.ImpersonateRawRequest<Fascicle, ODataModel<bool>>(WebAPIHelper, odataFilter, customHttpConfiguration);
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

            ODataModel<bool> result = WebAPIImpersonatorFacade.ImpersonateRawRequest<Fascicle, ODataModel<bool>>(WebAPIHelper, odataFilter, customHttpConfiguration);
            return result.Value;
        }

        public bool HasCollaborationViewableRight(int idCollaboration)
        {
            string odataFilter = $"/CollaborationService.HasViewableRight(idCollaboration={idCollaboration})";
            IBaseAddress webApiAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.Single(x => x.AddressName.Eq(WebApiHttpClient.ODATA_ADDRESS_NAME));
            TenantEntityConfiguration collaborationEndpoint = DocSuiteContext.Current.CurrentTenant.Entities.Single(x => x.Key.Eq(ODATA_COLLABORATION_ENDPOINT_NAME)).Value;

            HttpClientConfiguration customHttpConfiguration = new HttpClientConfiguration();
            customHttpConfiguration.Addresses.Add(webApiAddress);
            WebApiControllerEndpoint endpoint = new WebApiControllerEndpoint
            {
                AddressName = webApiAddress.AddressName,
                ControllerName = collaborationEndpoint.ODATAControllerName,
                EndpointName = ODATA_COLLABORATION_ENDPOINT_NAME
            };
            customHttpConfiguration.EndPoints.Add(endpoint);

            ODataModel<bool> result = WebAPIImpersonatorFacade.ImpersonateRawRequest<Collaboration, ODataModel<bool>>(WebAPIHelper, odataFilter, customHttpConfiguration);
            return result.Value;
        }

        public DomainUserModel GetDomainUserModel()
        {
            string odataFilter = $"/DomainUserService.GetCurrentRights()";
            IBaseAddress webApiAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.Single(x => x.AddressName.Eq(WebApiHttpClient.ODATA_ADDRESS_NAME));
            TenantEntityConfiguration domainuserEndpoint = DocSuiteContext.Current.CurrentTenant.Entities.Single(x => x.Key.Eq(ODATA_DOMAINUSER_ENDPOINT_NAME)).Value;

            HttpClientConfiguration customHttpConfiguration = new HttpClientConfiguration();
            customHttpConfiguration.Addresses.Add(webApiAddress);
            WebApiControllerEndpoint endpoint = new WebApiControllerEndpoint
            {
                AddressName = webApiAddress.AddressName,
                ControllerName = domainuserEndpoint.ODATAControllerName,
                EndpointName = ODATA_DOMAINUSER_ENDPOINT_NAME
            };
            customHttpConfiguration.EndPoints.Add(endpoint);
            DomainUserModel result = WebAPIImpersonatorFacade.ImpersonateRawRequest<DomainUser, DomainUserModel>(WebAPIHelper, odataFilter, customHttpConfiguration);
            return result;
        }
        #endregion
    }
}
