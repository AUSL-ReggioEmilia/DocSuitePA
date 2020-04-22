using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.StampaConforme.Interfaces.Common.Converters;
using VecompSoftware.StampaConforme.Interfaces.Common.ServiceRules;
using VecompSoftware.StampaConforme.Models.Office;
using VecompSoftware.StampaConforme.Models.Office.Excel;
using VecompSoftware.StampaConforme.Services.Office;
using VecompSoftware.StampConforme.Models.Commons;

namespace BiblosDS.Library.Common.Converter.Office.Converters
{
    [LogCategory(LogCategoryName.SERVICEOFFICE)]
    public class ExcelConverter : IServiceRule, IToPdfConverter
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static ICollection<LogCategory> _logCategories;
        private ExcelToPdfService _service;
        private const string XLS_EXTENSION = ".xls";
        private const string XLSX_EXTENSION = ".xlsx";
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ExcelConverter));
                }
                return _logCategories;
            }
        }

        private bool ForcePortrait
        {
            get
            {
                if (bool.TryParse(ConfigurationManager.AppSettings["StampaConforme.ForcePortrait"], out bool r))
                {
                    return r;
                }
                return false;
            }
        }

        private int? FitToPagesTall
        {
            get
            {
                if (ConfigurationManager.AppSettings["FitToPagesTall"] == null)
                {
                    return 1;
                }

                if (int.TryParse(ConfigurationManager.AppSettings["FitToPagesTall"], out int r))
                {
                    return r;
                }
                return null;
            }
        }

        private int? FitToPagesWide
        {
            get
            {
                if (ConfigurationManager.AppSettings["FitToPagesWide"] == null)
                {
                    return 1;
                }

                if (int.TryParse(ConfigurationManager.AppSettings["FitToPagesWide"], out int r))
                {
                    return r;
                }
                return null;
            }
        }
        #endregion

        #region [ Constructor ]
        public ExcelConverter(ILogger logger)
        {
            _logger = logger;            
        }
        #endregion

        #region [ Methods ]
        public bool CheckRule(string elementToCheck)
        {
            return (elementToCheck.ToLower() == XLS_EXTENSION || elementToCheck.ToLower() == XLSX_EXTENSION);
        }

        public bool Convert(string source, string destination, ConversionMode conversionMode = ConversionMode.Default)
        {
            using (_service = new ExcelToPdfService(_logger))
            {
                SaveWorkbookToPdfRequest model = new SaveWorkbookToPdfRequest()
                {
                    DestinationFilePath = destination,
                    ForcePortrait = ForcePortrait,
                    FitToPagesTall = FitToPagesTall,
                    FitToPagesWide = FitToPagesWide,
                    SourceFilePath = source
                };
                return _service.SaveTo(model);
            }            
        }
        #endregion        
    }
}
