using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Commons
{
    public class MetadataRepositoryService : BaseService<MetadataRepository>, IMetadataRepositoryService
    {
        #region [ Field ]
        #endregion

        #region [ Constructor ]

        public MetadataRepositoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IMetadataRepositoryRuleset MetadataRepositoryRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, MetadataRepositoryRuleset, mapperUnitOfWork, security)
        {
        }

        #endregion

        #region [ Method ]
        protected override MetadataRepository BeforeCreate(MetadataRepository entity)
        {
            //TO DO
            //questo è quello che deve essere fatto, revisionare e controllare
            //entity.Version = 1;
            //entity.DateFrom = DateTimeOffset.UtcNow.Date;
            //entity.DateTo = null;
            return base.BeforeCreate(entity);
        }

        protected override MetadataRepository BeforeUpdate(MetadataRepository entity, MetadataRepository entityTransformed)
        {
            //TO DO:
            // Si deve controllare se si sta facendo l'update senza modificare nulla, 
            // si deve controllare se sto aggiornando la giusta versione (debole il controllo sul nome)
            // 
            //MetadataRepository latestRepository = _unitOfWork.Repository<MetadataRepository>().GetLatestVersion(entity.Name);
            //DEVI SAPERE SE ESISTE UNA VERSIONE NON UGUALE A ME (entity.UniqueID)
            //latestRepository == null -> creo nuovo 
            //if (latestRepository.Version > 1)
            //{
            //    entityTransformed.Version = (short)(latestRepository.Version + 1);
            //    entityTransformed.DateFrom = latestRepository.DateTo ?? DateTimeOffset.UtcNow.Date;
            //    entityTransformed.DateTo = null;
            //    latestRepository.DateTo = entity.DateFrom.Date;
            //    _unitOfWork.Repository<MetadataRepository>().Update(latestRepository);
            //}
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
