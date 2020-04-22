using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using VecompSoftware.Clients.WebAPI.Configuration;
using VecompSoftware.Clients.WebAPI.Http;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.Helpers.Web.ExtensionMethods;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.DocSuite.DocumentHandler
{
    /// <summary>
    /// Summary description for ResolutionDocumentHandler
    /// </summary>
    public abstract class BaseDocumentHandler : IHttpHandler
    {
        #region [ Fields ]
        private WebApiHttpClient _httpClient = null;
        private static string _webAPIAddressesPath = string.Empty;
        private static string _webAPIEndpointsPath = string.Empty;
        private static string _cachePath = string.Empty;
        private static string _exceptionTemplatePath = string.Empty;
        private const string ODATA_ADDRESS_NAME = "ODATA-EntityAddress";
        private const string ODATA_RESOLUTION_CONTROLLER_NAME = "Resolutions";
        private const string ODATA_DOCUMENT_UNIT_CHAIN_CONTROLLER_NAME = "DocumentUnitChains";
        private const string EXCEPTION_TEMPLATE_PATH = "~/ExceptionTemplate/Error.pdf";
        private const string CACHE_PATH = "~/Cache";
        private const string ERROR_PAGE_NAME = "Error.pdf";
        private const string ADDRESSES_CONFIG_NAME = "~/Config/WebApi.Client.Config.Addresses.json";
        private const string RESPONSE_CONTENT_TYPE = "application/pdf";
        private const string RESPONSE_CONTENT_DISPOSITION = "Content-Disposition";
        private const string RESPONSE_CONTENT_LENGTH = "Content-Length";

        #endregion


        public BaseDocumentHandler()
        {
            try
            {
                FileLogger.Initialize();
            }
            catch
            {

            }
            _httpClient = new WebApiHttpClient(new WebAPIClientConfiguration(ADDRESSES_CONFIG_NAME).HttpConfiguration,
                new WebAPIClientConfiguration(ADDRESSES_CONFIG_NAME).HttpConfiguration, f => FileLogger.Debug(LogName.WebAPIClientLog, f));
        }

        #region [ Properties ]
        public static string CachePath
        {
            get
            {
                if (string.IsNullOrEmpty(_cachePath))
                {
                    _cachePath = HostingEnvironment.MapPath(CACHE_PATH);
                }
                return _cachePath;
            }
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


        public bool ViewLockedPdf { get; set; }

        private static string _pdfWatermark = null;
        public static string PdfWatermark
        {
            get
            {
                if (_pdfWatermark == null)
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["PdfWatermark"]))
                    {
                        _pdfWatermark = HttpUtility.HtmlDecode(ConfigurationManager.AppSettings["PdfWatermark"]);
                    }
                }
                return _pdfWatermark;
            }
        }
        private static bool? _mergeAllChainDocuments = null;
        public static bool MergeAllChainDocuments
        {
            get
            {
                if (!_mergeAllChainDocuments.HasValue)
                {
                    bool mergeAllChainDocuments = true;
                    if (!bool.TryParse(ConfigurationManager.AppSettings["MergeAllChainDocuments"], out mergeAllChainDocuments))
                    {
                        return true;
                    }
                    _mergeAllChainDocuments = mergeAllChainDocuments;
                }
                return _mergeAllChainDocuments.Value;
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
            Guid idResolution = context.Request.QueryString.GetValueOrDefault("UniqueId", Guid.Empty);
            byte[] stream = { };

            try
            {
                Resolution resolution = GetResolution(idResolution);
                if (!CheckValidity(context, resolution))
                {
                    return;
                }
                string cached_file_path = Path.Combine(CachePath, string.Concat(resolution.UniqueId.ToString(), ".pdf"));
                if (!File.Exists(cached_file_path))
                {
                    DocumentUnitChain documentChain = GetDocumentUnitChain(idResolution, ChainType.MainOmissisChain);
                    if (documentChain == null)
                    {
                        documentChain = GetDocumentUnitChain(idResolution, ChainType.MainChain);
                    }

                    if (documentChain == null)
                    {
                        ElaborateException(context);
                        return;
                    }

                    IList<BiblosDocumentInfo> documents = BiblosDocumentInfo.GetDocumentsLatestVersion(string.Empty, documentChain.IdArchiveChain);
                    if (documents == null || !documents.Any())
                    {
                        ElaborateException(context);
                        return;
                    }

                    IEnumerable<byte[]> streams = documents.Select(f => !ViewLockedPdf ? f.GetPdfStream() : Services.StampaConforme.Service.ConvertToSimplePdf(f.Stream, "pdf"));

                    if (MergeAllChainDocuments)
                    {
                        //recupero gli allegati
                        DocumentUnitChain attachmentsChain = GetDocumentUnitChain(idResolution, ChainType.AttachmentOmissisChain);
                        if (attachmentsChain == null)
                        {
                            attachmentsChain = GetDocumentUnitChain(idResolution, ChainType.AttachmentsChain);
                        }

                        if (attachmentsChain != null)
                        {
                            IList<BiblosDocumentInfo> attachmentDocuments = BiblosDocumentInfo.GetDocumentsLatestVersion(string.Empty, attachmentsChain.IdArchiveChain);
                            if (attachmentDocuments != null && attachmentDocuments.Any())
                            {
                                streams = streams.Concat(attachmentDocuments.Select(f => !ViewLockedPdf ? f.GetPdfStream() : Services.StampaConforme.Service.ConvertToSimplePdf(f.Stream, "pdf")));
                            }
                        }

                        //recupero il frontalino di pubblicazione
                        DocumentUnitChain frontespieceChain = GetDocumentUnitChain(idResolution, ChainType.FrontespizioChain);
                        if (frontespieceChain != null)
                        {
                            IList<BiblosDocumentInfo> frontespieceDocuments = BiblosDocumentInfo.GetDocumentsLatestVersion(string.Empty, frontespieceChain.IdArchiveChain);
                            if (frontespieceDocuments != null && frontespieceDocuments.Any())
                            {
                                streams = streams.Concat(frontespieceDocuments.Select(f => !ViewLockedPdf ? f.GetPdfStream() : Services.StampaConforme.Service.ConvertToSimplePdf(f.Stream, "pdf")));
                            }
                        }
                    }


                    stream = MergePDF(streams);
                    if (ViewLockedPdf)
                    {
                        using (MemoryDocumentInfo mdi = new MemoryDocumentInfo(stream, cached_file_path))
                        {
                            stream = mdi.GetPdfLocked(documents.First().Signature, PdfWatermark);
                        }
                    }

                    if (!Encoding.Default.GetString(stream, 0, 4).Equals("%PDF"))
                    {
                        throw new Exception("Il pdf non può essere visualizzato a causa di un errore di conversione.", new Exception("Lo stream non inizia con '%PDF'"));
                    }
                    File.WriteAllBytes(cached_file_path, stream);
                }
                else
                {
                    stream = File.ReadAllBytes(cached_file_path);
                }

                ElaborateStream(context, stream, string.Empty);
            }
            catch (Exception ex)
            {
                FileLogger.Error(LogName.BiblosServiceLog, ex.Message, ex);
                ElaborateException(context);
                return;
            }
        }

        public abstract bool CheckValidity(HttpContext context, Resolution resolution);

        protected void ElaborateStream(HttpContext context, byte[] stream, string name)
        {
            context.Response.Clear();
            context.Response.ContentType = RESPONSE_CONTENT_TYPE;
            context.Response.AddHeader(RESPONSE_CONTENT_DISPOSITION, string.Concat("inline", "; filename=", name));
            context.Response.AddHeader(RESPONSE_CONTENT_LENGTH, stream.Length.ToString());
            context.Response.BinaryWrite(stream);

            context.ApplicationInstance.CompleteRequest();
        }


        protected void SetEntityODATA<TEntity>(string controllerName) where TEntity : class
        {
            string entityName = typeof(TEntity).Name;
            IWebApiControllerEndpoint controller = _httpClient.Configuration.EndPoints.Single(f => f.EndpointName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            controller.AddressName = ODATA_ADDRESS_NAME;
            controller.ControllerName = controllerName;
        }

        protected Resolution GetResolution(Guid uniqueId)
        {
            try
            {
                SetEntityODATA<Resolution>(ODATA_RESOLUTION_CONTROLLER_NAME);
                ODataModel<Resolution> result = _httpClient.GetAsync<Resolution>().WithOData(string.Concat("$filter=UniqueId eq ", uniqueId)).ResponseToModel<ODataModel<Resolution>>();
                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected DocumentUnitChain GetDocumentUnitChain(Guid uniqueId, ChainType chainType)
        {
            try
            {
                SetEntityODATA<DocumentUnitChain>(ODATA_DOCUMENT_UNIT_CHAIN_CONTROLLER_NAME);
                ODataModel<DocumentUnitChain> result = _httpClient.GetAsync<DocumentUnitChain>()
                    .WithOData(string.Concat("$filter=DocumentUnit/UniqueId eq ", uniqueId, " and ChainType eq VecompSoftware.DocSuiteWeb.Entity.DocumentUnits.ChainType'", (int)chainType, "'"))
                    .ResponseToModel<ODataModel<DocumentUnitChain>>();
                return result == null || result.Value == null || !result.Value.Any() ? null : result.Value.Single();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void ElaborateException(HttpContext context)
        {
            byte[] stream = File.ReadAllBytes(ExceptionTemplatePath);
            ElaborateStream(context, stream.ToArray(), ERROR_PAGE_NAME);
        }

        public byte[] MergePDF(IEnumerable<byte[]> contents)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document doc = new Document())
                using (PdfSmartCopy copy = new PdfSmartCopy(doc, ms))
                {
                    doc.Open();
                    foreach (byte[] p in contents)
                    {
                        using (PdfReader reader = new PdfReader(p))
                        {
                            copy.AddDocument(reader);
                        }
                    }
                    doc.Close();
                }
                return ms.ToArray();
            }
        }
        #endregion
    }
}