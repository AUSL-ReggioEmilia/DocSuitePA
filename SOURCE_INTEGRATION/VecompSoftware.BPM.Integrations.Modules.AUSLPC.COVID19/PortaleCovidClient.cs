using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Configurations;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19.Models;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.COVID19
{
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class PortaleCovidClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly AuthModel _authModel;
        private const string _media_type = "application/json";
        private const string _authentication_header_scheme = "Bearer";
        private readonly string _apiUrl;
        private readonly string _apiAuthUrl;
        private const int _retry_tentative = 10;
        private readonly TimeSpan _threadWaiting = TimeSpan.FromSeconds(5);
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(PortaleCovidClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public PortaleCovidClient(ILogger logger, ModuleConfigurationModel moduleConfigurationModel)
        {
            _logger = logger;
            _apiUrl = moduleConfigurationModel.PortaleCovidAPIUrl;
            _apiAuthUrl = moduleConfigurationModel.PortaleCovidAPIAuthUrl;
            _authModel = new AuthModel(moduleConfigurationModel.PortaleCovidAPIUsername, moduleConfigurationModel.PortaleCovidAPIPassword);
        }
        #endregion

        #region [ Methods ]
        public async Task SendAsync(RequestModel request)
        {
            await RetryingPolicyAction(async () =>
            {
                string token = await GetTokenAsync();
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authentication_header_scheme, token);

                    _logger.WriteDebug(new LogMessage($"SendEventAsync -> prepare httpcontent"), LogCategories);
                    _logger.WriteDebug(new LogMessage(JsonConvert.SerializeObject(request)), LogCategories);
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, _media_type);
                    HttpResponseMessage response = await httpClient.PostAsync(_apiUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.WriteError(new LogMessage($"SendEventAsync -> COVID portal web api error with status code {response.StatusCode}"), LogCategories);
                        throw new Exception($"SendEventAsync -> COVID portal web api error with status code {response.StatusCode}");
                    }
                    _logger.WriteInfo(new LogMessage($"SendEventAsync -> request sended correctly"), LogCategories);
                }
            });
        }

        private async Task<string> GetTokenAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(_authModel), Encoding.UTF8, _media_type);
                HttpResponseMessage response = await httpClient.PostAsync(_apiAuthUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.WriteError(new LogMessage($"GetTokenAsync -> COVID portal web api error with status code {response.StatusCode}"), LogCategories);
                    throw new Exception($"GetTokenAsync -> COVID portal web api error with status code {response.StatusCode}");
                }
                return await response.Content.ReadAsStringAsync();
            }
        }

        private async Task RetryingPolicyAction(Func<Task> func, int step = 1, Exception lastException = null)
        {
            _logger.WriteDebug(new LogMessage($"RetryingPolicyAction : tentative {step}/{_retry_tentative} in progress..."), LogCategories);
            if (step >= _retry_tentative)
            {
                throw lastException ?? new Exception("retry policy expired maximum tentatives");
            }
            try
            {
                await func();
            }
            catch (Exception ex)
            {
                _logger.WriteWarning(new LogMessage($"SafeActionWithRetryPolicy : tentative {step}/{_retry_tentative} faild. Waiting {_threadWaiting} second before retrying action"), ex, LogCategories);
                Task.Delay(_threadWaiting).Wait();
                await RetryingPolicyAction(func, step: ++step, lastException: ex);
            }
        }
        #endregion
    }
}
