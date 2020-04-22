using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;
using VecompSoftware.Helpers.Signer.Security;
using VecompSoftware.JeepService.LogConservation.Models;
using VecompSoftware.Services.Logging;
using APIEntity = VecompSoftware.DocSuiteWeb.Entity;

namespace VecompSoftware.JeepService.LogConservation.Services
{
    public class DossierLogConservationService : BaseConservationService
    {
        #region [ Fields ]
        private readonly HashGenerator _hashGenerator;
        private readonly string _loggerName;
        #endregion

        #region [ Properties ]
        private IDictionary<APIEntity.Dossiers.DossierLogType, string> DossierLogDescriptions => JsonConvert.DeserializeObject<IDictionary<APIEntity.Dossiers.DossierLogType, string>>(Encoding.UTF8.GetString(Properties.Resources.DossierLogDescriptions));
        #endregion

        #region [ Constructor ]
        public DossierLogConservationService(string loggerName, LogConservationParameters parameters, Func<bool> cancelRequest)
            : base(loggerName, parameters, cancelRequest)
        {
            _hashGenerator = new HashGenerator();
            _loggerName = loggerName;
        }
        #endregion

        #region [ Methods ]
        public override int AvailableLogs()
        {
            SetEntityODATA<Conservation>(CONSERVATION_ODATA_CONTROLLER_NAME);
            ODATAModel<int> availableLogs = WebAPIHelper.GetRawRequest<Conservation, ODATAModel<int>>(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig,
                 DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, "/ConservationService.CountAvailableDossierLogs()");
            return availableLogs.value;
        }

        public override ICollection<ConservationLogModel> GetLogs()
        {
            SetEntityODATA<Conservation>(CONSERVATION_ODATA_CONTROLLER_NAME);
            ODATAModel<ICollection<ConservationLogModel>> logs = WebAPIHelper.GetRawRequest<Conservation, ODATAModel<ICollection<ConservationLogModel>>>(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig,
                 DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, string.Concat("/ConservationService.GetAvailableDossierLogs(skip=", 0, ",top=", DocSuiteContext.Current.DefaultODataTopQuery, ")"));
            return logs.value;
        }
        
        public override string GetLogTypeDescription(LogModel logModel)
        {
            APIEntity.Dossiers.DossierLogType logType = (APIEntity.Dossiers.DossierLogType)Enum.Parse(typeof(APIEntity.Dossiers.DossierLogType), logModel.LogType);
            return DossierLogDescriptions[logType];
        }

        public override bool HashIsValid(ConservationLogModel logModel)
        {
            string toHash = string.Concat(logModel.RegistrationUser, "|", logModel.LogType, "|", logModel.Description, "|", logModel.Id, "|", logModel.ReferenceUniqueId, "|", logModel.LogDate.Value.ToString("yyyyMMddHHmmss"));
            string hashStringify = _hashGenerator.GenerateHash(toHash);
            FileLogger.Debug(_loggerName, string.Concat(LogEntityNameDefinition.DOSSIERLOG_NAME, " - runtime calculated hash: ", hashStringify));
            FileLogger.Debug(_loggerName, string.Concat(LogEntityNameDefinition.DOSSIERLOG_NAME, " - log hash to validate: ", logModel.Hash));
            FileLogger.Debug(_loggerName, string.Concat(LogEntityNameDefinition.DOSSIERLOG_NAME, " - log hash ", hashStringify.Equals(logModel.Hash) ? " is valid" : " is not valid"));
            return hashStringify.Equals(logModel.Hash);
        }
        #endregion
    }
}
