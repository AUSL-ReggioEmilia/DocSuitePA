using BiblosDS.Library.Common.StampaConforme;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.EnterpriseLogging;
using VecompSoftware.StampaConforme.Interfaces.Common.Services;
using VecompSoftware.StampaConforme.Models.ConversionParameters;
using VecompSoftware.StampaConforme.Services.DOU;
using VecompSoftware.StampConforme.Models.SecureDocument;

namespace VecompSoftware.BiblosDSConv
{
    /// <summary>
    /// Summary description for BiblosDSConv
    /// </summary>
    [WebService(Namespace = "http://www.vecompsoftware.it/BiblosDSConv/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class BiblosDSConv : WebService
    {
        #region [ Fields ]
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(BiblosDSConv));
        private static ILogger _globalLogger = new GlobalLogger();
        private readonly ISecureDocumentService _secureDocumentService;
        private int? _idSecureDocumentService = null;
        private string _certificateThumbprint;
        private Lazy<PrintRedirected> _printRedirectedService = new Lazy<PrintRedirected>(() => new PrintRedirected());
        private Lazy<PdfLabeler> _pdfLabelerService = new Lazy<PdfLabeler>(() => new PdfLabeler());
        #endregion

        #region [ Properties ]
        public int? IdSecureDocumentService
        {
            get
            {
                if (!_idSecureDocumentService.HasValue)
                {
                    string idSecureDocumentService = ConfigurationManager.AppSettings.Get("IdSecureDocumentService");
                    if (!string.IsNullOrEmpty(idSecureDocumentService))
                    {
                        _idSecureDocumentService = int.Parse(idSecureDocumentService);
                    }
                }
                return _idSecureDocumentService;
            }
        }

        public string SecureDocumentServiceCertificateThumbprint
        {
            get
            {
                if (string.IsNullOrEmpty(_certificateThumbprint))
                {
                    _certificateThumbprint = ConfigurationManager.AppSettings.Get("SecureDocumentServiceCertificateThumbprint");
                }
                return _certificateThumbprint;
            }
        }

        private PrintRedirected PrintRedirectedService => _printRedirectedService.Value;

        private PdfLabeler PdfLabelerService => _pdfLabelerService.Value;
        #endregion

        #region [ Constructor ]
        public BiblosDSConv()
        {
            if (IdSecureDocumentService.HasValue)
            {
                _secureDocumentService = new SecureDocumentService(_globalLogger, IdSecureDocumentService.Value, SecureDocumentServiceCertificateThumbprint);
            }
        }
        #endregion

        #region [ Methods ]
        public enum stParameterOption
        {
            None = 0,
            AttachMode = 1,
        }

        public struct stParameter
        {
            public stParameterOption Name { get; set; }
            public string Value { get; set; }
        }

        public struct stDoc
        {
            public string FileExtension;
            public string Blob;
            public string XmlValues;
            public string WmfP7m;
            public string ReferenceId;
        }

        public struct stSecureParameter
        {
            public string User;
            public string Password;
            public int AllowPrinting;
            public int AllowModifyContents;
            public int AllowCopy;
            public int AllowModifyAnnotations;
            public int AllowFillIn;
            public int AllowScreenReaders;
            public int AllowAssembly;
            public int AllowDegradedPrinting;
        }

        [WebMethod]
        public stDoc ToRasterFormatRgWatermarked(stDoc Doc, string ext, string label, string watermark, string user, string password,
                                                             int AllowPrinting, int AllowModifyContents,
                                                             int AllowCopy, int AllowModifyAnnotations,
                                                             int AllowFillIn, int AllowScreenReaders,
                                                             int AllowAssembly, int AllowDegradedPrinting)
        {
            try
            {
                logger.DebugFormat("ToRasterForToRasterFormatRgWatermarkedmatRg - INIT - {0}", HttpContext.Current.Request.UserHostAddress);

                stDoc res = new stDoc() { FileExtension = "PDF" };
                byte[] documentContent = Convert.FromBase64String(Doc.Blob);
                byte[] pdfDocumentContent = PrintRedirectedService.ConvertToFormatLabeled(documentContent, Doc.FileExtension, "PDF", label);
                byte[] watermarkedDocumentContent = PrintRedirectedService.EtichettaWatermark(pdfDocumentContent, watermark);
                ConversionSecureParameter secureParameter = new ConversionSecureParameter
                {
                    User = user,
                    Password = password,
                    AllowPrinting = Convert.ToBoolean(AllowPrinting),
                    AllowModifyContents = Convert.ToBoolean(AllowModifyContents),
                    AllowCopy = Convert.ToBoolean(AllowCopy),
                    AllowModifyAnnotations = Convert.ToBoolean(AllowModifyAnnotations),
                    AllowFillIn = Convert.ToBoolean(AllowFillIn),
                    AllowScreenReaders = Convert.ToBoolean(AllowScreenReaders),
                    AllowAssembly = Convert.ToBoolean(AllowAssembly),
                    AllowDegradedPrinting = Convert.ToBoolean(AllowDegradedPrinting)
                };
                byte[] lockedDocumentContent = PdfLabelerService.RightPdf(watermarkedDocumentContent, secureParameter);
                res.Blob = Convert.ToBase64String(lockedDocumentContent);
                return res;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw;
            }
            finally
            {
                logger.DebugFormat("ToRasterForToRasterFormatRgWatermarkedmatRg - END -{0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public stDoc ToRasterFormatRg(stDoc Doc, string ext, string label, string user, string password,
                                                             int AllowPrinting, int AllowModifyContents,
                                                             int AllowCopy, int AllowModifyAnnotations,
                                                             int AllowFillIn, int AllowScreenReaders,
                                                             int AllowAssembly, int AllowDegradedPrinting)
        {
            try
            {
                logger.DebugFormat("ToRasterFormatRg - INIT - {0}", HttpContext.Current.Request.UserHostAddress);
                stDoc res = new stDoc() { FileExtension = ext };
                byte[] documentContent = Convert.FromBase64String(Doc.Blob);
                byte[] pdfDocumentContent = PrintRedirectedService.ConvertToFormatLabeled(documentContent, Doc.FileExtension, ext, label);
                ConversionSecureParameter secureParameter = new ConversionSecureParameter
                {
                    User = user,
                    Password = password,
                    AllowPrinting = Convert.ToBoolean(AllowPrinting),
                    AllowModifyContents = Convert.ToBoolean(AllowModifyContents),
                    AllowCopy = Convert.ToBoolean(AllowCopy),
                    AllowModifyAnnotations = Convert.ToBoolean(AllowModifyAnnotations),
                    AllowFillIn = Convert.ToBoolean(AllowFillIn),
                    AllowScreenReaders = Convert.ToBoolean(AllowScreenReaders),
                    AllowAssembly = Convert.ToBoolean(AllowAssembly),
                    AllowDegradedPrinting = Convert.ToBoolean(AllowDegradedPrinting)
                };
                byte[] lockedDocumentContent = PdfLabelerService.RightPdf(pdfDocumentContent, secureParameter);
                res.Blob = Convert.ToBase64String(lockedDocumentContent);
                return res;
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw;
            }
            finally
            {
                logger.DebugFormat("ToRasterFormatRg - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public stDoc ToRasterFormatWatermarked(stDoc Doc, string label, string watermark)
        {
            try
            {
                logger.DebugFormat("ToRasterFormatWatermarked - INIT - {0}, {1}, {2}", HttpContext.Current.Request.UserHostAddress, label, watermark);
                byte[] documentContent = Convert.FromBase64String(Doc.Blob);
                byte[] pdfDocumentContent = PrintRedirectedService.ConvertToFormatLabeled(documentContent, Doc.FileExtension, "PDF", label);
                byte[] watermarkedDocumentContent = PrintRedirectedService.EtichettaWatermark(pdfDocumentContent, watermark);
                return new stDoc
                {
                    Blob = Convert.ToBase64String(watermarkedDocumentContent),
                    FileExtension = "PDF"
                };
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw;
            }
            finally
            {
                logger.DebugFormat("ToRasterFormatWatermarked - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public stDoc ToRasterFormatExBox(stDoc Doc, string ext, string label, BoxConfig boxConfig)
        {
            try
            {
                logger.DebugFormat("ToRasterFormatExBox - INIT - {0}", HttpContext.Current.Request.UserHostAddress);
                byte[] documentContent = Convert.FromBase64String(Doc.Blob);
                byte[] pdfDocumentContent = PrintRedirectedService.ConvertToFormatLabeledWithForm(documentContent, Doc.FileExtension, ext, label, boxConfig);
                return new stDoc
                {
                    Blob = Convert.ToBase64String(pdfDocumentContent),
                    FileExtension = ext
                };
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw;
            }
            finally
            {
                logger.DebugFormat("ToRasterFormatEx - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public stDoc ToRasterFormatEx(stDoc Doc, string ext, string label)
        {
            try
            {
                logger.DebugFormat("ToRasterFormatEx - INIT - {0}, {1}, {2}", HttpContext.Current.Request.UserHostAddress, ext, label);
                return ToRasterFormatExParameters(Doc, ext, label, null);
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw;
            }
            finally
            {
                logger.DebugFormat("ToRasterFormatEx - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public stDoc ToRasterFormatExParameters(stDoc Doc, string ext, string label, stParameter[] parameters)
        {
            try
            {
                logger.DebugFormat("ToRasterFormatExParameters - INIT - {0} - ext: {1}, label: {2}", HttpContext.Current.Request.UserHostAddress, ext, label);

                AttachConversionMode mode = AttachConversionMode.Default;
                if (parameters != null && parameters.Length > 0 &&
                    parameters.Any(x => x.Name == stParameterOption.AttachMode))
                {
                    stParameter foundParameter = parameters.First(x => x.Name == stParameterOption.AttachMode);
                    if (int.TryParse(foundParameter.Value, out int val))
                    {
                        mode = (AttachConversionMode)val;
                    }
                }

                byte[] documentContent = Convert.FromBase64String(Doc.Blob);
                byte[] pdfDocumentContent = PrintRedirectedService.ConvertToFormatLabeled(documentContent, Doc.FileExtension, ext, label, mode);
                return new stDoc
                {
                    Blob = Convert.ToBase64String(pdfDocumentContent),
                    FileExtension = ext
                };
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw;
            }
            finally
            {
                logger.DebugFormat("ToRasterFormatExParameters - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public stDoc PdfToPngThumbnail(stDoc Doc)
        {
            try
            {
                logger.DebugFormat("ToRasterFormatWatermarked - INIT - {0}", HttpContext.Current.Request.UserHostAddress);
                byte[] documentContent = Convert.FromBase64String(Doc.Blob);
                byte[] thumbnailContent = PrintRedirectedService.ConvertPdfToThumbnailPng(documentContent);
                return new stDoc
                {
                    Blob = Convert.ToBase64String(thumbnailContent)
                };
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                throw;
            }
            finally
            {
                logger.DebugFormat("ToRasterFormatWatermarked - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public string GetVersion()
        {
            try
            {
                logger.InfoFormat("GetVersion - INIT - {0}", HttpContext.Current.Request.UserHostAddress);
                return PrintRedirectedService.GetVersion();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
            finally
            {
                logger.InfoFormat("GetVersion - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public int GetNumberOfPages(stDoc document, string fileName)
        {
            try
            {
                logger.InfoFormat("GetNumberOfPages - INIT - {0}", HttpContext.Current.Request.UserHostAddress);
                byte[] documentContent = Convert.FromBase64String(document.Blob);
                return PrintRedirectedService.GetNumberOfPages(documentContent, fileName);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
            finally
            {
                logger.InfoFormat("GetNumberOfPages - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public stDoc CreateSecureDocument(stDoc document, string filename, string label)
        {
            try
            {
                logger.InfoFormat("CreateSecureDocument - INIT - {0}", HttpContext.Current.Request.UserHostAddress);
                if (_secureDocumentService == null)
                {
                    logger.Warn("CreateSecureDocument - SecureDocumentService not initialize. Please check application configuration settings.");
                    throw new Exception("Servizio di securizzazione non abilitato. Verificare le impostazioni del sistema.");
                }

                filename = Path.GetFileName(filename);
                logger.DebugFormat("CreateSecureDocument - Convert document {0} to PDF", filename);
                stDoc pdfDocument = ToRasterFormatEx(document, Path.GetExtension(filename), label);
                SecureDocumentModel request = new SecureDocumentModel()
                {
                    DocumentContent = Convert.FromBase64String(pdfDocument.Blob),
                    DocumentName = string.Concat(Path.GetFileName(filename), ".pdf")
                };
                logger.DebugFormat("CreateSecureDocument - Call create secure document for document {0}", filename);
                SecureDocumentModel response = _secureDocumentService.Create(request);
                return new stDoc()
                {
                    Blob = Convert.ToBase64String(response.DocumentContent),
                    ReferenceId = response.IdDocument
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
            finally
            {
                logger.InfoFormat("CreateSecureDocument - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public void UploadSecureDocument(stDoc document)
        {
            try
            {
                logger.InfoFormat("UploadSecureDocument - INIT - {0}", HttpContext.Current.Request.UserHostAddress);
                if (_secureDocumentService == null)
                {
                    logger.Warn("UploadSecureDocument - SecureDocumentService not initialize. Please check application configuration settings.");
                    throw new Exception("Servizio di securizzazione non abilitato. Verificare le impostazioni del sistema.");
                }

                SecureDocumentModel request = new SecureDocumentModel()
                {
                    DocumentContent = Convert.FromBase64String(document.Blob),
                    IdDocument = document.ReferenceId
                };
                logger.DebugFormat("UploadSecureDocument - Call upload secure document for iddocument {0}", document.ReferenceId);
                _secureDocumentService.Upload(request);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
            finally
            {
                logger.InfoFormat("UploadSecureDocument - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }

        [WebMethod]
        public stDoc ToRasterXmlWithStylesheet(stDoc toConvertDocument, byte[] xsl, string label)
        {
            try
            {
                logger.InfoFormat("ToRasterXmlWithStylesheet - INIT - {0}", HttpContext.Current.Request.UserHostAddress);
                byte[] documentContent = Convert.FromBase64String(toConvertDocument.Blob);
                byte[] convertedDocumentContent = PrintRedirectedService.ConvertXmlToPdfWithStylesheet(documentContent, xsl, toConvertDocument.FileExtension, label);
                return new stDoc
                {
                    Blob = Convert.ToBase64String(convertedDocumentContent)
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw ex;
            }
            finally
            {
                logger.InfoFormat("ToRasterXmlWithStylesheet - END - {0}", HttpContext.Current.Request.UserHostAddress);
            }
        }
        #endregion
    }
}
