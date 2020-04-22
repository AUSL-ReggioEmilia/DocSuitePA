namespace VecompSoftware.DocSuiteWeb.Mapper
{
    public abstract class BaseMapper<T, TTransformed> : IMapper<T, TTransformed>
    {
        public BaseMapper() { }

        #region [ Methods ]
        public abstract TTransformed Map(T entity, TTransformed entityTransformed);

        #endregion

    }
}
