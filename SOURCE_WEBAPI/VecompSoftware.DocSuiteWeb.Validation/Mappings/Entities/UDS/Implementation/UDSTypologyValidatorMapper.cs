using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSTypologyValidatorMapper : BaseMapper<UDSTypology, UDSTypologyValidator>, IUDSTypologyValidatorMapper
    {
        public UDSTypologyValidatorMapper() { }

        public override UDSTypologyValidator Map(UDSTypology entity, UDSTypologyValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.UDSRepositories = entity.UDSRepositories;

            #endregion

            return entityTransformed;
        }
    }
}
