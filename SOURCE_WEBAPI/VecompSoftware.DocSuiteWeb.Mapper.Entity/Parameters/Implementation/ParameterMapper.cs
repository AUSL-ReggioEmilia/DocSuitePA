using VecompSoftware.DocSuiteWeb.Entity.Parameters;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Parameters
{
    public class ParameterMapper : BaseEntityMapper<Parameter, Parameter>, IParameterMapper
    {
        public override Parameter Map(Parameter entity, Parameter entityTransformed)
        {
            #region [ Base ]
            entityTransformed.LastUsedYear = entity.LastUsedYear;
            entityTransformed.LastUsedNumber = entity.LastUsedNumber;
            entityTransformed.Locked = entity.Locked;
            entityTransformed.LastUsedIdContainer = entity.LastUsedIdContainer;
            entityTransformed.LastUsedIdRole = entity.LastUsedIdRole;
            entityTransformed.LastUsedIdRoleUser = entity.LastUsedIdRoleUser;
            entityTransformed.LastUsedIdResolution = entity.LastUsedIdResolution;
            entityTransformed.LastUsedResolutionYear = entity.LastUsedResolutionYear;
            entityTransformed.LastUsedResolutionNumber = entity.LastUsedResolutionNumber;
            entityTransformed.LastUsedBillNumber = entity.LastUsedBillNumber;
            #endregion

            return entityTransformed;
        }
    }
}
