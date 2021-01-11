using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;
using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Mapper.Model.Fascicles
{
    public class FascicleDocumentModelMapper : BaseModelMapper<FascicleDocument, FascicleDocumentModel>, IFascicleDocumentModelMapper
    {
        public override FascicleDocumentModel Map(FascicleDocument entity, FascicleDocumentModel entityTransformed)
        {

            entityTransformed.IdArchiveChain = entity.IdArchiveChain;
            entityTransformed.ChainType = (ChainType)entity.ChainType;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.UniqueId = entity.UniqueId;
            return entityTransformed;
        }
    }
}
