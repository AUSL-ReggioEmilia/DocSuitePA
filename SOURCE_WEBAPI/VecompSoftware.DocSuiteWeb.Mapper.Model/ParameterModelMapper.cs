namespace VecompSoftware.DocSuiteWeb.Mapper.Model
{
    public abstract class ParameterModelMapper<T, TTransformed> : BaseMapper<T, TTransformed>, IMapper<T, TTransformed>
        where T : class
        where TTransformed : struct
    {
        public ParameterModelMapper() { }

    }
}
