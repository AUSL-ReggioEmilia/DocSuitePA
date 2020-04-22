using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Mapper.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits
{
    public class DocumentUnitFascicleHistoricizedCategoryService : BaseService<DocumentUnitFascicleHistoricizedCategory>, IDocumentUnitFascicleHistoricizedCategoryService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        #endregion

        #region [ Constructor ]
        public DocumentUnitFascicleHistoricizedCategoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDocumentUnitFascicleHistoricizedCategoryRuleset documentUnitFascicleHistoricizedCategoryRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security)
            : base(unitOfWork, logger, validationService, documentUnitFascicleHistoricizedCategoryRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override DocumentUnitFascicleHistoricizedCategory BeforeCreate(DocumentUnitFascicleHistoricizedCategory entity)
        {
            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.DocumentUnit != null)
            {
                entity.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().Find(entity.DocumentUnit.UniqueId);
            }

            return base.BeforeCreate(entity);
        }

        protected override DocumentUnitFascicleHistoricizedCategory BeforeUpdate(DocumentUnitFascicleHistoricizedCategory entity, DocumentUnitFascicleHistoricizedCategory entityTransformed)
        {
            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.DocumentUnit != null)
            {
                entityTransformed.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().Find(entity.DocumentUnit.UniqueId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion
    }
}
