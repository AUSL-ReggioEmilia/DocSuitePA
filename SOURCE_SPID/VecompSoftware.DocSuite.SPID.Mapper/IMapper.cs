namespace VecompSoftware.DocSuite.SPID.Mapper.SAML
{
    public interface IMapper<T, TResult>
        where T: class
    {
        TResult Map(T source);
    }
}
