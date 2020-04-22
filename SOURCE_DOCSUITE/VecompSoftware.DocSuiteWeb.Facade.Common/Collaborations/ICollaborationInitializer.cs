
namespace VecompSoftware.DocSuiteWeb.Facade.Common.Collaborations
{
    public interface ICollaborationInitializer
    {
        /// <summary>
        /// Recupera dalla pagina i dati di inizializzazione per l'inserimento di una nuova collaborazione
        /// </summary>
        /// <returns></returns>
        CollaborationInitializer GetCollaborationInitializer();
    }
}
