using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;
using VecompSoftware.JeepService.LogConservation.Mappers;
using VecompSoftware.JeepService.LogConservation.Models;
using VecompSoftware.Helpers.UDS;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using APIEntity = VecompSoftware.DocSuiteWeb.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VecompSoftware.Helpers.Signer.Security;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.LogConservation.Services
{
    public class UDSLogConservationService : BaseConservationService
    {
        #region [ Fields ]
        private readonly LogModelMapper _logModelMapper;
        private const string UDS_ADDRESS_NAME = "API-UDSAddress";
        private readonly HashGenerator _hashGenerator;
        private readonly string _loggerName;
        #endregion

        #region [ Properties ]
        private IDictionary<APIEntity.UDS.UDSLogType, string> UDSLogDescriptions => JsonConvert.DeserializeObject<IDictionary<APIEntity.UDS.UDSLogType, string>>(Encoding.UTF8.GetString(Properties.Resources.UDSLogDescriptions));
        #endregion

        #region [ Constructor ]
        public UDSLogConservationService(string loggerName, LogConservationParameters parameters, Func<bool> cancelRequest)
            : base(loggerName, parameters, cancelRequest)
        {
            _logModelMapper = new LogModelMapper();
            _hashGenerator = new HashGenerator();
            _loggerName = loggerName;
        }
        #endregion

        #region [ Methods ]
        public override int AvailableLogs()
        {
            SetEntityODATA<Conservation>(CONSERVATION_ODATA_CONTROLLER_NAME);
            ODATAModel<int> availableLogs = WebAPIHelper.GetRawRequest<Conservation, ODATAModel<int>>(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig,
                 DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, "/ConservationService.CountAvailableUDSLogs()");
            return availableLogs.value;
        }

        public override ICollection<ConservationLogModel> GetLogs()
        {
            SetEntityODATA<Conservation>(CONSERVATION_ODATA_CONTROLLER_NAME);
            ODATAModel<ICollection<ConservationLogModel>> logs = WebAPIHelper.GetRawRequest<Conservation, ODATAModel<ICollection<ConservationLogModel>>>(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig,
                 DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, string.Concat("/ConservationService.GetAvailableUDSLogs(skip=", 0, ",top=", DocSuiteContext.Current.DefaultODataTopQuery, ")"));
            return logs.value;
        }

        public override LogModel MapToLogModel(ConservationLogModel log)
        {
            FileLogger.Debug(_loggerName, string.Concat("UDSLog - mapping log data to xml model"));
            LogModel logModel = _logModelMapper.Map(log);

            FileLogger.Info(_loggerName, string.Concat("UDSLog - get UDS reference data for Id ", log.ReferenceUniqueId));
            string controllerName = Utils.SafeSQLName(log.ReferenceEntityName);
            IBaseAddress webApiAddress = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.Addresses.FirstOrDefault(x => x.AddressName.Equals(UDS_ADDRESS_NAME, StringComparison.InvariantCultureIgnoreCase));
            WebApiControllerEndpoint udsEndpoint = new WebApiControllerEndpoint
            {
                AddressName = UDS_ADDRESS_NAME,
                ControllerName = controllerName,
                EndpointName = "UDSModel"
            };

            HttpClientConfiguration customHttpConfiguration = new HttpClientConfiguration();
            customHttpConfiguration.Addresses.Add(webApiAddress);
            customHttpConfiguration.EndPoints.Add(udsEndpoint);

            string odataFilter = string.Concat("$filter=UDSId eq ", log.ReferenceUniqueId, "&applySecurity='0'");
            string jsonSource = WebAPIHelper.GetRequest<UDSModel, string>(customHttpConfiguration, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, odataFilter);
            if (string.IsNullOrEmpty(jsonSource))
            {
                throw new Exception(string.Concat("UDSLog - UDS with Id ", log.ReferenceUniqueId, " not found"));
            }
            string parsedJson = ParseJson(jsonSource);
            UDSEntityModel entityDto = JsonConvert.DeserializeObject<IList<UDSEntityModel>>(parsedJson, DocSuiteContext.DefaultUDSJsonSerializerSettings).FirstOrDefault();
            if (entityDto == null)
            {
                throw new Exception(string.Concat("UDSLog - UDS with Id ", log.ReferenceUniqueId, " not found"));
            }

            FileLogger.Info(_loggerName, string.Concat("UDSLog - UDS ", log.ReferenceUniqueId, " found"));
            logModel.ReferenceModel = new Models.ReferenceModel()
            {
                Number = entityDto.Number,
                ReferenceUniqueId = entityDto.Id,
                Subject = entityDto.Subject,
                Year = entityDto.Year
            };
            return logModel;
        }

        private string ParseJson(string udsJson)
        {
            JToken rootToken = JToken.Parse(udsJson);
            if (rootToken == null)
            {
                throw new Exception("UDSLog - deserialization error");
            }

            IDictionary<string, JToken> rootProperties = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(udsJson, DocSuiteContext.DefaultUDSJsonSerializerSettings);
            if (rootProperties.Count == 0)
            {
                throw new Exception("UDSLog - no items found on json result");
            }

            return rootProperties.First().Value.ToString();
        }

        public override string GetLogTypeDescription(LogModel logModel)
        {
            APIEntity.UDS.UDSLogType logType = (APIEntity.UDS.UDSLogType)Enum.Parse(typeof(APIEntity.UDS.UDSLogType), logModel.LogType);
            return UDSLogDescriptions[logType];
        }

        public override bool HashIsValid(ConservationLogModel logModel)
        {            
            string toHash = string.Concat(logModel.RegistrationUser, "|", logModel.LogType, "|", logModel.Description, "|", logModel.Id, "|", logModel.ReferenceUniqueId, "|", logModel.LogDate.Value.ToString("yyyyMMddHHmmss"));
            string hashStringify = _hashGenerator.GenerateHash(toHash);
            FileLogger.Debug(_loggerName, string.Concat(LogEntityNameDefinition.UDSLOG_NAME, " - runtime calculated hash: ", hashStringify));
            FileLogger.Debug(_loggerName, string.Concat(LogEntityNameDefinition.UDSLOG_NAME, " - log hash to validate: ", logModel.Hash));
            FileLogger.Debug(_loggerName, string.Concat(LogEntityNameDefinition.UDSLOG_NAME, " - log hash ", hashStringify.Equals(logModel.Hash) ? " is valid" : " is not valid"));
            return hashStringify.Equals(logModel.Hash);
        }
        #endregion
    }
}
