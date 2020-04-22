namespace VecompSoftware.Commons.Interfaces.ServiceLocator
{
    public interface ILocator
    {
        T GetService<T>();
    }
}
