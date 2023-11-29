using VecompSoftware.DocSuiteWeb.Entity.Parameters;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Parameter;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Parameters
{
    public class ParameterValidatorMapper : BaseMapper<Parameter, ParameterValidator>, IParameterValidatorMapper
    {
        public ParameterValidatorMapper() { }

        public override ParameterValidator Map(Parameter entity, ParameterValidator entityTransformed)
        {
            #region [Base]

            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Incremental = entity.Incremental;
            entityTransformed.LastUsedYear = entity.LastUsedYear;
            entityTransformed.LastUsedNumber = entity.LastUsedNumber;
            entityTransformed.Locked = entity.Locked;
            entityTransformed.LastUsedidCategory = entity.LastUsedIdCategory;
            entityTransformed.LastUsedidContainer = entity.LastUsedIdContainer;
            entityTransformed.LastUsedidRole = entity.LastUsedIdRole;
            entityTransformed.LastUsedIdRoleUser = entity.LastUsedIdRoleUser;
            entityTransformed.LastUsedidResolution = entity.LastUsedIdResolution;
            entityTransformed.LastUsedResolutionYear = entity.LastUsedResolutionYear;
            entityTransformed.LastUsedResolutionNumber = entity.LastUsedResolutionNumber;
            entityTransformed.LastUsedBillNumber = entity.LastUsedBillNumber;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;

            #endregion

            #region [ Navigation Properties ]
            entityTransformed.TenantAOO = entity.TenantAOO;
            #endregion

            return entityTransformed;
        }
    }
}
