using VecompSoftware.DocSuiteWeb.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Mapper;
using VecompSoftware.DocSuiteWeb.Validation.Objects.Entities.Commons;

namespace VecompSoftware.DocSuiteWeb.Validation.Mappings.Entities.Commons
{
    public class IncrementalValidatorMapper : BaseMapper<Incremental, IncrementalValidator>, IIncrementalValidatorMapper
    {
        #region [ Constructor ]
        public IncrementalValidatorMapper()
        {
        }
        #endregion

        #region [ Methods ]
        public override IncrementalValidator Map(Incremental entity, IncrementalValidator entityTransformed)
        {
            #region [ Base ]
            entityTransformed.UniqueId = entity.UniqueId;
            entityTransformed.Name = entity.Name;
            entityTransformed.IncrementalValue = entity.IncrementalValue;
            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
