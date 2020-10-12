using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Configurations;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO
{
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class AVELCOClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private const string _media_type = "application/json";
        private readonly string _webAPIUrl;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(AVELCOClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public AVELCOClient(ILogger logger, string webAPIUrl)
        {
            _logger = logger;
            _webAPIUrl = webAPIUrl;
        }
        #endregion

        #region [ Methods ]
        public async Task SendEventAsync(DocSuiteEvent @event)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    _logger.WriteDebug(new LogMessage(string.Concat("SendEventAsync -> prepare httpcontent for event id ", @event.UniqueId)), LogCategories);
                    _logger.WriteDebug(new LogMessage(JsonConvert.SerializeObject(@event)), LogCategories);
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(@event), Encoding.UTF8, _media_type);
                    HttpResponseMessage response = await httpClient.PostAsync(_webAPIUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.WriteError(new LogMessage(string.Concat("SendEventAsync -> AVELCO web api error with status code ", response.StatusCode)), LogCategories);
                        throw new Exception(string.Concat("SendEventAsync -> AVELCO web api error with status code ", response.StatusCode));
                    }
                    _logger.WriteInfo(new LogMessage(string.Concat("SendEventAsync -> event id ", @event.UniqueId, " sended correctly")), LogCategories);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("SendEventAsync Error -> error on sending event to AVELCO web api."), ex, LogCategories);
                throw ex;
            }
        }
        #endregion
    }
}
