using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class TableLogMapper : BaseEntityMapper<TableLog, TableLog>, ITableLogMapper
    {
        public TableLogMapper()
        {

        }

        public override TableLog Map(TableLog entity, TableLog entityTransformed)
        {
            #region [ Base ]
            entityTransformed.LoggedEntityId = entity.LoggedEntityId;
            entityTransformed.LoggedEntityUniqueId = entity.LoggedEntityUniqueId;
            entityTransformed.LogType = entity.LogType;
            entityTransformed.TableName = entity.TableName;
            entityTransformed.SystemComputer = entity.SystemComputer;
            entityTransformed.LogDescription = entity.LogDescription;
            entityTransformed.Hash = entity.Hash;
            #endregion

            return entityTransformed;
        }

    }
}
