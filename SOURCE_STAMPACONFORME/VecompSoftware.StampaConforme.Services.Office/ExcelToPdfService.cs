using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.StampaConforme.Interfaces.Office;
using VecompSoftware.StampaConforme.Models.Office.Excel;
using VecompSoftware.StampConforme.Models.Commons;

namespace VecompSoftware.StampaConforme.Services.Office
{
    [LogCategory(LogCategoryName.SERVICEOFFICE)]
    public class ExcelToPdfService : OfficeToPdfService, IExcelToPdfService
    {
        #region [ Fields ]
        private bool _disposed;
        private readonly ILogger _logger;
        private dynamic _excelInstance;
        private dynamic _excelWorkbook;
        private static ICollection<LogCategory> _logCategories;
        private const string APPLICATION_PROG_ID = "Excel.Application";
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ExcelToPdfService));
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
                if (_excelWorkbook != null)
                {
                    try
                    {
                        _excelWorkbook.Close(false, Missing.Value, Missing.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteWarning(new LogMessage("Error on dispose workbook instance. The exception will be ignore."), ex, LogCategories);
                    }
                }

                if (_excelInstance != null)
                {
                    try
                    {
                        _excelInstance.Quit();
                    }
                    catch (Exception ex)
                    {
                        _logger.WriteWarning(new LogMessage("Error on dispose excel instance. The exception will be ignore."), ex, LogCategories);
                    }
                }
            }

            _excelWorkbook = null;
            _excelInstance = null;
            _disposed = true;
        }
        #endregion

        #region [ Constructor ]
        public ExcelToPdfService(ILogger logger)
            : base(APPLICATION_PROG_ID)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ] 
        public bool SaveTo(SaveWorkbookToPdfRequest model)
        {
            try
            {
                return RetryingPolicyAction<bool>(() =>
                {
                    _logger.WriteInfo(new LogMessage(string.Concat("SaveTo -> process item ", model.SourceFilePath, ". Workbook type: ", Path.GetExtension(model.SourceFilePath))), LogCategories);
                OpenWorkbook(model.SourceFilePath);

                SetSheetsPaging(model);

                _excelWorkbook.ExportAsFixedFormat(WorkbookExportFormat.PDF, model.DestinationFilePath);
                _logger.WriteInfo(new LogMessage("SaveTo -> item saved to pdf correctly"), LogCategories);
                return true;
                }, "EXCEL", _logger, LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("SaveTo -> error on convert item ", model.SourceFilePath, " to pdf")), ex, LogCategories);
                throw;
            }
        }

        private void OpenWorkbook(string path)
        {
            InstantiateExcel();
            _excelWorkbook = _excelInstance.Workbooks.Open(path);
            if (_excelWorkbook == null)
            {
                _logger.WriteError(new LogMessage(string.Concat("OpenWorkbook -> item ", path, " not open.")), LogCategories);
                throw new Exception("Workbook not open. The office runtime not work correctly.");
            }
        }

        private void InstantiateExcel()
        {
            if (_excelInstance != null)
                return;

            _logger.WriteDebug(new LogMessage("InstantiateExcel -> create excel instance"), LogCategories);
            _excelInstance = GetRuntimeInstance();
            _excelInstance.Visible = false;
            _excelInstance.ScreenUpdating = false;
            _logger.WriteDebug(new LogMessage(string.Format("InstantiateExcel -> excel instance created: {0}", _excelInstance != null)), LogCategories);
        }

        private void SetSheetsPaging(SaveWorkbookToPdfRequest model)
        {
            if (_excelWorkbook.Sheets == null)
            {
                return;
            }

            foreach (dynamic sheet in _excelWorkbook.Sheets)
            {
                if (model.ForcePortrait)
                {
                    ForcePortrait(sheet, model);
                }
                sheet.PageSetup.PaperSize = WorkbookPaperSize.A4;
            }
        }

        private void ForcePortrait(dynamic sheet, SaveWorkbookToPdfRequest model)
        {
            if (model.FitToPagesTall.HasValue || model.FitToPagesWide.HasValue)
            {
                sheet.PageSetup.Zoom = false;
                if (model.FitToPagesTall.HasValue)
                {
                    sheet.PageSetup.FitToPagesTall = model.FitToPagesTall.Value;
                }
                if (model.FitToPagesWide.HasValue)
                {
                    sheet.PageSetup.FitToPagesWide = model.FitToPagesWide.Value;
                }
            }
            sheet.PageSetup.Orientation = WorkbookOrientation.Portrait;
        }
        #endregion
    }
}
