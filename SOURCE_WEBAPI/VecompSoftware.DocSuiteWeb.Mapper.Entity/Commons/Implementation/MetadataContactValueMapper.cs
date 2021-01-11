using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class MetadataContactValueMapper : BaseEntityMapper<MetadataValueContact, MetadataValueContact>, IMetadataContactValueMapper
    {
        public MetadataContactValueMapper()
        {
        }

        public override MetadataValueContact Map(MetadataValueContact entity, MetadataValueContact entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.ContactManual = entity.ContactManual;
            #endregion

            return entityTransformed;
        }
    }
}
