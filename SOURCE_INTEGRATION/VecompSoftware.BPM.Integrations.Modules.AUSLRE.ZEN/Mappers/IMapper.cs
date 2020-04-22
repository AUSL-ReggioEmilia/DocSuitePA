namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.ZEN.Mappers
{
    public interface IMapper<T, TResult>
    {
        TResult Map(T source);
    }
}
