using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;
using VecompSoftware.DocSuite.Document.Generator.PDF.SC;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters;
using ModelDocument = VecompSoftware.DocSuiteWeb.Model.Documents;
using StorageDocument = VecompSoftware.DocSuite.Document;

namespace VecompSoftware.DocSuite.Document.Generator.PDF
{
    [LogCategory(LogCategoryDefinition.DOCUMENTGENERATOR)]
    public class PDFDocumentGenerator : IPDFDocumentGenerator
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static ICollection<LogCategory> _logCategories;
        private readonly SC.BiblosDSConvSoapClient _pdfGeneratorClient;
        private readonly StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> _documentService;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IDecryptedParameterEnvService _parameterEnvService;
        private readonly XmlDocument _signatureTemplate;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(PDFDocumentGenerator));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public PDFDocumentGenerator(ILogger logger, IDataUnitOfWork unitOfWork, IDecryptedParameterEnvService parameterEnvService,
            StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
        {
            _logger = logger;
            _pdfGeneratorClient = new SC.BiblosDSConvSoapClient();
            _unitOfWork = unitOfWork;
            _documentService = documentService;
            _parameterEnvService = parameterEnvService;
            _signatureTemplate = new XmlDocument();
            _signatureTemplate.LoadXml(parameterEnvService.SignatureTemplate);
        }
        #endregion

        #region [ Methods ]
        private bool MatchExtension(string filename, string extension)
        {
            return !string.IsNullOrEmpty(filename) && filename.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsCompliantPrintFileName(string fileName)
        {
            List<string> list = new List<string>() { ".p7m", ".p7x", ".m7m", ".tds", ".p7s" };
            return list.Any(e => MatchExtension(fileName, e)) || string.IsNullOrEmpty(fileName);
        }

        private string CorrectSignedExtensions(string source)
        {
            if (!IsCompliantPrintFileName(source))
            {
                return source;
            }

            char[] extToReplace = Path.GetInvalidFileNameChars();
            foreach (char c in extToReplace.Where(c => source.Contains(c)))
            {
                source = source.Replace(c.ToString(CultureInfo.InvariantCulture), string.Empty);
            }

            Stack extensionStack = new Stack();
            while (Path.HasExtension(source))
            {
                string extension = Path.GetExtension(source);
                if (string.IsNullOrEmpty(extension))
                {
                    break;
                }

                string newExtension = Regex.Match(extension, @"(?<=\.)[^\W\s]+").Value;
                if (string.IsNullOrEmpty(newExtension))
                {
                    break;
                }

                extensionStack.Push(newExtension);
                source = source.Remove(source.Length - extension.Length, extension.Length);

                if (!IsCompliantPrintFileName("." + newExtension))
                {
                    break;
                }
            }

            if (extensionStack.Count == 0)
            {
                return source;
            }

            return extensionStack.Cast<object>().Aggregate(source, (current, ext) => $"{current}.{ext}");
        }

        private string GetSignature(string source)
        {
            return _signatureTemplate.OuterXml.Replace("(SIGNATURE)", source);
        }

        public async Task<byte[]> GenerateDocumentAsync(Guid idTemplate, IDocumentGeneratorModel parameters)
        {
            return await DocumentGeneratorHelper.TryCatchWithLogger(async () =>
            {
                _logger.WriteDebug(new LogMessage($"Request PDF generation with idTemplate {idTemplate}"), LogCategories);
                if (idTemplate.Equals(Guid.Empty))
                {
                    throw new ArgumentException("Parameter idTemplate is empty", nameof(idTemplate));
                }

                Guid? IdArchiveChain = _unitOfWork.Repository<TemplateDocumentRepository>().GetDocumentIdArchiveChainById(idTemplate);

                if (!IdArchiveChain.HasValue)
                {
                    throw new DSWException($"Template '{idTemplate}' not found", null, DSWExceptionCode.Invalid);
                }

                ModelDocument.Document document = (await _documentService.GetDocumentLatestVersionFromChainAsync(IdArchiveChain.Value)).FirstOrDefault();
                if (document == null)
                {
                    throw new DSWException($"Document '{IdArchiveChain.Value}' not found", null, DSWExceptionCode.Invalid);
                }
                BuilderParameter builderParameter = new BuilderParameter(parameters.DocumentGeneratorParameters);
                byte[] content = await _documentService.GetDocumentContentAsync(document.IdDocument);
                return (await _pdfGeneratorClient.BuildPDFAsync(content, builderParameter.MappedParameters.ToArray(), string.Empty)).Body.BuildPDFResult;
            }, _logger, LogCategories);
        }

        public async Task<byte[]> GeneratePdfAsync(byte[] content, string filename, string signature)
        {
            return await DocumentGeneratorHelper.TryCatchWithLogger(async () =>
            {
                _logger.WriteInfo(new LogMessage($"Generate PDF from content {content?.Length}, filename {filename} and signature {signature}"), LogCategories);
                if (content == null || content.Length <= 0)
                {
                    throw new DSWException($"Content is empty", null, DSWExceptionCode.Invalid);
                }

                string extension = Path.GetExtension(filename);
                if (IsCompliantPrintFileName(filename))
                {
                    extension = filename;
                }
                extension = CorrectSignedExtensions(extension);
                signature = GetSignature(signature);
                _logger.WriteDebug(new LogMessage($"set signature {signature} and extension {extension}"), LogCategories);
                SC.stDoc prepareDoc = new SC.stDoc
                {
                    Blob = Convert.ToBase64String(content),
                    FileExtension = extension,
                    SimpleSignersAttribute = string.Empty
                };
                SC.ToRasterFormatExResponse response = await _pdfGeneratorClient.ToRasterFormatExAsync(prepareDoc, "pdf", signature);
                return Convert.FromBase64String(response.Body.ToRasterFormatExResult.Blob);
            }, _logger, LogCategories);
        }

        public async Task<byte[]> GenerateDocumentAsync(Guid idTemplate, IDocumentGeneratorModel source, byte[] content = null)
        {
            try
            {
                byte[] template = await GetLatestVersionAsync(idTemplate);
                IEnumerable<KeyValuePair<string, string>> parametersParsered = source.DocumentGeneratorParameters.Select(s => ParseParameter(s));
                BuildValueModel[] buildValueModel = parametersParsered.Select(x => new BuildValueModel
                {
                    Name = x.Key,
                    Value = x.Value
                }).ToArray();

                BuildPDFResponse response = await _pdfGeneratorClient.BuildPDFAsync(template, buildValueModel, string.Empty);
                return response.Body.BuildPDFResult;
            }
            catch (Exception ex)
            {

                _logger.WriteError(ex, LogCategories);
                throw new DSWException(string.Concat("Document BiblosDSConvSoapClient layer - unexpected exception was thrown while invoking operation: ", ex.Message), ex, DSWExceptionCode.DM_Anomaly);
            }
        }

        public async Task<byte[]> GetLatestVersionAsync(Guid idTemplate)
        {
            _logger.WriteDebug(new LogMessage($"Request Word generation with idTemplate {idTemplate}"), LogCategories);

            if (idTemplate.Equals(Guid.Empty))
            {
                throw new ArgumentException("Parameter idTemplate is empty.", nameof(idTemplate));
            }

            Guid? IdArchiveChain = _unitOfWork.Repository<TemplateDocumentRepository>().GetDocumentIdArchiveChainById(idTemplate);

            if (!IdArchiveChain.HasValue)
            {
                throw new DSWException($"Template '{idTemplate}' not found", null, DSWExceptionCode.Invalid);
            }

            DocSuiteWeb.Model.Documents.Document document = (await _documentService.GetDocumentLatestVersionFromChainAsync(IdArchiveChain.Value)).FirstOrDefault();
            if (document == null)
            {
                throw new DSWException($"Document '{IdArchiveChain.Value}' not found", null, DSWExceptionCode.Invalid);
            }

            return await _documentService.GetDocumentContentAsync(document.IdDocument);
        }

        public byte[] AppendTable(IDocumentGeneratorModel source, byte[] content)
        {
            throw new NotImplementedException();
        }

        #region [ Helpers ]
        private KeyValuePair<string, string> ParseParameter(IDocumentGeneratorParameter visitable)
        {
            if (visitable is BooleanParameter)
            {
                BooleanParameter b = visitable as BooleanParameter;
                return new KeyValuePair<string, string>(b.LookingTag, Convert.ToString(b.Value));
            }
            if (visitable is CharParameter)
            {
                CharParameter ch = visitable as CharParameter;
                return new KeyValuePair<string, string>(ch.LookingTag, ch.Value.ToString());
            }
            if (visitable is FloatParameter)
            {
                FloatParameter f = visitable as FloatParameter;
                return new KeyValuePair<string, string>(f.LookingTag, f.Value.ToString("00.000"));
            }
            if (visitable is GuidParameter)
            {
                GuidParameter g = visitable as GuidParameter;
                return new KeyValuePair<string, string>(g.LookingTag, g.Value.ToString());
            }
            if (visitable is IntParameter)
            {
                IntParameter i = visitable as IntParameter;
                return new KeyValuePair<string, string>(i.LookingTag, i.Value.ToString());
            }
            if (visitable is StringParameter)
            {
                StringParameter s = visitable as StringParameter;
                return new KeyValuePair<string, string>(s.LookingTag, HttpUtility.HtmlDecode(s.Value));
            }
            if (visitable is DateTimeParameter)
            {
                DateTimeParameter d = visitable as DateTimeParameter;
                return new KeyValuePair<string, string>(d.LookingTag, d.Value.ToShortDateString());
            }
            throw new DSWException($"Parameter '{visitable.GetType().Name}' is not correct", null, DSWExceptionCode.Invalid);
        }
        #endregion

        #endregion
    }
}
