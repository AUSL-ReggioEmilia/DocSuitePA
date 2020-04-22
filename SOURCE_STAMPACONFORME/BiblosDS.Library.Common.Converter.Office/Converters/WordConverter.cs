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
using VecompSoftware.StampaConforme.Models.Office.Word;
using VecompSoftware.StampaConforme.Services.Office;
using VecompSoftware.StampConforme.Models.Commons;

namespace BiblosDS.Library.Common.Converter.Office.Converters
{
    [LogCategory(LogCategoryName.SERVICEOFFICE)]
    public class WordConverter : IServiceRule, IToPdfConverter
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static ICollection<LogCategory> _logCategories;
        private WordToPdfService _service;
        private const string XLS_EXTENSION = ".xls";
        private const string XLSX_EXTENSION = ".xlsx";
        private const string MSG_EXTENSION = ".msg";
        private const string EML_EXTENSION = ".eml";
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(WordConverter));
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

        private ICollection<string> RedirectFilters
        {
            get
            {
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["OpenOfficeFilter"]))
                {
                    return ConfigurationManager.AppSettings["OpenOfficeFilter"].Split(';');
                }
                return new List<string>();
            }
        }
        #endregion

        #region [ Constructor ]
        public WordConverter(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region [ Methods ]
        public bool CheckRule(string elementToCheck)
        {
            return (elementToCheck.ToLower() != XLS_EXTENSION && elementToCheck.ToLower() != XLSX_EXTENSION) 
                && (elementToCheck.ToLower() != EML_EXTENSION && elementToCheck.ToLower() != MSG_EXTENSION);
        }

        public bool Convert(string source, string destination, ConversionMode conversionMode = ConversionMode.Default)
        {
            using (_service = new WordToPdfService(_logger))
            {
                SaveDocumentToPdfRequest model = new SaveDocumentToPdfRequest()
                {
                    DestinationFilePath = destination,
                    ForcePortrait = ForcePortrait,
                    RedirectFilters = RedirectFilters,
                    SourceFilePath = source
                };
                return _service.SaveTo(model);
            }            
        }
        #endregion        
    }
}
