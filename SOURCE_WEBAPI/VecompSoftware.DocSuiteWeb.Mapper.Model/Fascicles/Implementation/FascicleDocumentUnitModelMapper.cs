using VecompSoftware.DocSuiteWeb.Entity.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;
using VecompSoftwareFascicle = VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles
{
    public class FascicleDocumentUnitModelMapper : BaseModelMapper<FascicleDocumentUnit, FascicleDocumentUnitModel>, IFascicleDocumentUnitModelMapper
    {
        #region [ Fields ]
        private readonly IMapperUnitOfWork _mapperUnitOfWork;
        #endregion
        public FascicleDocumentUnitModelMapper(IMapperUnitOfWork mapperUnitOfWork)
        {
            _mapperUnitOfWork = mapperUnitOfWork;
        }
        public override FascicleDocumentUnitModel Map(FascicleDocumentUnit entity, FascicleDocumentUnitModel entityTransformed)
        {
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ReferenceType = (VecompSoftwareFascicle.ReferenceType)entity.ReferenceType;
            entityTransformed.SequenceNumber = entity.SequenceNumber;
            entityTransformed.DocumentUnit = _mapperUnitOfWork.Repository<IDomainMapper<DocumentUnit, DocumentUnitModel>>().Map(entity.DocumentUnit, new DocumentUnitModel());
            return entityTransformed;
        }
    }
}

