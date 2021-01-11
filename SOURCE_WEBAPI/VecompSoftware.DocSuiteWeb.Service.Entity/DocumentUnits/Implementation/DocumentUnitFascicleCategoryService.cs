using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.DocumentUnits
{
    public class DocumentUnitFascicleCategoryService : BaseService<DocumentUnitFascicleCategory>, IDocumentUnitFascicleCategoryService
    {
        #region [ Fields ]

        private readonly IDataUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        #endregion

        #region [ Constructor ]

        public DocumentUnitFascicleCategoryService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IDocumentUnitFascicleCategoryRuleset documentUnitFascicleCategoryRuleset, IMapperUnitOfWork mapperUnitOfWork, ISecurity security) 
            : base(unitOfWork, logger, validationService, documentUnitFascicleCategoryRuleset, mapperUnitOfWork, security)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region [ Methods ]

        protected override DocumentUnitFascicleCategory BeforeCreate(DocumentUnitFascicleCategory entity)
        {
            if (entity.Category != null)
            {
                entity.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.DocumentUnit != null)
            {
                entity.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().Find(entity.DocumentUnit.UniqueId);
            }

            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            return base.BeforeCreate(entity);
        }

        protected override DocumentUnitFascicleCategory BeforeUpdate(DocumentUnitFascicleCategory entity, DocumentUnitFascicleCategory entityTransformed)
        {
            if (entity.Category != null)
            {
                entityTransformed.Category = _unitOfWork.Repository<Category>().Find(entity.Category.EntityShortId);
            }

            if (entity.DocumentUnit != null)
            {
                entityTransformed.DocumentUnit = _unitOfWork.Repository<DocumentUnit>().Find(entity.DocumentUnit.UniqueId);
            }

            if (entity.Fascicle != null)
            {
                entity.Fascicle = _unitOfWork.Repository<Fascicle>().Find(entity.Fascicle.UniqueId);
            }

            return base.BeforeUpdate(entity, entityTransformed);
        }

        #endregion
    }
}
