using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.PECMails;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.PECMails;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.PECMails;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.PECMails
{
    public class PECMailService : BaseService<PECMail>, IPECMailService
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;

        #endregion

        #region [ Constructor ]

        public PECMailService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IPECMailRuleset PECMailRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, PECMailRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        public void CreatPECMailLog(PECMail pecMail, string logType, string logDescription)
        {
            PECMailLog pecMailLog = new PECMailLog()
            {
                LogDate = DateTime.UtcNow,
                LogType = logType,
                Description = logDescription,
                SystemComputer = Environment.MachineName,
                PECMail = pecMail,
                RegistrationUser = CurrentDomainUser.Account,
            };
            pecMailLog.Hash = HashGenerator.GenerateHash(string.Concat(pecMailLog.RegistrationUser, "|", pecMailLog.SystemComputer, "|", pecMailLog.LogType, "|", pecMailLog.Description, "|", pecMailLog.UniqueId, "|", pecMailLog.PECMail.EntityId, "|", pecMailLog.LogDate.ToString("yyyyMMddHHmmss")));
            _unitOfWork.Repository<PECMailLog>().Insert(pecMailLog);
        }

        protected override PECMail BeforeCreate(PECMail entity)
        {
            if (entity.Attachments != null && entity.Attachments.Count() > 0)
            {
                foreach (PECMailAttachment item in entity.Attachments)
                {
                    item.PECMail = entity;
                }
                _unitOfWork.Repository<PECMailAttachment>().InsertRange(entity.Attachments);
            }
            if (entity.PECMailBox != null)
            {
                entity.PECMailBox = _unitOfWork.Repository<PECMailBox>().Find(entity.PECMailBox.EntityShortId);
            }
            if (entity.Location != null)
            {
                entity.Location = _unitOfWork.Repository<Location>().Find(entity.Location.EntityShortId);
            }
            return base.BeforeCreate(entity);
        }

        protected override bool ExecuteMappingBeforeUpdate()
        {
            return CurrentUpdateActionType != UpdateActionType.PECMailManaged && CurrentUpdateActionType != UpdateActionType.PECMailInvoiceTenantCorrection;
        }

        protected override PECMail BeforeUpdate(PECMail entity, PECMail entityTransformed)
        {
            if (CurrentUpdateActionType == UpdateActionType.PECMailManaged)
            {
                entityTransformed.Year = entity.Year;
                entityTransformed.Number = entity.Number;
                entityTransformed.RecordedInDocSuite = Convert.ToByte(true);
                entityTransformed.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().GetByIdWithUDSRepository(entity.DocumentUnit.UniqueId).SingleOrDefault();

                string logDescription = string.Empty;
                switch ((DSWEnvironmentType)entityTransformed.DocumentUnit.Environment)
                {
                    case DSWEnvironmentType.Protocol:
                        {
                            logDescription = $"Pec protocollata con protocollo {entityTransformed.Year.Value}/{entityTransformed.Number.Value:0000000}";
                            break;
                        }
                    case DSWEnvironmentType.UDS:
                        {
                            logDescription = $"Pec collegata ad archivio {entityTransformed.DocumentUnit.UDSRepository.Name} numero {entityTransformed.Year.Value}/{entityTransformed.Number.Value:0000000}";
                            break;
                        }
                    default:
                        break;
                }
                CreatPECMailLog(entityTransformed, PECMailLogType.Linked.ToString(), logDescription);
            }

            if (entity.Attachments != null)
            {
                foreach (PECMailAttachment item in entityTransformed.Attachments.Where(f => !entity.Attachments.Any(c => c.EntityId == f.EntityId)).ToList())
                {
                    _unitOfWork.Repository<PECMailAttachment>().Delete(item);
                }
                foreach (PECMailAttachment item in entity.Attachments.Where(f => !entityTransformed.Attachments.Any(c => c.EntityId == f.EntityId)))
                {
                    item.PECMail = entityTransformed;
                    _unitOfWork.Repository<PECMailAttachment>().Insert(item);
                }
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override PECMail AfterUpdate(PECMail beforeEntity, PECMail persistedEntity)
        {
            if (CurrentUpdateActionType == UpdateActionType.PECMailInvoiceTenantCorrection)
            {
                beforeEntity.Attachments = _unitOfWork.Repository<PECMailAttachment>().GetAttachments(beforeEntity.EntityId).ToList();
                return beforeEntity;
            }
            return persistedEntity;
        }
        #endregion

    }
}
