using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Finder.MassimariScarto;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository.UnitOfWork;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.MassimariScarto;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.MassimariScarto
{
    public class MassimarioScartoService : BaseService<MassimarioScarto>, IMassimarioScartoService
    {
        #region [ Fields ]

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;
        #endregion

        #region [ Constructor ]

        public MassimarioScartoService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IMassimarioScartoRuleset massimarioScartoRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, massimarioScartoRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]
        protected override MassimarioScarto BeforeCreate(MassimarioScarto entity)
        {
            entity.Status = MassimarioScartoStatus.Active;
            MassimarioScarto root = null;
            if (!entity.FakeInsertId.HasValue && (root = _unitOfWork.Repository<MassimarioScarto>().GetRoot().SingleOrDefault()) != null)
            {
                entity.FakeInsertId = root.UniqueId;
            }
            return base.BeforeCreate(entity);
        }

        protected override MassimarioScarto BeforeUpdate(MassimarioScarto entity, MassimarioScarto entityTransformed)
        {
            MassimarioScarto root = null;
            if (!entityTransformed.FakeInsertId.HasValue && (root = _unitOfWork.Repository<MassimarioScarto>().GetRoot().SingleOrDefault()) != null)
            {
                entityTransformed.FakeInsertId = root.UniqueId;
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }
        #endregion
    }
}
