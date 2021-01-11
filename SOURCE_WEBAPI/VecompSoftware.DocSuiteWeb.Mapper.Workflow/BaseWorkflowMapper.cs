namespace VecompSoftware.DocSuiteWeb.Mapper.Workflow
{
    public abstract class BaseWorkflowMapper<T, TTransformed> : BaseDomainMapper<T, TTransformed>, IDomainMapper<T, TTransformed>
        where T : class
        where TTransformed : class, new()
    {
        public BaseWorkflowMapper() { }

    }
}
