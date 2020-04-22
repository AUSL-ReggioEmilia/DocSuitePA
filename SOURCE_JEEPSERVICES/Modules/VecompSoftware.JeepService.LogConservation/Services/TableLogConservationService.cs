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
using System.Linq;

namespace VecompSoftware.JeepService.LogConservation.Services
{
    public class TableLogConservationService : BaseConservationService
    {
        #region [ Fields ]
        private readonly HashGenerator _hashGenerator;
        private readonly string _loggerName;
        #endregion

        #region [ Properties ]
        private IDictionary<APIEntity.Commons.TableLogEvent, string> TableLogDescriptions => JsonConvert.DeserializeObject<IDictionary<APIEntity.Commons.TableLogEvent, string>>(Encoding.UTF8.GetString(Properties.Resources.TableLogDescriptions));
        #endregion

        #region [ Constructor ]
        public TableLogConservationService(string loggerName, LogConservationParameters parameters, Func<bool> cancelRequest)
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
                 DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, "/ConservationService.CountAvailableTableLogs()");
            return availableLogs.value;
        }

        public override ICollection<ConservationLogModel> GetLogs()
        {
            SetEntityODATA<Conservation>(CONSERVATION_ODATA_CONTROLLER_NAME);
            ODATAModel<ICollection<ConservationLogModel>> logs = WebAPIHelper.GetRawRequest<Conservation, ODATAModel<ICollection<ConservationLogModel>>>(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig,
                 DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, string.Concat("/ConservationService.GetAvailableTableLogs(skip=", 0, ",top=", DocSuiteContext.Current.DefaultODataTopQuery, ")"));
            return logs.value;
        }
        
        public override string GetLogTypeDescription(LogModel logModel)
        {
            APIEntity.Commons.TableLogEvent logType = (APIEntity.Commons.TableLogEvent)Enum.Parse(typeof(APIEntity.Commons.TableLogEvent), logModel.LogType);
            return TableLogDescriptions[logType];
        }

        public override bool HashIsValid(ConservationLogModel logModel)
        {
            int separatorIndex = logModel.Description.IndexOf('-') + 1;
            string realLogDescription = logModel.Description.Substring(separatorIndex, logModel.Description.Length - separatorIndex);
            string toHash = string.Concat(logModel.RegistrationUser, "|", logModel.LogType, "|", realLogDescription.TrimStart(), "|", logModel.Id, "|", logModel.LogDate.Value.ToString("yyyyMMddHHmmss"));
            string hashStringify = _hashGenerator.GenerateHash(toHash);
            FileLogger.Debug(_loggerName, string.Concat(LogEntityNameDefinition.TABLELOG_NAME, " - runtime calculated hash: ", hashStringify));
            FileLogger.Debug(_loggerName, string.Concat(LogEntityNameDefinition.TABLELOG_NAME, " - log hash to validate: ", logModel.Hash));
            FileLogger.Debug(_loggerName, string.Concat(LogEntityNameDefinition.TABLELOG_NAME, " - log hash ", hashStringify.Equals(logModel.Hash) ? " is valid" : " is not valid"));
            return hashStringify.Equals(logModel.Hash);
        }
        #endregion
    }
}
