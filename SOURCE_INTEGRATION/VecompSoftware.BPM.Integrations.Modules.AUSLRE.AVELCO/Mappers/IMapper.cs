namespace VecompSoftware.BPM.Integrations.Modules.AUSLRE.AVELCO.Mappers
{
    public interface IMapper<T, TResult>
    {
        TResult Map(T source);
    }
}
