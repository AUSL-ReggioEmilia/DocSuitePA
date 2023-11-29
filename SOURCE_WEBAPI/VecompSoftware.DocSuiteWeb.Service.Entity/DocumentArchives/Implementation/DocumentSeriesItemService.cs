using System;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Finder.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentArchives;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.DocumentArchives
{
    public class DocumentSeriesItemService : BaseService<DocumentSeriesItem>, IDocumentSeriesItemService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        private readonly ISecurity _security;
        #endregion

        #region [ Constructor ]
        public DocumentSeriesItemService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDocumentSeriesRuleset documentSeriesRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, documentSeriesRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _security = security;
        }

        #endregion

        #region [ Methods ]
        public DocumentSeriesItemLog CreatDocumentSeriesItemLog(DocumentSeriesItem documentSeriesItem, string logType, string logDescription, string registrationUser = "")
        {
            DocumentSeriesItemLog protocolLog = new DocumentSeriesItemLog()
            {
                IdDocumentSeriesItem = documentSeriesItem.EntityId,
                LogDate = DateTime.UtcNow,
                LogType = logType,
                LogDescription = logDescription,
                SystemComputer = Environment.MachineName,
                Program = "Private.WebAPI",
                Entity = documentSeriesItem,
                RegistrationUser = string.IsNullOrEmpty(registrationUser) ? _security.GetCurrentUser().Account : registrationUser,
                ObjectState = Repository.Infrastructure.ObjectState.Added
            };
            protocolLog.Hash = HashGenerator.GenerateHash(String.Concat(protocolLog.RegistrationUser, "|", protocolLog.LogType, "|", protocolLog.LogDescription, "|", protocolLog.UniqueId, "|", documentSeriesItem.UniqueId, "|", protocolLog.LogDate.ToString("yyyyMMddHHmmss")));
            return protocolLog;
        }

        protected override DocumentSeriesItem BeforeCreate(DocumentSeriesItem entity)
        {
            if (CurrentInsertActionType == InsertActionType.CreateDocumentSeriesItem)
            {
                entity.Status = DocumentSeriesItemStatus.NotActive;                
                if (entity.DocumentSeries != null)
                {
                    entity.DocumentSeries = _unitOfWork.Repository<DocumentSeries>().Find(entity.DocumentSeries.EntityId);
                }
                entity.Number = _unitOfWork.Repository<DocumentSeriesItem>().GetSeriesLastNumberByYear(entity.DocumentSeries.EntityId, entity.Year.Value) + 1;

                if (entity.Category != null)
                {
                    entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
                }

                if (entity.Location != null)
                {
                    entity.Location = _unitOfWork.Repository<Location>().Find(entity.Location.EntityShortId);
                }

                if (entity.LocationAnnexed != null)
                {
                    entity.LocationAnnexed = _unitOfWork.Repository<Location>().Find(entity.LocationAnnexed.EntityShortId);
                }

                if (entity.LocationUnpublishedAnnexed != null)
                {
                    entity.LocationUnpublishedAnnexed = _unitOfWork.Repository<Location>().Find(entity.LocationUnpublishedAnnexed.EntityShortId);
                }

                if (entity.DocumentSeriesItemRoles != null && entity.DocumentSeriesItemRoles.Count > 0)
                {
                    foreach (DocumentSeriesItemRole item in entity.DocumentSeriesItemRoles)
                    {
                        if (item.Role != null)
                        {
                            item.Role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);
                        }
                    }
                    _unitOfWork.Repository<DocumentSeriesItemRole>().InsertRange(entity.DocumentSeriesItemRoles);
                }
                entity.DocumentSeriesItemLogs.Add(CreatDocumentSeriesItemLog(entity, "Insert", string.Empty));
            }
            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<DocumentSeriesItem> SetEntityIncludeOnUpdate(IQueryFluent<DocumentSeriesItem> query)
        {
            return query.Include(d => d.DocumentSeriesItemRoles.Select(f => f.Role.TenantAOO));
        }

        protected override bool ExecuteMappingBeforeUpdate()
        {
            return false;
        }

        protected override DocumentSeriesItem BeforeUpdate(DocumentSeriesItem entity, DocumentSeriesItem entityTransformed)
        {
            if (CurrentUpdateActionType.HasValue && CurrentUpdateActionType == UpdateActionType.ActivateDocumentSeriesItem)
            {
                entityTransformed.Status = DocumentSeriesItemStatus.Active;
                entityTransformed.IdMain = entity.IdMain ?? entityTransformed.IdMain;
                entityTransformed.IdAnnexed = entity.IdAnnexed ?? entityTransformed.IdAnnexed;
                entityTransformed.IdUnpublishedAnnexed = entity.IdUnpublishedAnnexed ?? entityTransformed.IdUnpublishedAnnexed;
            }

            if (CurrentUpdateActionType.HasValue && CurrentUpdateActionType == UpdateActionType.PublishDocumentSeriesItem)
            {
                entityTransformed.PublishingDate = DateTime.Now;
            }

            if (entity.DocumentSeriesItemRoles != null)
            {
                foreach (DocumentSeriesItemRole item in entityTransformed.DocumentSeriesItemRoles.Where(f => !entity.DocumentSeriesItemRoles.Any(c => c.UniqueId == f.UniqueId)).ToList())
                {
                    entityTransformed.DocumentSeriesItemRoles.Remove(item);
                    _unitOfWork.Repository<DocumentSeriesItemRole>().Delete(item);
                }
                foreach (DocumentSeriesItemRole item in entity.DocumentSeriesItemRoles.Where(f => !entityTransformed.DocumentSeriesItemRoles.Any(c => c.UniqueId == f.UniqueId)))
                {
                    if (item.Role != null)
                    {
                        item.Role = _unitOfWork.Repository<Role>().Find(item.Role.EntityShortId);
                        entityTransformed.DocumentSeriesItemRoles.Add(item);
                    }
                }
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override IQueryFluent<DocumentSeriesItem> SetEntityIncludeOnDelete(IQueryFluent<DocumentSeriesItem> query)
        {
            return query.Include(i => i.DocumentSeriesItemRoles)
                .Include(i => i.DocumentSeriesItemLogs)
                .Include(i => i.DocumentSeriesItemLinks);
        }

        protected override bool ExecuteDelete()
        {
            return CurrentDeleteActionType == DeleteActionType.DeleteDocumentSeriesItem;
        }

        protected override DocumentSeriesItem BeforeDelete(DocumentSeriesItem entity, DocumentSeriesItem entityTransformed)
        {
            if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.DeleteDocumentSeriesItem)
            {
                _unitOfWork.Repository<DocumentSeriesItemRole>().DeleteRange(entityTransformed.DocumentSeriesItemRoles.ToList());
                _unitOfWork.Repository<DocumentSeriesItemLog>().DeleteRange(entityTransformed.DocumentSeriesItemLogs.ToList());
                _unitOfWork.Repository<DocumentSeriesItemLink>().DeleteRange(entityTransformed.DocumentSeriesItemLinks.ToList());
                return base.BeforeDelete(entity, entityTransformed);
            }
            throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
        }
        #endregion
    }
}
