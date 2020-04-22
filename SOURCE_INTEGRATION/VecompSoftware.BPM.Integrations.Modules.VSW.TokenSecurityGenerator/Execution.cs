using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Security.Principal;
using VecompSoftware.BPM.Integrations.Modules.VSW.TokenSecurityGenerator.Configurations;
using VecompSoftware.BPM.Integrations.Modules.VSW.TokenSecurityGenerator.Models;
using VecompSoftware.BPM.Integrations.Services.ServiceBus;
using VecompSoftware.BPM.Integrations.Services.WebAPI;
using VecompSoftware.Core.Command;
using VecompSoftware.Core.Command.CQRS.Events.Models.ExternalSecurities;
using VecompSoftware.DocSuiteWeb.Common.CustomAttributes;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Model.Securities;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.TokenSecurityGenerator
{
    [Export(typeof(IModule))]
    [LogCategory(ModuleConfigurationHelper.MODULE_NAME)]
    public class Execution : ModuleBase
    {
        #region [ Fields ]
        private const string _message_attribute_module_name = "ModuleName";

        private static readonly HostIdentify _hostIdentify = new HostIdentify(Environment.MachineName, ModuleConfigurationHelper.MODULE_NAME);

        private static IEnumerable<LogCategory> _logCategories;
        private readonly ModuleConfigurationModel _moduleConfiguration;
        private readonly ILogger _logger;
        private readonly IServiceBusClient _serviceBusClient;
        private readonly IWebAPIClient _webAPIClient;
        private readonly IdentityContext _identityContext = null;
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
        public Execution(ILogger logger, IServiceBusClient serviceBusClient, IWebAPIClient webAPIClient)
            : base(logger, ModuleConfigurationHelper.MODULE_NAME)
        {
            try
            {
                _logger = logger;
                _moduleConfiguration = ModuleConfigurationHelper.GetModuleConfiguration();
                _serviceBusClient = serviceBusClient;
                _webAPIClient = webAPIClient;
                string username = "anonymous";
                if (WindowsIdentity.GetCurrent() != null)
                {
                    username = WindowsIdentity.GetCurrent().Name;
                }
                _identityContext = new IdentityContext(username);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("VSW.TokenSecurityGenerator -> Critical error in costruction module"), ex, LogCategories);
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
                Guid currentToken = Guid.NewGuid();
                DateTimeOffset creationDate = DateTimeOffset.UtcNow;
                DateTimeOffset expiryDate = creationDate.AddMilliseconds(_moduleConfiguration.MillisecondExpiryToken);
                _logger.WriteDebug(new LogMessage(string.Concat("Generated token ", currentToken, " expiry on ", expiryDate.ToString())), LogCategories);
                TokenSecurityModel tokenModel = new TokenSecurityModel()
                {
                    AuthenticationId = _moduleConfiguration.AuthenticationId,
                    ExpiryDate = expiryDate,
                    Token = currentToken,
                    WorkflowName = _moduleConfiguration.WorkflowName,
                    Host = _hostIdentify
                };
                EventTokenSecurity eventTokenSecurity = new EventTokenSecurity(_moduleConfiguration.TenantName, _moduleConfiguration.TenantId, _identityContext, tokenModel);
                eventTokenSecurity.CustomProperties.Add(ModuleConfigurationHelper.MODULE_NAME, _message_attribute_module_name);
                eventTokenSecurity = _webAPIClient.PostAsync(eventTokenSecurity).Result;
                _logger.WriteInfo(new LogMessage(string.Concat("Generated and sended new token security authentication and expiry on ", expiryDate.ToString())), LogCategories);
            }
            catch (Exception ex)
            {
                _logger.WriteError(new LogMessage("TokenSecurityGenerator -> Critical Error"), ex, LogCategories);
                throw;
            }
        }

        protected override void OnStop()
        {
            _logger.WriteInfo(new LogMessage("OnStop -> VSW.TokenSecurityGenerator"), LogCategories);
        }

        #endregion

    }
}
