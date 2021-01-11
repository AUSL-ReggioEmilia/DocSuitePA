using VecompSoftware.DocSuiteWeb.Entity.Resolutions;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Resolutions;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Resolutions
{
    public class ResolutionContactValidatorMapper : BaseMapper<ResolutionContact, ResolutionContactValidator>, IResolutionContactValidatorMapper
    {
        public ResolutionContactValidatorMapper() { }

        public override ResolutionContactValidator Map(ResolutionContact entity, ResolutionContactValidator entityTransformed)
        {
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.IdResolution = entity.IdResolution;
            entityTransformed.ComunicationType = entity.ComunicationType;
            //entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;


            #region [ Navigation Properties ]

            entityTransformed.Contact = entity.Contact;
            entityTransformed.Resolution = entity.Resolution;

            #endregion

            return entityTransformed;
        }

    }
}
