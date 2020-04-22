using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.StampaConforme.Interfaces.Office;
using VecompSoftware.StampaConforme.Models.Office;
using VecompSoftware.StampaConforme.Models.Office.Word;
using VecompSoftware.StampConforme.Models.Commons;

namespace VecompSoftware.StampaConforme.Services.Office
{
    [LogCategory(LogCategoryName.SERVICEOFFICE)]
    public class WordToPdfService : OfficeToPdfService, IWordToPdfService
    {
        #region [ Fields ]
        private bool _disposed;
        private readonly ILogger _logger;
        private dynamic _wordInstance;
        private dynamic _wordDocument;
        private static ICollection<LogCategory> _logCategories;
        private const string APPLICATION_PROG_ID = "Word.Application";
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WordToPdfService));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Dispose ]
        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_wordDocument != null)
                {
                    try
                    {
                        _wordDocument.Close(false, Missing.Value, Missing.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteWarning(new LogMessage("Error on dispose document instance. The exception will be ignore."), ex, LogCategories);
                    }
                }

                if (_wordInstance != null)
                {
                    try
                    {
                        _wordInstance.Quit(false, Missing.Value, Missing.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteWarning(new LogMessage("Error on dispose word instance. The exception will be ignore."), ex, LogCategories);
                    }
                }
            }

            _wordDocument = null;
            _wordInstance = null;
            _disposed = true;
        }
        #endregion

        #region [ Constructor ]
        public WordToPdfService(ILogger logger)
            : base(APPLICATION_PROG_ID)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]           
        public bool SaveTo(SaveDocumentToPdfRequest model)
        {
            try
            {
                return RetryingPolicyAction<bool>(() =>
                {
                    _logger.WriteInfo(new LogMessage(string.Concat("SaveTo -> process item ", model.SourceFilePath, ". Document type: ", Path.GetExtension(model.SourceFilePath))), LogCategories);
                    OpenDocument(model.SourceFilePath);
                    if (HasShapesToRedirect(model.RedirectFilters))
                    {
                        return false;
                    }

                    if (_wordDocument.Sections != null && model.ForcePortrait)
                    {
                        ForcePortrait();
                    }

                    _wordDocument.ExportAsFixedFormat(model.DestinationFilePath, DocumentExportFormat.PDF, false, DocumentExportOptimizeFor.Print, DocumentExportRange.AllDocument, 1, 1, DocumentExportItem.DocumentContent, true,
                          true, DocumentExportCreateBookmarks.NoBookmarks, true, true, true);
                    _logger.WriteInfo(new LogMessage("SaveTo -> item saved to pdf correctly"), LogCategories);
                    return true;
                }, "WINWORD", _logger, LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("SaveTo -> error on convert item ", model.SourceFilePath, " to pdf")), ex, LogCategories);
                throw;
            }
        }

        private void OpenDocument(string path)
        {
            InstantiateWord();
            _wordDocument = _wordInstance.Documents.Open(path, false, true);
            if (_wordDocument == null)
            {
                _logger.WriteError(new LogMessage(string.Concat("OpenDocument -> item ", path, " not open.")), LogCategories);
                throw new Exception("Document not open. The office runtime not work correctly.");
            }
        }

        private void InstantiateWord()
        {
            if (_wordInstance != null)
                return;

            _logger.WriteDebug(new LogMessage("InstantiateWord -> create word instance"), LogCategories);
            _wordInstance = GetRuntimeInstance();
            _wordInstance.Visible = false;
            _wordInstance.ScreenUpdating = false;
            _logger.WriteDebug(new LogMessage(string.Format("InstantiateWord -> word instance created: {0}", _wordInstance != null)), LogCategories);
        }

        private bool HasShapesToRedirect(ICollection<string> redirectFilters)
        {
            if (redirectFilters != null && redirectFilters.Count > 0)
            {
                foreach (dynamic shape in (_wordDocument.Shapes as IEnumerable).OfType<dynamic>().Where(x => x.Type == (int)ShapeType.Picture))
                {
                    if (redirectFilters.Any(x => shape.ID == int.Parse(x.Split('|')[0]) && shape.Name == x.Split('|')[1]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ForcePortrait()
        {
            IEnumerable<dynamic> sections = (_wordDocument.Sections as IEnumerable).OfType<dynamic>().Where(x => x.PageSetup != null && x.PageSetup.Orientation != (int)DocumentOrientation.Portrait);
            if (_wordDocument.Tables != null && sections.Any())
            {
                foreach (dynamic table in _wordDocument.Tables)
                {
                    table.AutoFitBehavior(DocumentAutoFitBehaviour.Window);
                    table.AllowAutoFit = true;
                }
            }

            foreach (dynamic section in sections)
            {
                section.PageSetup.Orientation = DocumentOrientation.Portrait;
            }
        }
        #endregion
    }
}
