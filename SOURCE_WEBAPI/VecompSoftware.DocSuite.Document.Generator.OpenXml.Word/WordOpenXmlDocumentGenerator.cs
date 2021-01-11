using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;
using VecompSoftware.DocSuite.Document.BiblosDS;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Templates;
using VecompSoftware.DocSuiteWeb.Finder.Templates;
using ModelDocuments = VecompSoftware.DocSuiteWeb.Model.Documents;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace VecompSoftware.DocSuite.Document.Generator.OpenXml.Word
{
    [LogCategory(LogCategoryDefinition.DOCUMENTGENERATOR)]
    public class WordOpenXmlDocumentGenerator : IWordOpenXmlDocumentGenerator
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static ICollection<LogCategory> _logCategories;
        private readonly IDocumentContext<ModelDocuments.Document, ModelDocuments.ArchiveDocument> _documentBiblos;
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Properties ]
        protected static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WordOpenXmlDocumentGenerator));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public WordOpenXmlDocumentGenerator(ILogger logger, IDataUnitOfWork unitOfWork, IDocumentContext<ModelDocuments.Document, ModelDocuments.ArchiveDocument> documentBiblos)
        {
            _logger = logger;
            _documentBiblos = documentBiblos;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        public async Task<byte[]> GenerateDocumentAsync(Guid idTemplate, IDocumentGeneratorModel parameters, byte[] content = null)
        {
            return await DocumentGeneratorHelper.TryCatchWithLogger(async () =>
            {
                if (content == null)
                {
                    content = await GetLatestVersionAsync(idTemplate);
                }
                WordGeneratorBuilder builder = new WordGeneratorBuilder();
                return builder.GenerateWordDocument(content, parameters.DocumentGeneratorParameters);
            }, _logger, LogCategories);
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

            DocSuiteWeb.Model.Documents.Document document = (await _documentBiblos.GetDocumentLatestVersionFromChainAsync(IdArchiveChain.Value)).FirstOrDefault();
            if (document == null)
            {
                throw new DSWException($"Document '{IdArchiveChain.Value}' not found", null, DSWExceptionCode.Invalid);
            }

            return await _documentBiblos.GetDocumentContentAsync(document.IdDocument);
        }

        public byte[] AppendTable(IDocumentGeneratorModel parameters, byte[] content)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(content, 0, content.Length);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                {
                    WordGeneratorBuilder builder = new WordGeneratorBuilder();
                    Table wordTable = builder.BuildWordTable(parameters.DocumentGeneratorParameters);
                    if (wordTable != null)
                    {
                        doc.MainDocumentPart.Document.Body.Append(new Paragraph());
                        doc.MainDocumentPart.Document.Body.Append(wordTable);
                    }
                }
                return stream.ToArray();
            }
        }
        #endregion
    }
}
