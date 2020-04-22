using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Fascicles;
using VecompSoftware.Helpers.Signer.Security;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.Fascicles
{
    public class FascicleFolderService : BaseService<FascicleFolder>, IFascicleFolderService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]

        public FascicleFolderService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IFascicleRuleset fascicleRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, fascicleRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override FascicleFolder BeforeCreate(FascicleFolder entity)
        {
            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            return base.BeforeCreate(entity);
        }

        protected override IQueryFluent<FascicleFolder> SetEntityIncludeOnUpdate(IQueryFluent<FascicleFolder> query)
        {
            return query.Include(x => x.Fascicle);
        }

        protected override FascicleFolder BeforeUpdate(FascicleFolder entity, FascicleFolder entityTransformed)
        {

            if (entity.Fascicle != null)
            {
                entityTransformed.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (CurrentUpdateActionType == Common.Infrastructures.UpdateActionType.FascicleMoveToFolder)
            {
                FascicleLog fascicleLog = new FascicleLog()
                {
                    LogType = FascicleLogType.Modify,
                    LogDescription = $"Spostata cartella {entityTransformed.Name}",
                    SystemComputer = Environment.MachineName,
                    Entity = entityTransformed.Fascicle
                };
                fascicleLog.Hash = HashGenerator.GenerateHash(string.Concat(fascicleLog.RegistrationUser, "|", fascicleLog.LogType, "|", fascicleLog.LogDescription, "|", fascicleLog.UniqueId, "|", fascicleLog.Entity.UniqueId, "|", fascicleLog.RegistrationDate.ToString("o")));
                _unitOfWork.Repository<FascicleLog>().Insert(fascicleLog);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override FascicleFolder BeforeDelete(FascicleFolder entity, FascicleFolder entityTransformed)
        {

            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion
    }
}