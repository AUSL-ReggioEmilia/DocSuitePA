using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class TableLogValidatorMapper : BaseMapper<TableLog, TableLogValidator>, ITableLogValidatorMapper
    {
        public TableLogValidatorMapper() { }

        public override TableLogValidator Map(TableLog entity, TableLogValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.LoggedEntityId = entity.LoggedEntityId;
            entityTransformed.LoggedEntityUniqueId = entity.LoggedEntityUniqueId;
            entityTransformed.TableName = entity.TableName;
            entityTransformed.RegistrationDate = entity.RegistrationDate;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.RegistrationUser = entity.RegistrationUser;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.Hash = entity.Hash;
            entityTransformed.LastChangedDate = entity.LastChangedDate;
            entityTransformed.LastChangedUser = entity.LastChangedUser;
            entityTransformed.Timestamp = entity.Timestamp;
            #endregion

            #region [ Navigation Properties ]
            #endregion

            return entityTransformed;
        }

    }
}
