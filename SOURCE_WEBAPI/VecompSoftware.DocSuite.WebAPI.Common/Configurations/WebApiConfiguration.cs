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
		private static bool? _corsAllow = null;
		private static bool? _shibbolethEnabled = null;
		private static ICollection<string> _corsLists = null;
		public const string MESSAGE_CONFIGURATION_FILE_PATH = "~/App_Data/ConfigurationFiles/MessageConfiguration.json";
		public const string CORS_FILE_PATH = "~/App_Data/ConfigurationFiles/WebAPI.Origin.Allow.Lists.json";
		public const string DOCUMENTPROXY_PEM_FILE_PATH = "~/App_Data/ConfigurationFiles/cert.pem";
		private static SecurityContextType? _securityContextType = null;
		private static short? _onlinePublicationInterval = null;
		private static string _customInstanceName = string.Empty;
		private static string _passwordEncryptionKey = string.Empty;
		private static TimeSpan? _autoDeleteOnIdle = null;
		private static TimeSpan? _defaultMessageTimeToLive = null;
		private static TimeSpan? _lockDuration = null;
		private static int? _maxDeliveryCount = null;
		private static bool? _azureEnabled = null;
		private static string _azureClientId = null;
		private static string _azureTenantId = null;
		private static string _azureInstance = null;
		private static string _documentProxyGrpcEndpoint = null;
		private static string _documentProxyIdentityAccount = null;
		private static Guid? _documentProxyIdentityUniqueId = null;
		private static string _documentProxyPEMCertificate = null;
		private static int? _signalRMaxIncomingWebSocketMessageSize = null;
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

		public static string PasswordEncryptionKey
		{
			get
			{
				if (string.IsNullOrEmpty(_passwordEncryptionKey))
				{
					_passwordEncryptionKey = ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.PasswordEncryptionKey"];
				}
				return _passwordEncryptionKey;
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

		public static bool ShibbolethEnabled
		{
			get
			{
				if (!_shibbolethEnabled.HasValue)
				{
					_shibbolethEnabled = bool.TrueString.Equals(ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.ShibbolethEnabled"], StringComparison.InvariantCultureIgnoreCase);
				}
				return _shibbolethEnabled.Value;
			}
		}

		public static bool AzureEnabled
		{
			get
			{
				if (!_azureEnabled.HasValue)
				{
					_azureEnabled = bool.TrueString.Equals(ConfigurationManager.AppSettings["AzureAD.Enabled"], StringComparison.InvariantCultureIgnoreCase);
				}
				return _azureEnabled.Value;
			}
		}

		public static string AzureInstance
		{
			get
			{
				if (string.IsNullOrEmpty(_azureInstance))
				{
					_azureInstance = ConfigurationManager.AppSettings["AzureAD.Instance"];
				}
				return _azureInstance;
			}
		}

		public static string AzureTenantId
		{
			get
			{
				if (string.IsNullOrEmpty(_azureTenantId))
				{
					_azureTenantId = ConfigurationManager.AppSettings["AzureAD.TenantId"];
				}
				return _azureTenantId;
			}
		}

		public static string AzureClientId
		{
			get
			{
				if (string.IsNullOrEmpty(_azureClientId))
				{
					_azureClientId = ConfigurationManager.AppSettings["AzureAD.ClientId"];
				}
				return _azureClientId;
			}
		}

		public static string DocumentProxyGrpcEndpoint
		{
			get
			{
				if (string.IsNullOrEmpty(_documentProxyGrpcEndpoint))
				{
					_documentProxyGrpcEndpoint = ConfigurationManager.AppSettings["DocSuiteNext.DocumentProxy.GrpcEndpoint"];
				}
				return _documentProxyGrpcEndpoint;
			}
		}

		public static string DocumentProxyIdentityAccount
		{
			get
			{
				if (string.IsNullOrEmpty(_documentProxyIdentityAccount))
				{
					_documentProxyIdentityAccount = ConfigurationManager.AppSettings["DocSuiteNext.DocumentProxy.IdentityAccount"];
				}
				return _documentProxyIdentityAccount;
			}
		}

        public static Guid? DocumentProxyIdentityUniqueId
        {
            get
            {
                if (!_documentProxyIdentityUniqueId.HasValue)
                {
                    if (Guid.TryParse(ConfigurationManager.AppSettings["DocSuiteNext.DocumentProxy.IdentityUniqueId"], out Guid documentProxyIdentityUniqueId))
                    {
                        _documentProxyIdentityUniqueId = documentProxyIdentityUniqueId;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(DocumentProxyIdentityAccount))
                        {
                            throw new Exception($"DocSuiteNext.DocumentProxy.IdentityUniqueId is null or invalid {ConfigurationManager.AppSettings["DocSuiteNext.DocumentProxy.IdentityUniqueId"]} but must be defined becasuse DocSuiteNext.DocumentProxy.IdentityAccount {ConfigurationManager.AppSettings["DocSuiteNext.DocumentProxy.IdentityAccount"]} is defined.");
                        }
                    }
                }
                return _documentProxyIdentityUniqueId;
            }
        }

		public static string DocumentProxyPEMCertificate
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_documentProxyPEMCertificate))
				{
					if (File.Exists(HostingEnvironment.MapPath(DOCUMENTPROXY_PEM_FILE_PATH)))
					{
						_documentProxyPEMCertificate = File.ReadAllText(HostingEnvironment.MapPath(DOCUMENTPROXY_PEM_FILE_PATH));
					}
				}
				return _documentProxyPEMCertificate;
			}
		}

		public static int? SignalRMaxIncomingWebSocketMessageSize
		{
			get
			{
				if (!_signalRMaxIncomingWebSocketMessageSize.HasValue)
				{
					if (int.TryParse(ConfigurationManager.AppSettings["VecompSoftware.DocSuiteWeb.SignalR.MaxIncomingWebSocketMessageSize"], out int maxIncomingWebSocketMessageSize))
					{
						_signalRMaxIncomingWebSocketMessageSize = maxIncomingWebSocketMessageSize;
					}
				}
				return _signalRMaxIncomingWebSocketMessageSize;
			}
		}
		#endregion
	}
}