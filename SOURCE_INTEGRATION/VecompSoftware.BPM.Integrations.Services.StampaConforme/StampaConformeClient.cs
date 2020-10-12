using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Services.StampaConforme.Service;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;

namespace VecompSoftware.BPM.Integrations.Services.StampaConforme
{
    [LogCategory(LogCategoryDefinition.DOCUMENTCONTEX)]
    public class StampaConformeClient : IStampaConformeClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly BiblosDSConvSoapClient _stampaConformeClient;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(StampaConformeClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public StampaConformeClient(ILogger logger)
        {
            _logger = logger;
            _stampaConformeClient = new BiblosDSConvSoapClient();
        }
        #endregion

        #region [ Methods ]
        public static string GetSignature(string signature, string source)
        {
            return signature.Replace("(SIGNATURE)", source);
        }

        public async Task<byte[]> ConvertToPDFAAsync(byte[] source, string fileExtension, string signature)
        {
            try
            {
                stDoc toConvertDocument = new stDoc { FileExtension = fileExtension, Blob = Convert.ToBase64String(source) };
                ToRasterFormatExResponse response = await _stampaConformeClient.ToRasterFormatExAsync(toConvertDocument, "pdf", signature);
                return Convert.FromBase64String(response.Body.ToRasterFormatExResult.Blob);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(string.Concat("Document StampaConforme layer - unexpected exception was thrown while invoking operation: ", ex.Message), ex, DSWExceptionCode.DM_Anomaly);
            }
        }

        public async Task UploadSecureDocumentAsync(byte[] source, string referenceId)
        {
            try
            {
                stDoc toUploadDocument = new stDoc { Blob = Convert.ToBase64String(source), ReferenceId = referenceId };
                await _stampaConformeClient.UploadSecureDocumentAsync(toUploadDocument);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex, LogCategories);
                throw new DSWException(string.Concat("Document StampaConforme layer - unexpected exception was thrown while invoking operation: ", ex.Message), ex, DSWExceptionCode.DM_Anomaly);
            }
        }

        public async Task<byte[]> BuildPDFAsync(byte[] template, BuildValueModel[] buildValueModel, string signature)
        {
            try
            {
                BuildPDFResponse response = await _stampaConformeClient.BuildPDFAsync(template, buildValueModel, signature);
                return response.Body.BuildPDFResult;
            }
            catch (Exception ex)
            {

                _logger.WriteError(ex, LogCategories);
                throw new DSWException(string.Concat("Document StampaConforme layer - unexpected exception was thrown while invoking operation: ", ex.Message), ex, DSWExceptionCode.DM_Anomaly);
            }
        }
        #endregion        
    }
}
