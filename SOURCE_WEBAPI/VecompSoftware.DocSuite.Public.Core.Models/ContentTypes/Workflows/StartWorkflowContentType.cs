using System;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;

namespace VecompSoftware.DocSuite.Public.Core.Models.ContentTypes.Workflows
{
    /// <summary>
    ///  Classe che gestisce il contenuto del comando <see cref="Commands.Workflows.StartWorkflowCommand"/>
    /// </summary>
    public sealed class StartWorkflowContentType : BaseWorkflowContentType<WorkflowModel>
    {
        #region [ Fields ]
        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Classe che gestisce il contenuto del comando <see cref="Commands.Workflows.StartWorkflowCommand"/>
        /// </summary>
        /// <param name="workflowModel">Modello coi parametri necessario all'avvio del Workflow.<see cref="WorkflowModel"/></param>
        /// <param name="executorUser">Dovrebbe sempre corrisponsere all'utente dell'IdentityContext.<see cref="Securities.IdentityContext"/>
        /// Se in particolari contesti integrativi i servizi non riescono a determinare correttamente l'IdentityContext è possibile passare a mano 
        /// l'utente applicativo che ha avviato la richiesta del comando.</param>
        /// <param name="correlationId">Valore univoco che permette di correlare il comando con una chiave esterna del sistema di integrazione.
        /// Può essere utile per impostare un 'indentificativo custom' che il sistema esterno potrà poi utilizzare per determinare 
        /// quali sono gli eventi che deve consumare dal service bus. </param>
        public StartWorkflowContentType(WorkflowModel workflowModel, string executorUser, Guid? correlationId = null)
            : base(workflowModel, executorUser)
        {
            CorrelationId = correlationId;
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Valore univoco che permette di correlare il comando con una chiave esterna del sistema di integrazione.
        /// Può essere utile per impostare un 'indentificativo custom' che il sistema esterno potrà poi utilizzare per determinare 
        /// quali sono gli eventi che deve consumare dal service bus. 
        /// </summary>
        public Guid? CorrelationId { get; private set; }
        #endregion
    }
}
