using System;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Protocols;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Protocols
{
    public class ProtocolLogService : BaseService<ProtocolLog>, IProtocolLogService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public ProtocolLogService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IProtocolRuleset protocolRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, protocolRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override ProtocolLog BeforeCreate(ProtocolLog entity)
        {
            //TODO:  implementare nell’ISecurity la corretta gestione per loggare IP del Chiamante.
            switch (CurrentInsertActionType)
            {
                case InsertActionType.ViewProtocolDocument:
                    {
                        entity.LogDescription = $"Visualizzato documento {entity.LogDescription}";
                        CreateProtocolLog(entity, ProtocolLogEvent.PD.ToString());
                        break;
                    }
                case InsertActionType.DematerialisationLogInsert:
                case InsertActionType.SecureDocumentLogInsert:
                case InsertActionType.DocumentUnitArchived:
                    {
                        CreateProtocolLog(entity, ProtocolLogEvent.PM.ToString());
                        break;
                    }
                case InsertActionType.ProtocolShared:
                    {
                        CreateProtocolLog(entity, ProtocolLogEvent.SH.ToString());
                        break;
                    }
                default:
                    {
                        throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
                    }
            }
            return base.BeforeCreate(entity);
        }

        protected override ProtocolLog BeforeUpdate(ProtocolLog entity, ProtocolLog entityTransformed)
        {
            throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
        }

        protected override ProtocolLog BeforeDelete(ProtocolLog entity, ProtocolLog entityTransformed)
        {
            if ((CurrentDeleteActionType == DeleteActionType.DematerialisationLogDelete) || (CurrentDeleteActionType == DeleteActionType.SecureDocumentLogDelete))
            {
                return base.BeforeDelete(entity, entityTransformed);
            }

            throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
        }

        private void CreateProtocolLog(ProtocolLog log, string logType)
        {
            if (log.Entity != null)
            {
                Protocol protocol = _unitOfWork.Repository<Protocol>().Find(log.Entity.UniqueId);
                if (protocol != null)
                {
                    log.Year = protocol.Year;
                    log.Number = protocol.Number;
                    log.Entity = protocol;
                }
            }
            log.LogDate = DateTime.UtcNow;
            log.SystemComputer = Environment.MachineName;
            log.Program = "Private.WebAPI";
            log.LogType = logType;
            log.Hash = HashGenerator.GenerateHash(string.Concat(log.RegistrationUser, "|", log.Year, "|", log.Number, "|", log.LogType, "|", log.LogDescription, "|", log.UniqueId, "|", log.Entity.UniqueId, "|", log.LogDate.ToString("yyyyMMddHHmmss")));
        }

        #endregion

    }
}
