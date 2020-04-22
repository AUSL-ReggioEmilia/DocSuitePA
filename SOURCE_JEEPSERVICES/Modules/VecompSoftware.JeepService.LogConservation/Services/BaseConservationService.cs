using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Conservations;
using VecompSoftware.DocSuiteWeb.Model.Entities.Conservations;
using VecompSoftware.DocSuiteWeb.Model.WebAPI.Client;
using VecompSoftware.Helpers;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.Helpers.WebAPI;
using VecompSoftware.JeepService.LogConservation.Mappers;
using VecompSoftware.JeepService.LogConservation.Models;
using VecompSoftware.Services.Biblos;
using VecompSoftware.Services.Biblos.Models;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.LogConservation.Services
{
    public abstract class BaseConservationService : IService
    {
        #region [ Fields ]
        private readonly string _loggerName;
        private readonly Func<bool> _cancelRequest;
        private readonly LogConservationParameters _parameters;
        private readonly WebAPIHelper _webAPIHelper;
        private readonly ConservationMapper _conservationMapper;
        private readonly LogModelMapper _logModelMapper;
        public const string CONSERVATION_ODATA_CONTROLLER_NAME = "Conservations";
        public const string PROTOCOL_ODATA_CONTROLLER_NAME = "Protocols";
        public const string DOCUMENTSERIESITEM_ODATA_CONTROLLER_NAME = "DocumentSeriesItems";
        public const string DOSSIER_ODATA_CONTROLLER_NAME = "Dossiers";
        public const string FASCICLE_ODATA_CONTROLLER_NAME = "Fascicles";
        public const string PECMAIL_ODATA_CONTROLLER_NAME = "PECMails";
        private const string ODATA_ADDRESS_NAME = "ODATA-EntityAddress";
        private const string REST_ADDRESS_NAME = "API-EntityAddress";

        private static CultureInfo _defaultCulture = new CultureInfo("it-IT") { DateTimeFormat = new DateTimeFormatInfo { ShortDatePattern = "dd/MM/yyyy" } };

        #region [ Biblos attributes ]
        public const string BIBLOS_UNIQUEID_ATTRIBUTE_NAME = "Identificativo";
        public const string BIBLOS_LOGDATE_ATTRIBUTE_NAME = "Data";
        public const string BIBLOS_EVENT_ATTRIBUTE_NAME = "Evento";
        public const string BIBLOS_LOGDESCRIPTION_ATTRIBUTE_NAME = "Descrizione";
        public const string BIBLOS_HASH_ATTRIBUTE_NAME = "Hash";
        public const string BIBLOS_TYPOLOGY_ATTRIBUTE_NAME = "Tipologia";
        public const string BIBLOS_REFERENCEUNIQUEID_ATTRIBUTE_NAME = "IdRiferimento";
        public const string BIBLOS_REFERENCEENTITYNAME_ATTRIBUTE_NAME = "Riferimento";
        public const string BIBLOS_REFERENCEENTITYYEAR_ATTRIBUTE_NAME = "AnnoRiferimento";
        public const string BIBLOS_REFERENCEENTITYNUMBER_ATTRIBUTE_NAME = "NumeroRiferimento";
        public const string BIBLOS_REFERENCEENTITYOBJECT_ATTRIBUTE_NAME = "OggettoRiferimento";
        public const string BIBLOS_REGISTRATIONDATE_ATTRIBUTE_NAME = "DataVersamento";
        #endregion

        #endregion

        #region [ Properties ]
        protected WebAPIHelper WebAPIHelper => _webAPIHelper;
        #endregion

        #region [ Constructor ]
        public BaseConservationService(string loggerName, LogConservationParameters parameters, Func<bool> cancelRequest)
        {
            _loggerName = loggerName;
            _cancelRequest = cancelRequest;
            _parameters = parameters;
            _conservationMapper = new ConservationMapper();
            _logModelMapper = new LogModelMapper();
            _webAPIHelper = new WebAPIHelper();
        }
        #endregion

        #region [ Methods ]
        public abstract int AvailableLogs();

        public abstract ICollection<ConservationLogModel> GetLogs();

        public abstract string GetLogTypeDescription(LogModel logModel);

        public abstract bool HashIsValid(ConservationLogModel logModel);

        public virtual LogModel MapToLogModel(ConservationLogModel log)
        {
            FileLogger.Debug(_loggerName, string.Concat(log.EntityName, " - mapping log data to xml model"));
            return _logModelMapper.Map(log);
        }

        public ConservateResultModel DoConservate()
        {
            FileLogger.Info(_loggerName, "Reading logs to conservate...");
            ConservateResultModel resultModel = new ConservateResultModel();
            int totalLogToConservate = AvailableLogs();
            FileLogger.Info(_loggerName, string.Concat("Found ", totalLogToConservate, " to conservate"));
            if (totalLogToConservate == 0)
            {
                return resultModel;
            }

            ICollection<ConservationLogModel> logs = GetLogs();
            FileLogger.Info(_loggerName, string.Concat("Process ", logs.Count, " items for current thread"));
            if (logs == null || logs.Count == 0)
            {
                FileLogger.Info(_loggerName, "No log found to conservate");
                return resultModel;
            }

            LogModel logModel;
            string serializedModel;
            Guid? chainId;
            bool hashIsValid;
            foreach (ConservationLogModel log in logs)
            {
                serializedModel = string.Empty;
                chainId = null;
                hashIsValid = false;
                try
                {
                    if (_cancelRequest())
                    {
                        FileLogger.Debug(_loggerName, "Process canceled by user");
                        return resultModel;
                    }
                    FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - process log ", log.Id));
                    FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - check hash validation for log ", log.Id));
                    hashIsValid = HashIsValid(log);
                    if (!hashIsValid)
                    {
                        throw new Exception(string.Concat(log.EntityName, " - error on validating log ", log.Id, ". Hash value not match"));
                    }
                    FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - log ", log.Id, " is valid"));
                    FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - creating conservation document..."));
                    logModel = MapToLogModel(log);
                    FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - read log type description for logtype ", log.LogType));
                    logModel.LogType = GetLogTypeDescription(logModel);
                    FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - log type description for logtype ", log.LogType, " is ", logModel.LogType));

                    serializedModel = SerializationHelper.SerializeToStringWithoutNamespace(logModel);
                    FileLogger.Debug(_loggerName, string.Concat(log.EntityName, " - serialized content: ", serializedModel));
                    using (MemoryDocumentInfo documentInfo = new MemoryDocumentInfo(Encoding.UTF8.GetBytes(serializedModel), string.Concat("log_", log.Id, ".xml")))
                    {
                        BiblosChainInfo chainInfo = new BiblosChainInfo(new List<DocumentInfo>() { documentInfo });
                        documentInfo.AddAttribute(BIBLOS_UNIQUEID_ATTRIBUTE_NAME, log.Id.ToString());
                        documentInfo.AddAttribute(BIBLOS_LOGDATE_ATTRIBUTE_NAME, logModel.RegistrationDate.Value.ToString(_defaultCulture));
                        documentInfo.AddAttribute(BIBLOS_LOGDESCRIPTION_ATTRIBUTE_NAME, logModel.Description?.Truncate(8000));
                        documentInfo.AddAttribute(BIBLOS_EVENT_ATTRIBUTE_NAME, logModel.LogType);
                        documentInfo.AddAttribute(BIBLOS_TYPOLOGY_ATTRIBUTE_NAME, log.EntityName);
                        documentInfo.AddAttribute(BIBLOS_HASH_ATTRIBUTE_NAME, logModel.Hash);
                        documentInfo.AddAttribute(BIBLOS_REFERENCEENTITYNAME_ATTRIBUTE_NAME, logModel.EntityName);
                        documentInfo.AddAttribute(BIBLOS_REFERENCEUNIQUEID_ATTRIBUTE_NAME, logModel.ReferenceModel.ReferenceUniqueId.ToString());
                        if (logModel.ReferenceModel.Year.HasValue)
                        {
                            documentInfo.AddAttribute(BIBLOS_REFERENCEENTITYYEAR_ATTRIBUTE_NAME, logModel.ReferenceModel.Year.ToString());
                        }

                        if (logModel.ReferenceModel.Number.HasValue)
                        {
                            documentInfo.AddAttribute(BIBLOS_REFERENCEENTITYNUMBER_ATTRIBUTE_NAME, logModel.ReferenceModel.Number.ToString());
                        }
                        documentInfo.AddAttribute(BIBLOS_REFERENCEENTITYOBJECT_ATTRIBUTE_NAME, logModel.ReferenceModel.Subject?.Truncate(8000));
                        documentInfo.AddAttribute(BIBLOS_REGISTRATIONDATE_ATTRIBUTE_NAME, DateTime.UtcNow.ToString(_defaultCulture));

                        FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - saving ", documentInfo.Name, " to biblos..."));
                        chainId = chainInfo.ArchiveInBiblos(_parameters.LogConservationLocation.DocumentServer, _parameters.LogConservationLocation.ProtBiblosDSDB);
                        FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - document ", documentInfo.Name, " saved with chain id ", chainId));
                    }

                    FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - log ", log.Id, " save conservated status"));
                    UpdateConservationEntity(log, string.Empty, chainId, ConservationStatus.Conservated);
                    FileLogger.Info(_loggerName, string.Concat(log.EntityName, " - log ", log.Id, " conservated correctly"));
                }
                catch (Exception ex)
                {
                    FileLogger.Error(_loggerName, string.Concat(log.EntityName, " - error on conservate log ", log.ReferenceUniqueId), ex);
                    resultModel.Errors.Add(string.Concat("E' avvenuto un errore durante la fase di conservazione del log ", log.ReferenceUniqueId, " per l'entità ", log.EntityName, ". Errore: ", ex.Message, ". StackTrace: ", ex.StackTrace));
                    try
                    {
                        UpdateConservationEntity(log, ex.Message, ConservationStatus.Error);
                    }
                    catch (Exception exx)
                    {
                        FileLogger.Error(_loggerName, "Error on update error conservation status log", exx);
                    }
                }
            }
            return resultModel;
        }

        private void UpdateConservationEntity(ConservationLogModel log, string message, ConservationStatus status)
        {
            UpdateConservationEntity(log, message, null, status);
        }

        private void UpdateConservationEntity(ConservationLogModel log, string message, Guid? chainId, ConservationStatus status)
        {
            SetEntityODATA<Conservation>(CONSERVATION_ODATA_CONTROLLER_NAME);
            ODATAModel<ICollection<Conservation>> odataResult = _webAPIHelper.GetRequest<Conservation, ODATAModel<ICollection<Conservation>>>(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig,
                 DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, string.Concat("$filter=UniqueId eq ", log.ReferenceUniqueId));
            bool sendUpdate = odataResult.value != null && odataResult.value.Count > 0;
            Conservation conservation = new Conservation();
            if (sendUpdate)
            {
                conservation = odataResult.value.First();
            }
            conservation = _conservationMapper.Map(conservation, log);
            conservation.Message = message;
            conservation.Status = status;
            if (chainId.HasValue)
            {
                conservation.Uri = chainId.ToString();
            }

            SetEntityRest<Conservation>();
            if (sendUpdate)
            {
                _webAPIHelper.SendUpdateRequest<Conservation>(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, conservation);
            }
            else
            {
                _webAPIHelper.SendRequest<Conservation>(DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, DocSuiteContext.Current.CurrentTenant.WebApiClientConfig, conservation);
            }
        }

        protected void SetEntityRest<TEntity>()
        {
            string entityName = typeof(TEntity).Name;
            IWebApiControllerEndpoint controller = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.EndPoints.Single(f => f.EndpointName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            controller.AddressName = REST_ADDRESS_NAME;
            controller.ControllerName = entityName;
        }

        protected void SetEntityODATA<TEntity>(string controllerName)
        {
            string entityName = typeof(TEntity).Name;
            IWebApiControllerEndpoint controller = DocSuiteContext.Current.CurrentTenant.WebApiClientConfig.EndPoints.Single(f => f.EndpointName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
            controller.AddressName = ODATA_ADDRESS_NAME;
            controller.ControllerName = controllerName;
        }
        #endregion
    }
}
