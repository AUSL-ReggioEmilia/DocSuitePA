using VecompSoftware.DocSuiteWeb.Entity.Commons;

namespace VecompSoftware.DocSuiteWeb.Mapper.Entity.Commons
{
    public class IncrementalMapper : BaseEntityMapper<Incremental, Incremental>, IIncrementalMapper
    {
        #region [ Constructor ]
        public IncrementalMapper()
        {

        }
        #endregion

        #region [ Methods ]
        public override Incremental Map(Incremental entity, Incremental entityTransformed)
        {
            #region [ Base ]
            entityTransformed.Name = entity.Name;
            entityTransformed.IncrementalValue = entity.IncrementalValue;
            #endregion

            return entityTransformed;
        }
        #endregion
    }
}
