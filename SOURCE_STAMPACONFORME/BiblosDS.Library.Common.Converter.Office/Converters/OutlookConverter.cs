using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.StampaConforme.Interfaces.Common.Converters;
using VecompSoftware.StampaConforme.Interfaces.Common.ServiceRules;
using VecompSoftware.StampaConforme.Models.Office;
using VecompSoftware.StampaConforme.Models.Office.Outlook;
using VecompSoftware.StampaConforme.Services.Office;
using VecompSoftware.StampConforme.Models.Commons;

namespace BiblosDS.Library.Common.Converter.Office.Converters
{
    [LogCategory(LogCategoryName.SERVICEOFFICE)]
    public class OutlookConverter : IServiceRule, IToPdfConverter
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static ICollection<LogCategory> _logCategories;
        private OutlookToPdfService _service;
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
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(OutlookConverter));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public OutlookConverter(ILogger logger)
        {
            _logger = logger;            
        }
        #endregion

        #region [ Methods ]
        public bool CheckRule(string elementToCheck)
        {
            return (elementToCheck.ToLower() == MSG_EXTENSION || elementToCheck.ToLower() == EML_EXTENSION);
        }

        public bool Convert(string source, string destination, ConversionMode conversionMode = ConversionMode.Default)
        {
            using (_service = new OutlookToPdfService(_logger))
            {
                SaveMailToPdfRequest model = new SaveMailToPdfRequest()
                {
                    DestinationFilePath = destination,
                    ConversionMode = conversionMode,
                    SourceFilePath = source
                };
                return _service.SaveTo(model);
            }            
        }
        #endregion        
    }
}
