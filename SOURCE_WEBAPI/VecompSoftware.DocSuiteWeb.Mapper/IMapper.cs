namespace VecompSoftware.DocSuiteWeb.Mapper
{
    public interface IMapper<T, TTransformed> : IBaseMapper
    {
        #region [ Methods ]
        TTransformed Map(T entity, TTransformed entityTransformed);

        #endregion
    }
}
