using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Common.Infrastructures;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Finder.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Repository;
using VecompSoftware.DocSuiteWeb.Repository.Infrastructure;
using VecompSoftware.DocSuiteWeb.Repository.Parameters;
using VecompSoftware.DocSuiteWeb.Repository.Repositories;
using VecompSoftware.DocSuiteWeb.Security;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Service.Entity.UDS
{
    public class UDSFieldListService : BaseService<UDSFieldList>, IUDSFieldListService
    {
        #region [ Fields ]
        private readonly IDataUnitOfWork _unitOfWork;
        #endregion

        #region [ Constructor ]
        public UDSFieldListService(IDataUnitOfWork unitOfWork, ILogger logger, IValidatorService validationService,
            IUDSRuleset ruleset, IMapperUnitOfWork mapper, ISecurity security)
            : base(unitOfWork, logger, validationService, ruleset, mapper, security)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region [ Methods ]
        protected override UDSFieldList BeforeCreate(UDSFieldList entity)
        {
            if (entity.Repository != null)
            {
                entity.Repository = _unitOfWork.Repository<UDSRepository>().Find(entity.Repository.UniqueId);
            }
            return base.BeforeCreate(entity);
        }

        protected override UDSFieldList BeforeUpdate(UDSFieldList entity, UDSFieldList entityTransformed)
        {
            if (entity.Repository != null)
            {
                entityTransformed.Repository = _unitOfWork.Repository<UDSRepository>().Queryable().Where(x => x.UniqueId == entity.Repository.UniqueId).FirstOrDefault();
            }
            return base.BeforeUpdate(entity, entityTransformed);
        }

        protected override IQueryFluent<UDSFieldList> SetEntityIncludeOnUpdate(IQueryFluent<UDSFieldList> query)
        {
            query.Include(x => x.Repository);
            return query;
        }
        protected override bool ExecuteDelete()
        {
            return CurrentDeleteActionType.HasValue && CurrentDeleteActionType != DeleteActionType.DeleteUDSFieldList;
        }
        protected override UDSFieldList BeforeDelete(UDSFieldList entity, UDSFieldList entityTransformed)
        {
            //Request is sent for every child node from kendo TreeList
            //TODO: Find a solution to skip api request in Control.ts from DocSuite before removing entity.Status condition
            if (CurrentDeleteActionType.HasValue && CurrentDeleteActionType == DeleteActionType.DeleteUDSFieldList && entity.Status != UDSFieldListStatus.Invalid)
            {
                _unitOfWork.Repository<UDSFieldList>().ExecuteProcedure(CommonDefinition.SQL_SP_UDSFieldList_PropagateUDSField_Status,
                    new QueryParameter(CommonDefinition.SQL_Param_UDSFieldList_IdUDSField, entity.UniqueId));
            }
            return base.BeforeDelete(entity, entityTransformed);
        }
        #endregion
    }
}
