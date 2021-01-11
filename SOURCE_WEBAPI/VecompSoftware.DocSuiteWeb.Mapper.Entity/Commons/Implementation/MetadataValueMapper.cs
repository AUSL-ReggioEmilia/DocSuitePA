using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class MetadataValueMapper : BaseEntityMapper<MetadataValue, MetadataValue>, IMetadataValueMapper
    {
        public MetadataValueMapper()
        {
        }

        public override MetadataValue Map(MetadataValue entity, MetadataValue entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.PropertyType = entity.PropertyType;
            entityTransformed.ValueString = entity.ValueString;
            entityTransformed.ValueInt = entity.ValueInt;
            entityTransformed.ValueGuid = entity.ValueGuid;
            entityTransformed.ValueDouble = entity.ValueDouble;
            entityTransformed.ValueDate = entity.ValueDate;
            entityTransformed.ValueBoolean = entity.ValueBoolean;
            #endregion

            return entityTransformed;
        }
    }
}
