using System;
using System.Reflection;
using System.IO;
using VecompSoftware.Commons.BiblosDS.Objects.Converters;
using VecompSoftware.Commons.BiblosDS.Objects.Enums;
using VecompSoftware.StampaConforme.Models.Office;
using VecompSoftware.DocSuiteWeb.EnterpriseLogging;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using System.Collections.Generic;
using VecompSoftware.StampConforme.Models.Commons;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;

namespace BiblosDS.Library.Common.Converter.Office
{
    [LogCategory(LogCategoryName.SERVICEOFFICE)]
    public class OfficeToPdfConverter : IConverter
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static ICollection<LogCategory> _logCategories;
        private readonly OfficeConverterService _converterService;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(OfficeToPdfConverter));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public OfficeToPdfConverter()
        {
            //TODO: Deve arrivare in ingresso dal costruttore
            _logger = new GlobalLogger();
            _converterService = new OfficeConverterService(_logger);
        }
        #endregion

        #region [ Methods ]
        public byte[] Convert(byte[] fileSource, string fileExtension, string extReq, AttachConversionMode mode)
        {            
            string fileName = Path.GetTempFileName();
            fileExtension = GetCorrectFileExtension(fileExtension);
            string destination = string.Concat(fileName, ".", extReq);
            try
            {                
                string formattedExtension = string.Concat(".", fileExtension.Replace(".", string.Empty));
                fileName = string.Concat(fileName, formattedExtension);
                File.WriteAllBytes(fileName, fileSource);
                _logger.WriteInfo(new LogMessage(string.Concat("Convert -> fileName: ", fileExtension, " - extension: ", fileExtension, " to pdf")), LogCategories);

                if (!_converterService.ConvertToPdf(fileName, destination, (ConversionMode)mode))
                {
                    return null;
                }
                return File.ReadAllBytes(destination);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage(string.Concat("Convert -> error on convert item ", fileName, " to pdf")), ex, LogCategories);
                throw;
            }
            finally
            {
                try
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("Convert -> deleting: {0}", fileName)), LogCategories);
                    File.Delete(fileName);
                    _logger.WriteDebug(new LogMessage(string.Concat("Convert -> deleting: {0}", destination)), LogCategories);
                    File.Delete(destination);
                }
                catch (Exception ex)
                {
                    _logger.WriteError(new LogMessage("Convert -> error on deleting pending files"), ex, LogCategories);
                }
            }
        }

        private string GetCorrectFileExtension(string fileName)
        {
            string path = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(path))
                return fileName;
            return path;
        }

        public string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        #endregion
    }
}
