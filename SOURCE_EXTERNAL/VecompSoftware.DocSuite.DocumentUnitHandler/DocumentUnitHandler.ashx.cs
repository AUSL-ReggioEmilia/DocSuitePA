using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using VecompSoftware.Clients.WebAPI.Configuration;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.Web.ExtensionMethods;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuite.DocumentUnitHandler
{
    /// <summary>
    /// Summary description for DocumentUnitHandler
    /// </summary>
    public class DocumentUnitHandler : IHttpHandler
    {
        #region [ Fields ]
        private WebApiHttpClient _httpClient = null;
        private static string _exceptionTemplatePath = string.Empty;
        private const string ERROR_PAGE_NAME = "Error.pdf";
        private const string EXCEPTION_TEMPLATE_PATH = "~/ExceptionTemplate/Error.pdf";
        private const string RESPONSE_CONTENT_TYPE = "application/pdf";
        private const string RESPONSE_CONTENT_DISPOSITION = "Content-Disposition";
        private const string RESPONSE_CONTENT_LENGTH = "Content-Length";
        private const string ADDRESSES_CONFIG_NAME = "~/Config/WebApi.Client.Config.Addresses.json";
        private const string ODATA_ADDRESS_NAME = "ODATA-EntityAddress";
        private const string ODATA_DOCUMENTUNITCHAIN_CONTROLLER_NAME = "DocumentUnitChains";
        #endregion

        #region [ Properties ]
        public DocumentUnitHandler()
        {
            _httpClient = new WebApiHttpClient(new WebAPIClientConfiguration(ADDRESSES_CONFIG_NAME).HttpConfiguration,
                new WebAPIClientConfiguration(ADDRESSES_CONFIG_NAME).HttpConfiguration, f => FileLogger.Debug(LogName.WebAPIClientLog, f));
        }


        public static string ExceptionTemplatePath
        {
            get
            {
                if (string.IsNullOrEmpty(_exceptionTemplatePath))
                {
                    _exceptionTemplatePath = HostingEnvironment.MapPath(EXCEPTION_TEMPLATE_PATH);
                }
                return _exceptionTemplatePath;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        #endregion

        #region [ Methods ]
        public void ProcessRequest(HttpContext context)
        {
            ElaborateDocument(context);
        }

        public void ElaborateDocument(HttpContext context)
        {
            Guid documentId = context.Request.QueryString.GetValueOrDefault("Id", Guid.Empty);
            byte[] stream = { };

            try
            {
                if (documentId == null)
                {
                    ElaborateException(context);
                }

                BiblosDocumentInfo documentInfo = BiblosDocumentInfo.GetDocumentByVersion(string.Empty, documentId, null, null);
                if (documentInfo == null)
                {
                    ElaborateException(context);
                }
                
                stream = documentInfo.GetPdfStream();
                if (!Encoding.Default.GetString(stream, 0, 4).Equals("%PDF"))
                {
                    throw new Exception("Il pdf non può essere visualizzato a causa di un errore di conversione.", new Exception("Lo stream non inizia con '%PDF'"));
                }
                DocumentUnit documentUnit = GetProtocolId(documentInfo.ChainId);

                if (documentUnit != null)
                {
                    InsertProtocolLog(documentUnit, documentInfo);
                    //throw new Exception(string.Concat("DocumentUnit con IdArchiveChain ", documentInfo.ChainId, " non trovata."), new Exception("DocumentUnit non trovata."));
                }

                ElaborateStream(context, stream, string.Empty);
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.BiblosServiceLog, ex.Message, ex);
                ElaborateException(context);
            }
        }

        protected void SetEntityODATA<TEntity>(string controllerName) where TEntity : class
        {
            string entityName = typeof(TEntity).Name;
            IWebApiControllerEndpoint controller = _httpClient.Configuration.EndPoints.Single(f => f.EndpointName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            controller.AddressName = ODATA_ADDRESS_NAME;
            controller.ControllerName = controllerName;
        }

        private void ElaborateStream(HttpContext context, byte[] stream, string name)
        {
            context.Response.Clear();
            context.Response.ContentType = RESPONSE_CONTENT_TYPE;
            context.Response.AddHeader(RESPONSE_CONTENT_DISPOSITION, string.Concat("inline", "; filename=", name));
            context.Response.AddHeader(RESPONSE_CONTENT_LENGTH, stream.Length.ToString());
            context.Response.BinaryWrite(stream);

            context.ApplicationInstance.CompleteRequest();
        }

        private void ElaborateException(HttpContext context)
        {
            byte[] stream = File.ReadAllBytes(ExceptionTemplatePath);
            ElaborateStream(context, stream.ToArray(), ERROR_PAGE_NAME);
        }


        protected DocumentUnit GetProtocolId(Guid idArchiveChain)
        {
            try
            {
                 SetEntityODATA<DocumentUnitChain>(ODATA_DOCUMENTUNITCHAIN_CONTROLLER_NAME);
                 ODataModel<DocumentUnitChain> result = _httpClient.GetAsync<DocumentUnitChain>().WithOData(string.Concat("$filter=IdArchiveChain eq ", idArchiveChain, " and DocumentUnit/Environment eq 1&$expand=DocumentUnit&$select=DocumentUnit")).ResponseToModel<ODataModel<DocumentUnitChain>>();

                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single().DocumentUnit;
            }

            catch (Exception ex)
            {
                FileLogger.Error(LogName.WebAPIClientLog, ex.Message, ex);
                throw ex;
            }
        }

        private void InsertProtocolLog(DocumentUnit entity, BiblosDocumentInfo documentInfo)
        {
            using (WindowsIdentity wi = (WindowsIdentity)HttpContext.Current.User.Identity)
            using (WindowsImpersonationContext wic = wi.Impersonate())
            using (ExecutionContext.SuppressFlow())
            {
                ProtocolLog protocolLog = new ProtocolLog()
                {
                    LogDescription = string.Format("\"{0}\" [{1}]", documentInfo.Name, documentInfo.DocumentId.ToString("N")),
                    Entity = new Protocol(entity.UniqueId)
                };

                try
                {
                    string actionType = EnumHelper.GetDescription(InsertActionType.ViewProtocolDocument);
                    //string actionType = "ViewProtocolDocument";
                    protocolLog = _httpClient.PostAsync(protocolLog, actionType).ResponseToModel<ProtocolLog>();
                }
                catch (Exception ex)
                {
                    FileLogger.Error(LogName.FileLog, ex.Message, ex);
                    throw ex;
                }
                finally
                {
                    wic.Undo();
                }
            }
        }
        #endregion

    }
}