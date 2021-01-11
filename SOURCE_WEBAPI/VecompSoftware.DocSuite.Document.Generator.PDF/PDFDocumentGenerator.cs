using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
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
        public PDFDocumentGenerator(ILogger logger, IDataUnitOfWork unitOfWork,
            StorageDocument.IDocumentContext<ModelDocument.Document, ModelDocument.ArchiveDocument> documentService)
        {
            _logger = logger;
            _pdfGeneratorClient = new SC.BiblosDSConvSoapClient();
            _unitOfWork = unitOfWork;
            _documentService = documentService;
        }
        #endregion

        #region [ Methods ]
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

        public Task<byte[]> GenerateDocumentAsync(Guid idTemplate, IDocumentGeneratorModel source, byte[] content = null)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GetLatestVersionAsync(Guid idTemplate)
        {
            throw new NotImplementedException();
        }

        public byte[] AppendTable(IDocumentGeneratorModel source, byte[] content)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
