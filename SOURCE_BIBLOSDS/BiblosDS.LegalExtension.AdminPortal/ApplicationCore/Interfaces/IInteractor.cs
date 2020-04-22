namespace BiblosDS.LegalExtension.AdminPortal.ApplicationCore.Interfaces
{
    public interface IInteractor<in T, out TOut>
    {
        TOut Process(T request);
    }
}
