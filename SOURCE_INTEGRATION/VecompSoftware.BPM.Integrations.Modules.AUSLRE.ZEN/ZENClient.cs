using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Configurations;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN
{
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class ZENClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private const string _media_type = "application/json";
        private const string _authentication_header_scheme = "Basic";
        private readonly string _zenWebAPIUrl;
        private readonly string _zenPassword;
        private readonly string _zenUsername;
        private readonly string _zenToken;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(ZENClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public ZENClient(ILogger logger, string zenWebAPIUrl, string zenUsername, string zenPassword)
        {
            _logger = logger;
            _zenWebAPIUrl = zenWebAPIUrl;
            _zenUsername = zenUsername;
            _zenPassword = zenPassword;
            _zenToken = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Concat(_zenUsername, ":", _zenPassword)));
        }
        #endregion

        #region [ Methods ]
        public async Task SendEventAsync(DocSuiteEvent @event)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authentication_header_scheme, _zenToken);

                    _logger.WriteDebug(new LogMessage(string.Concat("SendEventAsync -> prepare httpcontent for event id ", @event.UniqueId)), LogCategories);
                    _logger.WriteDebug(new LogMessage(JsonConvert.SerializeObject(@event)), LogCategories);
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(@event), Encoding.UTF8, _media_type);
                    HttpResponseMessage response = await httpClient.PostAsync(_zenWebAPIUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.WriteError(new LogMessage(string.Concat("SendEventAsync -> ZEN web api error with status code ", response.StatusCode)), LogCategories);
                        throw new Exception(string.Concat("SendEventAsync -> ZEN web api error with status code ", response.StatusCode));
                    }
                    _logger.WriteInfo(new LogMessage(string.Concat("SendEventAsync -> event id ", @event.UniqueId, " sended correctly")), LogCategories);
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("SendEventAsync Error -> error on sending event to ZEN web api."), ex, LogCategories);
                throw ex;
            }
        }
        #endregion
    }
}
