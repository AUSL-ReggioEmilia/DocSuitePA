namespace VecompSoftware.JeepService.LogConservation.Mappers
{
    public interface IMapper<T, TResult>
    {
        TResult Map(T source);
        TResult Map(TResult reference, T source);
    }
}
