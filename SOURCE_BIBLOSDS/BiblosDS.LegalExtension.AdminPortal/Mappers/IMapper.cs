namespace BiblosDS.LegalExtension.AdminPortal.Mappers
{
    public interface IMapper<T, TResult>
    {
        TResult Map(T source);
    }
}
