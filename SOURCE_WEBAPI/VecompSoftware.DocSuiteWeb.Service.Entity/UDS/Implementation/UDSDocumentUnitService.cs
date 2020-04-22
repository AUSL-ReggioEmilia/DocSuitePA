using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Service.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSDocumentUnitService : BaseService<UDSDocumentUnit>, IUDSDocumentUnitService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly IProtocolLogService _protocolLogService;
        private readonly IDictionary<UDSRelationType, Action<UDSDocumentUnit>> _relationLogActions;
        #endregion

        #region [ Constructor ]

        public UDSDocumentUnitService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IUDSRuleset udsRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security, IProtocolLogService protocolLogService)
            : base(unitOfWork, logger, validationService, udsRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _protocolLogService = protocolLogService;

            _relationLogActions = new Dictionary<UDSRelationType, Action<UDSDocumentUnit>>
            {
                { UDSRelationType.Protocol, CreateProtocolRelationLog },
                { UDSRelationType.ArchiveProtocol, CreateProtocolRelationLog },
                { UDSRelationType.ProtocolArchived, CreateProtocolRelationLog }
            };
        }

        #endregion

        #region [ Methods ]
        private void CreateProtocolRelationLog(UDSDocumentUnit udsDocumentUnit)
        {
            Protocol protocol = _unitOfWork.Repository<Protocol>().Find(udsDocumentUnit.Relation.UniqueId);
            ProtocolLog protocolLog = new ProtocolLog()
            {
                Year = protocol.Year,
                Number = protocol.Number,
                LogDate = DateTime.UtcNow,
                SystemComputer = Environment.MachineName,
                Program = "Private.WebAPI",
                LogType = "PM",
                LogDescription = string.Format("Protocollo collegato ad archivio {0} con ID {1}", udsDocumentUnit.Repository.Name, udsDocumentUnit.IdUDS),
                Entity = protocol
            };
            protocolLog.Hash = HashGenerator.GenerateHash(string.Concat(protocolLog.RegistrationUser, "|", protocolLog.Year, "|", protocolLog.Number, "|", protocolLog.LogType, "|", protocolLog.LogDescription, "|", protocolLog.UniqueId, "|", protocolLog.Entity.UniqueId, "|", protocolLog.LogDate.ToString("yyyyMMddHHmmss")));
            _unitOfWork.Repository<ProtocolLog>().Insert(protocolLog);
        }

        protected override UDSDocumentUnit BeforeCreate(UDSDocumentUnit entity)
        {
            if (entity.Repository != null)
            {
                entity.Repository = _unitOfWork.Repository<UDSRepository>().Find(entity.Repository.UniqueId);
            }
            if (entity.Relation != null)
            {
                entity.Relation = _unitOfWork.Repository<DocumentUnit>().Find(entity.Relation.UniqueId);
            }
            if (entity.Repository != null && entity.Relation != null)
            {
                if (_relationLogActions.ContainsKey(entity.RelationType))
                {
                    _relationLogActions[entity.RelationType](entity);
                }
            }
            return base.BeforeCreate(entity);
        }

        protected override UDSDocumentUnit BeforeUpdate(UDSDocumentUnit entity, UDSDocumentUnit entityTransformed)
        {
            if (entity.Repository != null)
            {
                entityTransformed.Repository = _unitOfWork.Repository<UDSRepository>().Find(entity.Repository.UniqueId);
            }
            if (entity.Relation != null)
            {
                entityTransformed.Relation = _unitOfWork.Repository<DocumentUnit>().Find(entity.Relation.UniqueId);
            }
            if (entity.Repository != null && entity.Relation != null)
            {
                if (_relationLogActions.ContainsKey(entity.RelationType))
                {
                    _relationLogActions[entity.RelationType](entity);
                }
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion

    }
}
