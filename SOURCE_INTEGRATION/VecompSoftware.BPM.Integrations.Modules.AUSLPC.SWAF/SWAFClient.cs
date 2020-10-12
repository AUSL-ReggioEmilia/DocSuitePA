using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF.Configurations;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;

namespace VecompSoftware.BPM.Integrations.Modules.AUSLPC.SWAF
{
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class SWAFClient
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private static IEnumerable<LogCategory> _logCategories;
        private const string _media_type = "application/json";
        private readonly string _swafAPIUrl;
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
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(SWAFClient));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        public SWAFClient(ILogger logger, string swafAPIUrl)
        {
            _logger = logger;
            _swafAPIUrl = swafAPIUrl;
        }
        #endregion

        #region [ Methods ]
        public async Task SendEventAsync(DocSuiteEvent @event)
        {
            await RetryingPolicyAction(async () =>
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    _logger.WriteDebug(new LogMessage($"SendEventAsync -> prepare httpcontent for event id {@event.UniqueId}"), LogCategories);
                    _logger.WriteDebug(new LogMessage(JsonConvert.SerializeObject(@event)), LogCategories);
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(@event), Encoding.UTF8, _media_type);
                    HttpResponseMessage response = await httpClient.PostAsync(_swafAPIUrl, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.WriteError(new LogMessage($"SendEventAsync -> SWAF web api error with status code {response.StatusCode}"), LogCategories);
                        throw new Exception($"SendEventAsync -> SWAF web api error with status code {response.StatusCode}");
                    }
                    _logger.WriteInfo(new LogMessage($"SendEventAsync -> event id {@event.UniqueId} sended correctly"), LogCategories);
                }
            });
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
