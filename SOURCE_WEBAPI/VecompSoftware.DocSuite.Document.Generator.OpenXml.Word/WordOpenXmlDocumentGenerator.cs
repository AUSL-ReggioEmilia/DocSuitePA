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

namespace VecompSoftware.DocSuite.Document.Generator.OpenXml.Word
{
    [LogCategory(LogCategoryDefinition.DOCUMENTGENERATOR)]
    public class WordOpenXmlDocumentGenerator : IWordOpenXmlDocumentGenerator
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static ICollection<LogCategory> _logCategories;
        private readonly DocumentBiblosDS documentBiblos;
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
        public WordOpenXmlDocumentGenerator(ILogger logger, IDataUnitOfWork unitOfWork)
        {
            _logger = logger;
            documentBiblos = new DocumentBiblosDS(logger);
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        public async Task<byte[]> GenerateDocumentAsync(Guid idTemplate, IDocumentGeneratorModel parameters)
        {
            return await DocumentGeneratorHelper.TryCatchWithLogger(async () =>
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

                DocSuiteWeb.Model.Documents.Document document = (await documentBiblos.GetDocumentLatestVersionFromChainAsync(IdArchiveChain.Value)).FirstOrDefault();
                if (document == null)
                {
                    throw new DSWException($"Document '{IdArchiveChain.Value}' not found", null, DSWExceptionCode.Invalid);
                }

                byte[] content = await documentBiblos.GetDocumentContentAsync(document.IdDocument);
                WordGeneratorBuilder builder = new WordGeneratorBuilder();
                return builder.GenerateWordDocument(content, parameters.DocumentGeneratorParameters);
            }, _logger, LogCategories);
        }
        #endregion
    }
}
