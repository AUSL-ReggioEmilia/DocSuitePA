using BiblosDS.Library.Common.Converter.Office.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.StampaConforme.Interfaces.Common.Converters;
using VecompSoftware.StampaConforme.Interfaces.Common.ServiceRules;
using VecompSoftware.StampaConforme.Interfaces.Common.Services;
using VecompSoftware.StampaConforme.Models.Office;
using VecompSoftware.StampaConforme.Services.Office;
using VecompSoftware.StampConforme.Models.Commons;

namespace BiblosDS.Library.Common.Converter.Office
{
    [LogCategory(LogCategoryName.SERVICEOFFICE)]
    public class OfficeConverterService
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static ICollection<LogCategory> _logCategories;
        private readonly ICollection<IToPdfConverter> _converters;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(OfficeConverterService));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public OfficeConverterService(ILogger logger)
        {
            _logger = logger;
            _converters = new List<IToPdfConverter>()
            {
                new ExcelConverter(logger),
                new WordConverter(logger),
                new OutlookConverter(logger)
            };
        }
        #endregion

        #region [ Methods ]
        private IToPdfConverter GetConverter(string fileExtension)
        {
            foreach (IToPdfConverter converter in _converters)
            {
                if ((converter as IServiceRule).CheckRule(fileExtension))
                {
                    return converter;
                }
            }
            return null;
        }

        public bool ConvertToPdf(string sourcePath, string destinationPath, ConversionMode conversionMode = ConversionMode.Default)
        {
            string extension = Path.GetExtension(sourcePath);
            IToPdfConverter converter = GetConverter(extension);
            if (converter == null)
            {
                throw new Exception(string.Concat("Converter not found for file type ", extension));
            }
            return converter.Convert(sourcePath, destinationPath, conversionMode);          
        }
        #endregion
    }
}
