using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class ResolutionDocumentSeriesItemMapper : BaseEntityMapper<ResolutionDocumentSeriesItem, ResolutionDocumentSeriesItem>, IResolutionDocumentSeriesItemMapper
    {
        public override ResolutionDocumentSeriesItem Map(ResolutionDocumentSeriesItem entity, ResolutionDocumentSeriesItem entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.EntityShortId = entity.EntityShortId;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.DocumentSeriesItem = entity.DocumentSeriesItem;
            entityTransformed.Resolution = entity.Resolution;
            #endregion

            return entityTransformed;
        }
    }
}
