using System;
using VecompSoftware.DocSuite.Public.Core.Models.ContentTypes.Workflows;
using VecompSoftware.DocSuite.Public.Core.Models.Securities;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;

namespace VecompSoftware.DocSuite.Public.Core.Models.Commands.Workflows
{
    /// <summary>
    /// Per avvia un flusso di lavoro tramite le WebAPI, a prescindere dal tipo di Workflow, 
    /// è necessario invocare un comando ‘generico’ definito dall’interfaccia IStartWorkflowCommand. 
    /// Questa interfaccia è state progettata per generalizzare al massimo tutti i possibili workflow definibili nella DocSuite. 
    /// Brevemente esponiamo uno schema sintattico delle parti significative che lo compongono.
    /// </summary>
    public sealed class StartWorkflowCommand : BaseCommand<StartWorkflowContentType, WorkflowModel>
    {
        #region [ Contructors ]

        /// <summary>
        /// Per avvia un flusso di lavoro tramite le WebAPI, a prescindere dal tipo di Workflow, 
        /// è necessario invocare un comando ‘generico’ definito dall’interfaccia IStartWorkflowCommand. 
        /// Questa interfaccia è state progettata per generalizzare al massimo tutti i possibili workflow definibili nella DocSuite. 
        /// Brevemente esponiamo uno schema sintattico delle parti significative che lo compongono.
        /// </summary>
        /// <param name="commandId">Valore guid che identifica univocamente il comando. 
        /// Questo valore sarà la chiave dell’istanza del workflow che funegrà da CorrelationId per gli eventi del Service Bus. 
        /// Diventerà ultile per determinare quali eventi sono associati a determinate istanze attive.</param>
        /// <param name="tenantName">Nome del Tenant del Cliente. Contattare Vecomp Software per il valore</param>
        /// <param name="tenantId">Guid del Tenant del Cliente. Contattare Vecomp Software per il valore</param>
        /// <param name="identityContext">Classe che specifica l'Identità dell'utente che sta eseguento l'avvio del Workflow<see cref="IdentityContext"/></param>
        /// <param name="contentType">ContentType coi parametri necessari all'avvio del Workflow Documentale in DocSuite.
        /// Per le specifiche contattare Vecomp Software<see cref="StartWorkflowContentType"/></param>

        public StartWorkflowCommand(Guid commandId, string tenantName, Guid tenantId,
            IdentityContext identityContext, StartWorkflowContentType contentType)
            : base(commandId, typeof(StartWorkflowCommand).Name, tenantName,
                  tenantId, identityContext, contentType)
        {
        }

        #endregion
    }
}
