using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using VecompSoftware.DocSuite.Service.Models.Parameters;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Tenants;
using VecompSoftware.DocSuiteWeb.Finder.Parameters;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Protocols;
using VecompSoftware.Helpers.Signer.Security;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Protocols
{
    public class ProtocolService : BaseService<Protocol>, IProtocolService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ISecurity _security;
        #endregion

        #region [ Constructor ]

        public ProtocolService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IProtocolRuleset protocolRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, protocolRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _security = security;
        }

        #endregion

        #region [ Methods ]

        public ProtocolLog CreatProtocoloLog(Protocol protocol, string logType, string logDescription, string registrationUser = "")
        {
            ProtocolLog protocolLog = new ProtocolLog()
            {
                Year = protocol.Year,
                Number = protocol.Number,
                LogDate = DateTime.UtcNow,
                LogType = logType,
                LogDescription = logDescription,
                SystemComputer = Environment.MachineName,
                Program = "Private.WebAPI",
                Entity = protocol,
                RegistrationUser = string.IsNullOrEmpty(registrationUser) ? _security.GetCurrentUser().Account : registrationUser,
                ObjectState = Repository.Infrastructure.ObjectState.Added
            };
            protocolLog.Hash = HashGenerator.GenerateHash(string.Concat(protocolLog.RegistrationUser, "|", protocolLog.Year, "|", protocolLog.Number, "|", protocolLog.LogType, "|", protocolLog.LogDescription, "|", protocolLog.UniqueId, "|", protocolLog.Entity.UniqueId, "|", protocolLog.LogDate.ToString("yyyyMMddHHmmss")));
            return protocolLog;
        }

        protected override Protocol BeforeCreate(Protocol entity)
        {
            if (CurrentInsertActionType == InsertActionType.CreateProtocol)
            {
                entity.IdStatus = -5;
                Parameter parameter = _unitOfWork.Repository<Parameter>().GetParameters().First();
                parameter.LastUsedNumber++;
                entity.Number = parameter.LastUsedNumber;
                _unitOfWork.Repository<Parameter>().Update(parameter);
                entity.Year = (short)DateTime.Now.Year;
                string identificationSdi = entity.AdvancedProtocol?.IdentificationSdi;
                string serviceCategory = entity.AdvancedProtocol?.ServiceCategory;
                string note = entity.AdvancedProtocol?.Note;
                string protocolStatus = null;
                if (!string.IsNullOrEmpty(entity.AdvancedProtocol?.ProtocolStatus))
                {
                    ProtocolStatus protocolStatuses = _unitOfWork.Repository<ProtocolStatus>().GetByProtocolStatus(entity.AdvancedProtocol.ProtocolStatus).SingleOrDefault();
                    protocolStatus = protocolStatuses.Status;
                }

                entity.AdvancedProtocol = new AdvancedProtocol
                {
                    Year = entity.Year,
                    Number = entity.Number,
                    IdentificationSdi = identificationSdi,
                    ServiceCategory = serviceCategory,
                    Note = note,
                    ProtocolStatus = protocolStatus
                };

                _unitOfWork.Repository<AdvancedProtocol>().Insert(entity.AdvancedProtocol);
                entity.ProtocolLogs.Add(CreatProtocoloLog(entity, "PI", "Creato protocollo"));
                if (entity.Category != null)
                {
                    entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
                }

                if (entity.Container != null)
                {
                    entity.Container = _unitOfWork.Repository<Container>().Find(entity.Container.EntityShortId);
                }

                if (entity.Location != null)
                {
                    entity.Location = _unitOfWork.Repository<Location>().Find(entity.Location.EntityShortId);
                }

                if (entity.AttachLocation != null)
                {
                    entity.AttachLocation = _unitOfWork.Repository<Location>().Find(entity.AttachLocation.EntityShortId);
                }

                if (entity.TenantAOO != null)
                {
                    entity.TenantAOO = _unitOfWork.Repository<TenantAOO>().Find(entity.TenantAOO.UniqueId);
                }

                if (entity.DocType != null)
                {
                    entity.DocType = _unitOfWork.Repository<ProtocolDocumentType>().Find(entity.DocType.EntityShortId);
                }

                if (entity.ProtocolRoles != null && entity.ProtocolRoles.Count > 0)
                {
                    foreach (ProtocolRole item in entity.ProtocolRoles)
                    {
                        if (item.Role != null)
                        {
                            item.Year = entity.Year;
                            item.Number = entity.Number;
                            item.Role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);
                            entity.ProtocolLogs.Add(CreatProtocoloLog(entity, "PZ", $"Autorizzazione (Add): {item.Role.EntityShortId} {item.Role.Name}"));
                        }
                    }
                    _unitOfWork.Repository<ProtocolRole>().InsertRange(entity.ProtocolRoles);
                }

                if (entity.ProtocolUsers != null && entity.ProtocolUsers.Count > 0)
                {
                    foreach (ProtocolUser item in entity.ProtocolUsers)
                    {
                        entity.ProtocolLogs.Add(CreatProtocoloLog(entity, "PZ", $"Autorizzazione utente (Add):  {item.Account}"));
                    }
                    _unitOfWork.Repository<ProtocolUser>().InsertRange(entity.ProtocolUsers);
                }

                if (entity.ProtocolContacts != null && entity.ProtocolContacts.Count > 0)
                {
                    foreach (ProtocolContact item in entity.ProtocolContacts)
                    {
                        if (item.Contact != null)
                        {
                            item.Year = entity.Year;
                            item.Number = entity.Number;
                            item.Contact = _unitOfWork.Repository<Contact>().Find(item.Contact.EntityId);
                        }
                    }
                    _unitOfWork.Repository<ProtocolContact>().InsertRange(entity.ProtocolContacts);
                }
                int incremental = 1;
                if (entity.ProtocolContactManuals != null && entity.ProtocolContactManuals.Count > 0)
                {
                    foreach (ProtocolContactManual item in entity.ProtocolContactManuals)
                    {
                        item.Year = entity.Year;
                        item.Number = entity.Number;
                        item.EntityId = incremental++;
                    }
                    _unitOfWork.Repository<ProtocolContactManual>().InsertRange(entity.ProtocolContactManuals);
                }

            }
            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<Protocol> SetEntityIncludeOnUpdate(IQueryFluent<Protocol> query)
        {
            query.Include(d => d.ProtocolRoles.Select(f => f.Role.TenantAOO))
                .Include(d => d.ProtocolUsers);
            return query;
        }

        protected override bool ExecuteMappingBeforeUpdate()
        {
            return false;
        }
        protected override Protocol BeforeUpdate(Protocol entity, Protocol entityTransformed)
        {
            if (CurrentUpdateActionType == UpdateActionType.ActivateProtocol)
            {
                entityTransformed.IdStatus = 0;
                entityTransformed.IdDocument = entity.IdDocument ?? entityTransformed.IdDocument;
                entityTransformed.IdAttachments = entity.IdAttachments ?? entityTransformed.IdAttachments;
                entityTransformed.IdAnnexed = entity.IdAnnexed ?? entityTransformed.IdAnnexed;
                _unitOfWork.Repository<ProtocolLog>().Insert(CreatProtocoloLog(entityTransformed, "PF", "Impostazione stato attivo"));
                if (entity.ProtocolLogs != null)
                {
                    foreach (ProtocolLog item in entity.ProtocolLogs)
                    {
                        CreatProtocoloLog(entityTransformed, item.LogType, item.LogDescription);
                    }
                }
            }
            if ((!entityTransformed.IdAnnexed.HasValue || (entityTransformed.IdAnnexed.HasValue && entityTransformed.IdAnnexed.Value == Guid.Empty)) 
                && entity.IdAnnexed.HasValue && entity.IdAnnexed.Value != Guid.Empty)
            {
                entityTransformed.IdAnnexed = entity.IdAnnexed;
            }
            if (entity.ProtocolRoles != null)
            {
                foreach (ProtocolRole item in entityTransformed.ProtocolRoles.Where(f => !entity.ProtocolRoles.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<ProtocolLog>().Insert(CreatProtocoloLog(entityTransformed, "PZ", $"Autorizzazione (Del): {item.Role.EntityShortId} {item.Role.Name}"));
                    entityTransformed.ProtocolRoles.Remove(item);
                    _unitOfWork.Repository<ProtocolRole>().Delete(item);
                }
                foreach (ProtocolRole item in entity.ProtocolRoles.Where(f => !entityTransformed.ProtocolRoles.Any(c => c.UniqueId == f.UniqueId)))
                {
                    if (item.Role != null)
                    {
                        item.Year = entity.Year;
                        item.Number = entity.Number;
                        item.Role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);
                        _unitOfWork.Repository<ProtocolLog>().Insert(CreatProtocoloLog(entityTransformed, "PZ", $"Autorizzazione (Add): {item.Role.EntityShortId} {item.Role.Name}"));
                        entityTransformed.ProtocolRoles.Add(item);
                    }
                }
            }

            if (entity.ProtocolUsers != null)
            {
                foreach (ProtocolUser item in entityTransformed.ProtocolUsers.Where(f => !entity.ProtocolUsers.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    _unitOfWork.Repository<ProtocolLog>().Insert(CreatProtocoloLog(entityTransformed, "PZ", $"Autorizzazione utente (Del): {item.Account}"));
                    entityTransformed.ProtocolUsers.Remove(item);
                    _unitOfWork.Repository<ProtocolUser>().Delete(item);
                }
                ICollection<ProtocolUser> toInsertUsers = new List<ProtocolUser>();
                foreach (ProtocolUser item in entity.ProtocolUsers.Where(f => !entityTransformed.ProtocolUsers.Any(c => c.UniqueId == f.UniqueId)))
                {
                    toInsertUsers.Add(item);
                    entityTransformed.ProtocolUsers.Add(item);
                    _unitOfWork.Repository<ProtocolLog>().Insert(CreatProtocoloLog(entityTransformed, "PZ", $"Autorizzazione utente (Add): {item.Account}"));                    
                }
                _unitOfWork.Repository<ProtocolUser>().InsertRange(toInsertUsers);
            }

            entityTransformed.LastChangedUser = entity.LastChangedUser;
            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override bool ExecuteDelete()
        {
            return false;
        }

        protected override Protocol BeforeDelete(Protocol entity, Protocol entityTransformed)
        {
            if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.DeleteProtocol)
            {
                entityTransformed.IdStatus = -2;
                entityTransformed.LastChangedReason = entity.LastChangedReason;
                entityTransformed.LastChangedUser = entity.LastChangedUser;
                entityTransformed.LastChangedDate = DateTimeOffset.UtcNow;
                _unitOfWork.Repository<ProtocolLog>().Insert(CreatProtocoloLog(entityTransformed, "PA", entity.LastChangedReason, registrationUser: entity.LastChangedUser));
                _unitOfWork.Repository<Protocol>().Update(entityTransformed);
                return base.BeforeDelete(entity, entityTransformed);
            }
            throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
        }

        #endregion

    }
}
