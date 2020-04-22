using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Common.Helpers;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Entity.Protocols;
using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.DocumentArchives;
using VecompSoftware.DocSuiteWeb.Finder.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Finder.Fascicles;
using VecompSoftware.DocSuiteWeb.Finder.Protocols;
using VecompSoftware.DocSuiteWeb.Finder.Resolutions;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles
{
    public class FascicleDocumentUnitService : BaseService<FascicleDocumentUnit>, IFascicleDocumentUnitService
    {
        #region [ Fields ]

        private readonly ILogger _logger;
        private readonly IDataUnitOfWork _unitOfWork;
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        private const string DOCUMENT_UNIT_NOT_FOUND_MESSAGE = "Unità documentaria non trovata nel fascicolo.";
        #endregion

        #region [ Constructor ]

        public FascicleDocumentUnitService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            Validation.RulesetDefinitions.Entities.Fascicles.IFascicleRuleset validatorRuleset, IMapperUnitOfWork mapperUnitOfWork,
            ISecurity security) : base(unitOfWork, logger, validationService, validatorRuleset, mapperUnitOfWork, security)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapperUnitOfWork = mapperUnitOfWork;
        }

        #endregion

        #region [ Methods ]

        protected override FascicleDocumentUnit BeforeCreate(FascicleDocumentUnit entity)
        {
            short? idCategory = null;
            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().GetByUniqueId(entity.Fascicle.UniqueId, optimization: false);
                idCategory = entity.Fascicle.Category.EntityShortId;
            }

            if (entity.FascicleFolder != null)
            {
                entity.FascicleFolder = _unitOfWork.Repository<FascicleFolder>().Find(entity.FascicleFolder.UniqueId);
            }

            if (entity.FascicleFolder == null && idCategory.HasValue)
            {
                entity.FascicleFolder = _unitOfWork.Repository<FascicleFolder>().GetByCategoryAndFascicle(entity.Fascicle.UniqueId, idCategory.Value, optimization: false).FirstOrDefault();
            }

            if (entity.DocumentUnit != null)
            {
                entity.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().GetByIdWithCategory(entity.DocumentUnit.UniqueId).SingleOrDefault();
            }

            entity.ReferenceType = ReferenceType.Reference;
            if (entity.Fascicle.FascicleType != FascicleType.Activity)
            {
                entity = AutomaticFascicleDetection(entity);
            }

            FascicleLog fascicleLog = FascicleService.CreateLog(entity.Fascicle, entity.ReferenceType.Equals(ReferenceType.Fascicle) ? FascicleLogType.UDInsert : FascicleLogType.UDReferenceInsert,
                string.Format("Inserimento ({0}) {1} n. {2} in fascicolo n. {3}",
                entity.ReferenceType.Equals(ReferenceType.Fascicle) ? EnumHelper.GetDescription(ReferenceType.Fascicle) : EnumHelper.GetDescription(ReferenceType.Reference),
                LogDocumentNameHelper.GetAttributeDescription(GetType()),
                entity.UniqueId,
                entity.Fascicle == null ? string.Empty : entity.Fascicle.Title), CurrentDomainUser.Account);
            _unitOfWork.Repository<FascicleLog>().Insert(fascicleLog);
            PrepareDocumentUnitLog(entity, fascicleLog);

            if (entity.Fascicle != null)
            {
                ReferenceType referenceType = (entity.ReferenceType == ReferenceType.Fascicle) ? ReferenceType.Reference : ReferenceType.Fascicle;
                IEnumerable<Fascicle> fascicles = AssociatedFascicles(entity.DocumentUnit, referenceType);

                if (fascicles.Any())
                {
                    IEnumerable<Fascicle> fascicleToLinks = fascicles.Where(f => (!f.FascicleLinks.Any() || !f.FascicleLinks.Any(fl => fl.FascicleLinked.UniqueId == entity.Fascicle.UniqueId)));

                    ICollection<FascicleLink> fascicleLinks = new HashSet<FascicleLink>();
                    foreach (Fascicle item in fascicleToLinks)
                    {
                        fascicleLinks.Add(new FascicleLink()
                        {
                            Fascicle = item,
                            FascicleLinked = entity.Fascicle,
                            FascicleLinkType = FascicleLinkType.Automatic
                        });

                        fascicleLinks.Add(new FascicleLink()
                        {
                            Fascicle = entity.Fascicle,
                            FascicleLinked = item,
                            FascicleLinkType = FascicleLinkType.Automatic
                        });
                    }
                    _unitOfWork.Repository<FascicleLink>().InsertRange(fascicleLinks);
                }

            }
            return base.BeforeCreate(entity);
        }

        protected override FascicleDocumentUnit BeforeUpdate(FascicleDocumentUnit entity, FascicleDocumentUnit entityTransformed)
        {
            if (CurrentUpdateActionType != Common.Infrastructures.UpdateActionType.FascicleMoveToFolder)
            {
                throw new DSWException(EXCEPTION_MESSAGE, null, DSWExceptionCode.SS_NotAllowedOperation);
            }

            short? idCategory = null;
            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().GetByUniqueId(entity.Fascicle.UniqueId, optimization: false);
                idCategory = entity.Fascicle.Category.EntityShortId;
            }

            if (entity.FascicleFolder != null)
            {
                entityTransformed.FascicleFolder = _unitOfWork.Repository<FascicleFolder>().GetByFolderId(entity.FascicleFolder.UniqueId, false);
            }

            if (entity.FascicleFolder == null && idCategory.HasValue)
            {
                entityTransformed.FascicleFolder = _unitOfWork.Repository<FascicleFolder>().GetByCategoryAndFascicle(entity.Fascicle.UniqueId, idCategory.Value, optimization: false).FirstOrDefault();
            }
            
            if (entity.DocumentUnit != null)
            {
                entityTransformed.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().GetByIdWithCategory(entity.DocumentUnit.UniqueId).SingleOrDefault();
            }

            entityTransformed = AutomaticFascicleDetection(entityTransformed);

            FascicleLog fascicleLog = new FascicleLog()
            {
                LogType = FascicleLogType.Modify,
                LogDescription = $"Spostato documento {entityTransformed.DocumentUnit.GetTitle()} nella cartella {entityTransformed.FascicleFolder.Name} ({entityTransformed.FascicleFolder.GetTitle()})",
                SystemComputer = Environment.MachineName,
                Entity = entityTransformed.Fascicle
            };
            fascicleLog.Hash = HashGenerator.GenerateHash(string.Concat(fascicleLog.RegistrationUser, "|", fascicleLog.LogType, "|", fascicleLog.LogDescription, "|", fascicleLog.UniqueId, "|", fascicleLog.Entity.UniqueId, "|", fascicleLog.RegistrationDate.ToString("yyyyMMddHHmmss")));
            _unitOfWork.Repository<FascicleLog>().Insert(fascicleLog);
            return base.BeforeUpdate(entity, entityTransformed);
        }


        protected IQueryable<Fascicle> AssociatedFascicles(Protocol documentUnit, ReferenceType referenceType)
        {
            return _unitOfWork.Repository<Fascicle>().GetAssociated(documentUnit, referenceType, false);
        }

        protected IQueryable<FascicleDocumentUnit> GetByFascicle(DocumentUnit documentUnit, Guid idFascicle)
        {
            return _unitOfWork.Repository<FascicleDocumentUnit>().GetByFascicleAndDocumentUnit(documentUnit, idFascicle);
        }

        protected IQueryable<FascicleDocumentUnit> GetByIdFascicleFolder(DocumentUnit documentUnit, Guid idFascicleFolder)
        {
            return _unitOfWork.Repository<FascicleDocumentUnit>().GetByIdFascicleFolder(documentUnit, idFascicleFolder);
        }

        protected void PrepareDocumentUnitLog(FascicleDocumentUnit entity, FascicleLog fascicleLog)
        {
            if (entity.DocumentUnit.Environment == (int)DSWEnvironmentType.Protocol)
            {
                Protocol protocol = _unitOfWork.Repository<Protocol>().GetByUniqueId(entity.DocumentUnit.UniqueId).SingleOrDefault();
                ProtocolLog protocolLog = new ProtocolLog()
                {
                    Year = entity.DocumentUnit.Year,
                    Number = entity.DocumentUnit.Number,
                    LogDate = fascicleLog.RegistrationDate.DateTime,
                    LogType = "FS",
                    Program = "Private.WebAPI",
                    LogDescription = fascicleLog.LogDescription,
                    RegistrationUser = fascicleLog.RegistrationUser,
                    SystemComputer = fascicleLog.SystemComputer,
                    Entity = protocol,
                };
                protocolLog.Hash = HashGenerator.GenerateHash(string.Concat(protocolLog.RegistrationUser, "|", protocolLog.Year, "|", protocolLog.Number, "|", protocolLog.LogType, "|", protocolLog.LogDescription, "|", protocolLog.UniqueId, "|", protocolLog.Entity.UniqueId, "|", protocolLog.LogDate.ToString("yyyyMMddHHmmss")));
                _unitOfWork.Repository<ProtocolLog>().Insert(protocolLog);
            }

            if (entity.DocumentUnit.Environment == (int)DSWEnvironmentType.Resolution)
            {
                Resolution resolution = _unitOfWork.Repository<Resolution>().GetByUniqueId(entity.DocumentUnit.UniqueId).SingleOrDefault();
                ResolutionLog resolutionLog = new ResolutionLog()
                {
                    LogDate = fascicleLog.RegistrationDate.DateTime,
                    LogType = "FS",
                    Program = "Private.WebAPI",
                    LogDescription = fascicleLog.LogDescription,
                    RegistrationUser = fascicleLog.RegistrationUser,
                    SystemComputer = fascicleLog.SystemComputer,
                    Entity = resolution,
                };
                resolutionLog.Hash = HashGenerator.GenerateHash(string.Concat(resolutionLog.RegistrationUser, "|", resolutionLog.LogType, "|", resolutionLog.LogDescription, "|", resolutionLog.UniqueId, "|", resolutionLog.Entity.UniqueId, "|", resolutionLog.LogDate.ToString("yyyyMMddHHmmss")));
                _unitOfWork.Repository<ResolutionLog>().Insert(resolutionLog);
            }

            if (entity.DocumentUnit.Environment == (int)DSWEnvironmentType.DocumentSeries)
            {
                DocumentSeriesItem documentSeriesItem = _unitOfWork.Repository<DocumentSeriesItem>().GetByUniqueId(entity.DocumentUnit.UniqueId).SingleOrDefault();
                DocumentSeriesItemLog documentSeriesItemLog = new DocumentSeriesItemLog()
                {
                    LogDate = fascicleLog.RegistrationDate.DateTime,
                    LogType = "FS",
                    Program = "Private.WebAPI",
                    LogDescription = fascicleLog.LogDescription,
                    RegistrationUser = fascicleLog.RegistrationUser,
                    SystemComputer = fascicleLog.SystemComputer,
                    Entity = documentSeriesItem,
                };
                documentSeriesItemLog.Hash = HashGenerator.GenerateHash(string.Concat(documentSeriesItemLog.RegistrationUser, "|", documentSeriesItemLog.LogType, "|", documentSeriesItemLog.LogDescription, "|", documentSeriesItemLog.UniqueId, "|", documentSeriesItemLog.Entity.UniqueId, "|", documentSeriesItemLog.LogDate.ToString("yyyyMMddHHmmss")));
                _unitOfWork.Repository<DocumentSeriesItemLog>().Insert(documentSeriesItemLog);
            }

            if (entity.DocumentUnit.Environment >= 100)
            {
                UDSRepository uDSRepository = _unitOfWork.Repository<UDSRepository>().GetByIdDocumentUnit(entity.DocumentUnit.UniqueId);
                UDSLog log = new UDSLog()
                {
                    LogType = UDSLogType.Delete,
                    LogDescription = fascicleLog.LogDescription,
                    SystemComputer = string.IsNullOrEmpty(fascicleLog.SystemComputer) ? Environment.MachineName : fascicleLog.SystemComputer,
                    Entity = uDSRepository,
                    RegistrationDate = fascicleLog.RegistrationDate.DateTime,
                    RegistrationUser = fascicleLog.RegistrationUser,
                    IdUDS = entity.DocumentUnit.UniqueId,
                    Environment = entity.DocumentUnit.Environment,
                };
                log.Hash = HashGenerator.GenerateHash(string.Concat(log.RegistrationUser, "|", log.LogType, "|", log.LogDescription, "|", log.UniqueId, "|", log.IdUDS, "|", log.RegistrationDate.ToString("yyyyMMddHHmmss")));
                _unitOfWork.Repository<UDSLog>().Insert(log);
            }
        }

        protected override IQueryFluent<FascicleDocumentUnit> SetEntityIncludeOnUpdate(IQueryFluent<FascicleDocumentUnit> query)
        {
            return query.Include(x => x.FascicleFolder)
                .Include(x => x.Fascicle);
        }

        private FascicleDocumentUnit AutomaticFascicleDetection(FascicleDocumentUnit entity)
        {
            entity.ReferenceType = ReferenceType.Reference;

            if (_unitOfWork.Repository<DocumentUnit>().CanBeFascicolable(entity.Fascicle, entity.DocumentUnit))
            {
                entity.ReferenceType = ReferenceType.Fascicle;
            }
            return entity;
        }

        protected IQueryable<Fascicle> AssociatedFascicles(DocumentUnit documentUnit, ReferenceType referenceType)
        {
            return _unitOfWork.Repository<Fascicle>().GetAssociated(documentUnit, referenceType, false);
        }

        protected override FascicleDocumentUnit BeforeDelete(FascicleDocumentUnit entity, FascicleDocumentUnit entityTransformed)
        {
            if (entityTransformed == null)
            {
                entityTransformed = GetByFascicle(entity.DocumentUnit, entity.Fascicle.UniqueId).FirstOrDefault();
                if (entityTransformed == null)
                {
                    throw new DSWException(DOCUMENT_UNIT_NOT_FOUND_MESSAGE, null, DSWExceptionCode.DB_Anomaly);
                }
            }

            if (entity.Fascicle != null)
            {
                entityTransformed.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            if (entity.DocumentUnit != null)
            {
                entityTransformed.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().Find(entity.DocumentUnit.UniqueId);
            }

            entity.ReferenceType = entityTransformed.ReferenceType;

            _unitOfWork.Repository<FascicleLog>().Insert(FascicleService.CreateLog(entityTransformed.Fascicle, FascicleLogType.UDDelete,
                $"Eliminazione {LogDocumentNameHelper.GetAttributeDescription(GetType())} n. { entityTransformed.DocumentUnit.GetTitle()} da fascicolo n. {entityTransformed.Fascicle.Title}",
                CurrentDomainUser.Account));

            return base.BeforeDelete(entity, entityTransformed);

        }

    }
}
#endregion