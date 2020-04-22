using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Model.Parameters;

namespace VecompSoftware.DocSuite.WebAPI.Common.Configurations
{
    public static class WebApiConfiguration
    {
        #region [ Fields ]
        private static string _serviceBusConnectionString = string.Empty;
        private static string _configurationName = string.Empty;
        private static string _workflowEngineType = string.Empty;
        private static bool? _corsAllow = null;
        private static ICollection<string> _corsLists = null;
        public const string MESSAGE_CONFIGURATION_FILE_PATH = "~/App_Data/ConfigurationFiles/MessageConfiguration.json";
        public const string CORS_FILE_PATH = "~/App_Data/ConfigurationFiles/WebAPI.Origin.Allow.Lists.json";
        private static SecurityContextType? _securityContextType = null;
        private static short? _onlinePublicationInterval = null;
        private static string _customInstanceName = string.Empty;
        private static TimeSpan? _autoDeleteOnIdle = null;
        private static TimeSpan? _defaultMessageTimeToLive = null;
        private static TimeSpan? _lockDuration = null;
        private static int? _maxDeliveryCount = null;
        #endregion

        #region [ Properties ]
        public static string ServiceBusConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_serviceBusConnectionString))
                {
                    _serviceBusConnectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
                }
                return _serviceBusConnectionString;
            }
        }

        public static string ConfigurationName
        {
            get
            {
                if (string.IsNullOrEmpty(_configurationName))
                {
                    _configurationName = ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.Finder.ConfigurationName"];
                }
                return _configurationName;
            }
        }

        public static string WorkflowEngineType
        {
            get
            {
                if (string.IsNullOrEmpty(_workflowEngineType))
                {
                    _workflowEngineType = ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.Service.Workflow.EngineType"];
                }
                return _workflowEngineType;
            }
        }

        public static bool AllowCrossOrigin
        {
            get
            {
                if (!_corsAllow.HasValue)
                {
                    _corsAllow = "ALL".Equals(ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.AllowCrossOrigin"]);
                }
                return _corsAllow.Value;
            }
        }

        public static ICollection<string> CrossOriginLists
        {
            get
            {
                if (_corsLists == null)
                {
                    _corsLists = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(HostingEnvironment.MapPath(CORS_FILE_PATH)));
                }
                return _corsLists;
            }
        }

        public static SecurityContextType SecurityContextType
        {
            get
            {
                if (!_securityContextType.HasValue)
                {
                    SecurityContextType tmp;
                    if (!Enum.TryParse(ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.Security.ContextType"], out tmp))
                    {
                        throw new DSWException("Parameter VecompSoftware.DocSuiteWeb.Security.ContextType doesn't have a valid enum format value", null, DSWExceptionCode.SS_RulesetValidation);
                    }
                    _securityContextType = tmp;
                }
                return _securityContextType.Value;
            }
        }

        public static short OnlinePublicationInterval
        {
            get
            {
                if (!_onlinePublicationInterval.HasValue)
                {
                    short onlinePublicationInterval = 15;
                    if (!short.TryParse(ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.OnlinePublicationInterval"], out onlinePublicationInterval))
                    {
                        throw new DSWException("Parameter VecompSoftware.DocSuiteWeb.OnlinePublicationInterval doesn't have a valid short format value", null, DSWExceptionCode.SS_RulesetValidation);
                    }
                    _onlinePublicationInterval = onlinePublicationInterval;
                }
                return _onlinePublicationInterval.Value;
            }
        }

        public static string CustomInstanceName
        {
            get
            {
                if (string.IsNullOrEmpty(_customInstanceName))
                {
                    _customInstanceName = ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.CustomInstanceName"];
                }
                return _customInstanceName;
            }
        }

        public static TimeSpan AutoDeleteOnIdle
        {
            get
            {
                if (!_autoDeleteOnIdle.HasValue)
                {
                    double autoDeleteOnIdle = 0;
                    if (!double.TryParse(ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.AutoDeleteOnIdle"], out autoDeleteOnIdle))
                    {
                        throw new DSWException("Parameter VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.AutoDeleteOnIdle doesn't have a valid timespan format value", null, DSWExceptionCode.SS_RulesetValidation);
                    }
                    _autoDeleteOnIdle = TimeSpan.FromSeconds(autoDeleteOnIdle);
                }
                return _autoDeleteOnIdle.Value;
            }
        }

        public static TimeSpan DefaultMessageTimeToLive
        {
            get
            {
                if (!_defaultMessageTimeToLive.HasValue)
                {
                    double defaultMessageTimeToLive = 0;
                    if (!double.TryParse(ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.DefaultMessageTimeToLive"], out defaultMessageTimeToLive))
                    {
                        throw new DSWException("Parameter VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.DefaultMessageTimeToLive doesn't have a valid timespan format value", null, DSWExceptionCode.SS_RulesetValidation);
                    }
                    _defaultMessageTimeToLive = TimeSpan.FromSeconds(defaultMessageTimeToLive);
                }
                return _defaultMessageTimeToLive.Value;
            }
        }
        public static TimeSpan LockDuration
        {
            get
            {
                if (!_lockDuration.HasValue)
                {
                    double lockDuration = 0;
                    if (!double.TryParse(ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.LockDuration"], out lockDuration))
                    {
                        throw new DSWException("Parameter VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.LockDuration doesn't have a valid timespan format value", null, DSWExceptionCode.SS_RulesetValidation);
                    }
                    _lockDuration = TimeSpan.FromSeconds(lockDuration);
                }
                return _lockDuration.Value;
            }
        }
        public static int MaxDeliveryCount
        {
            get
            {
                if (!_maxDeliveryCount.HasValue)
                {
                    int maxDeliveryCount = 0;
                    if (!int.TryParse(ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.MaxDeliveryCount"], out maxDeliveryCount))
                    {
                        throw new DSWException("Parameter VecompSoftware.DocSuiteWeb.ServiceBus.Subscription.MaxDeliveryCount doesn't have a valid int format value", null, DSWExceptionCode.SS_RulesetValidation);
                    }
                    _maxDeliveryCount = maxDeliveryCount;
                }
                return _maxDeliveryCount.Value;
            }
        }
        #endregion
    }
}