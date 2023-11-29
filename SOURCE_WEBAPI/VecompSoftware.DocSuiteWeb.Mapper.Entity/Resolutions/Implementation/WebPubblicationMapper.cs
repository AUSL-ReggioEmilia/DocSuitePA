using VecompSoftware.DocSuiteWeb.Entity.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Resolutions
{
    public class WebPublicationMapper : BaseEntityMapper<WebPublication, WebPublication>, IWebPublicationMapper
    {
        public override WebPublication Map(WebPublication entity, WebPublication entityTransformed)
        {
            #region [ Base ]
            entityTransformed.EntityId = entity.EntityId;
            entityTransformed.ExternalKey = entity.ExternalKey;
            entityTransformed.Status = entity.Status;
            entityTransformed.IDLocation = entity.IDLocation;
            entityTransformed.IDDocument = entity.IDDocument;
            entityTransformed.EnumDocument = entity.EnumDocument;
            entityTransformed.Descrizione = entity.Descrizione;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.IsPrivacy = entity.IsPrivacy;
            #endregion

            return entityTransformed;
        }
    }
}
