using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.IP4D.Configurations;
using VecompSoftware.BPM.Integrations.Modules.***REMOVED***.IP4D.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.ExternalModels;
using VecompSoftware.Services.Command.CQRS.Commands.Models.ExternalViewer;
using VecompSoftware.Services.Command.CQRS.Events.Models.ExternalViewer;

namespace VecompSoftware.BPM.Integrations.Modules.***REMOVED***.IP4D
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly IList<Guid> _subscriptions = new List<Guid>();
        private bool _needInitializeModule = false;
        #endregion

        #region [ Properties ]
        private static IEnumerable<LogCategory> LogCategories
        {
            get
            {
                if (_logCategories == null)
                {
                    _logCategories = LogCategoryHelper.GetCategoriesAttribute(typeof(Execution));
                }
                return _logCategories;
            }
        }
        #endregion

        #region [ Constructor ]
        [ImportingConstructor]
        public Execution(ILogger logger, IServiceBusClient serviceBusClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _needInitializeModule = true;
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("***REMOVED***.IP4D -> Critical error in costruction module"), ex, LogCategories);
                throw;
            }
        }
        #endregion

        #region [ Methods ]
        protected override void Execute()
        {
            if (Cancel)
            {
                return;
            }

            try
            {
                InitializeModule();
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("***REMOVED*** IP4D -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            CleanSubscriptions();
            _logger.WriteInfo(new LogMessage("OnStop -> ***REMOVED***.IP4D"), LogCategories);
        }

        private void InitializeModule()
        {
            if (_needInitializeModule)
            {
                _logger.WriteDebug(new LogMessage("Initialize module"), LogCategories);
                _subscriptions.Add(_serviceBusClient.StartListening<IEventProtocolExternalViewer>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.Topic_IP4DToExternalViewer, 
                    _moduleConfiguration.Subscription_IP4DToExternalViewer, EventProtocolExternalViewerCallbackAsync));
                _subscriptions.Add(_serviceBusClient.StartListening<ICommandProtocolExternalViewer>(ModuleConfigurationHelper.MODULE_NAME, _moduleConfiguration.Queue_IP4DProtocolExternalViewer,
                    CommandProtocolExternalViewerCallbackAsync));

                _needInitializeModule = false;
            }
        }

        internal void CleanSubscriptions()
        {
            foreach (Guid item in _subscriptions)
            {
                _serviceBusClient.CloseListeningAsync(item).Wait();
            }
            _subscriptions.Clear();
            _needInitializeModule = true;
        }

        private async Task SentIP4D<T>(T model, string url)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream mem = new MemoryStream())
            using (WebClient webClient = new WebClient())
            {
                ser.WriteObject(mem, model);
                string data = Encoding.UTF8.GetString(mem.ToArray(), 0, (int)mem.Length);
                webClient.Headers["Content-type"] = "application/json";
                webClient.Encoding = Encoding.UTF8;
                webClient.UseDefaultCredentials = true;
                webClient.Credentials = new NetworkCredential(_moduleConfiguration.IP4D_username, _moduleConfiguration.IP4D_password, _moduleConfiguration.IP4D_domain);
                _logger.WriteDebug(new LogMessage(string.Format("WebClient posting {0} ... ", model.GetType())), LogCategories);
                _logger.WriteDebug(new LogMessage(data), LogCategories);
                string res = await webClient.UploadStringTaskAsync(url, "POST", data);
                _logger.WriteDebug(new LogMessage(string.Format("WebClient receive {0} ... ", res)), LogCategories);
                ResponseContract responseContract = JsonConvert.DeserializeObject<ResponseContract>(res);
                if (responseContract == null)
                {
                    _logger.WriteError(new LogMessage(string.Concat("SentIP4D Receive null ResponseContract: ", res)), LogCategories);
                    throw new Exception("Response unknow");
                }
                if (!responseContract.Status.Equals("OK", StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.WriteWarning(new LogMessage(string.Concat("SentIP4D Receive Status KO : ", responseContract.ErrorMessage)), LogCategories);
                    throw new Exception(string.Concat("Status KO : ", responseContract.ErrorMessage));
                }
                _logger.WriteInfo(new LogMessage(string.Concat("SentIP4D Receive Status OK : ", responseContract.metodochiamato)), LogCategories);
            }

        }

        private async Task CommandProtocolExternalViewerCallbackAsync(ICommandProtocolExternalViewer cmd)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage(string.Format("CommandProtocolExternalViewerCallbackAsync -> received callback with event id {0}", cmd.Id)), LogCategories);

            try
            {
                ExternalViewerModel externalViewerModel = cmd.ContentType.ContentTypeValue;
                SendNewProtocolContract sendNewProtocolContract = new SendNewProtocolContract
                {
                    username = externalViewerModel.Recipients.First().Account,
                    uri_protocollo = externalViewerModel.Url,
                    protocollo = string.Concat(externalViewerModel.Year.ToString(), "/", externalViewerModel.Number.ToString())
                };

                _logger.WriteDebug(new LogMessage(string.Concat("Send Protocol ", sendNewProtocolContract.protocollo, " destinated ", sendNewProtocolContract.username)), LogCategories);
                await SentIP4D(sendNewProtocolContract, _moduleConfiguration.IP4D_SendNewProtocolUrl);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventProtocolExternalViewerCallbackAsync -> error complete call IP4D Services"), ex, LogCategories);
                throw;
            }

        }

        private async Task EventProtocolExternalViewerCallbackAsync(IEventProtocolExternalViewer evt)
        {
            if (Cancel)
            {
                return;
            }
            _logger.WriteInfo(new LogMessage(string.Format("EventProtocolExternalViewerCallbackAsync -> received callback with event id {0}", evt.Id)), LogCategories);

            try
            {
                ExternalViewerModel externalViewerModel = evt.ContentType.ContentTypeValue;
                SetProtocolContract setProtocolContract = new SetProtocolContract
                {
                    anno = externalViewerModel.Year.ToString(),
                    DocumentUniqueId = externalViewerModel.UniqueId.ToString(),
                    uri_protocollo = externalViewerModel.Url,
                    protocollo = string.Concat(externalViewerModel.Year.ToString(), "/", externalViewerModel.Number.ToString())
                };

                await SentIP4D(setProtocolContract, _moduleConfiguration.IP4D_SetProtocolUrl);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("EventProtocolExternalViewerCallbackAsync -> error complete call IP4D Services"), ex, LogCategories);
                throw;
            }
        }

        #endregion

    }
}
