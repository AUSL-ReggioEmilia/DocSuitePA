using VecompSoftware.DocSuiteWeb.Entity.UDS;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.UDS;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.UDS
{
    public class UDSFieldListValidatorMapper : BaseMapper<UDSFieldList, UDSFieldListValidator>, IUDSFieldListValidatorMapper
    {

        #region [ Constructor ]
        public UDSFieldListValidatorMapper() { }
        #endregion

        #region [ Methods ]
        public override UDSFieldListValidator Map(UDSFieldList entity, UDSFieldListValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.FieldName = entity.FieldName;
            entityTransformed.Environment = entity.Environment;
            entityTransformed.Name = entity.Name;
            entityTransformed.Status = entity.Status;
            entityTransformed.UDSFieldListLevel = entity.UDSFieldListLevel;
            entityTransformed.UDSFieldListPath = entity.UDSFieldListPath;
            entityTransformed.ParentInsertId = entity.ParentInsertId;
            #endregion

            #region [ Navigation Properties ]
            entityTransformed.Repository = entity.Repository;
            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
