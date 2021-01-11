namespace VecompSoftware.DocSuiteWeb.Mapper.Model
{
    public abstract class BaseModelMapper<T, TTransformed> : BaseDomainMapper<T, TTransformed>, IDomainMapper<T, TTransformed>
        where T : class
        where TTransformed : class, new()
    {
        public BaseModelMapper() { }

    }
}
