namespace VecompSoftware.DocSuiteWeb.Mapper.ServiceBus
{
    public abstract class BaseServiceBusMapper<T, TTransformed> : BaseDomainMapper<T, TTransformed>, IDomainMapper<T, TTransformed>
        where T : class
        where TTransformed : class, new()
    {
        public BaseServiceBusMapper() { }

    }
}
