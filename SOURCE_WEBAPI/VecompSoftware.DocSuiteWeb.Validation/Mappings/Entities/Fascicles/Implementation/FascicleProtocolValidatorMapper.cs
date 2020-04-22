using VecompSoftware.DocSuiteWeb.Entity.Fascicles;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Fascicles
{
    public class FascicleProtocolValidatorMapper : BaseMapper<FascicleProtocol, FascicleProtocolValidator>, IFascicleProtocolValidatorMapper
    {
        public FascicleProtocolValidatorMapper(){ }

        
        public override FascicleProtocolValidator Map(FascicleProtocol entity, FascicleProtocolValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.ReferenceType = entity.ReferenceType;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Fascicle = entity.Fascicle;
            entityTransformed.Protocol = entity.DocumentUnit;
            entityTransformed.FascicleFolder = entity.FascicleFolder;
            #endregion

            return entityTransformed;
        }

    }
}
