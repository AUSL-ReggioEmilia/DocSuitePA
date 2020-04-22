using System;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows
{
    /// <summary>
    /// Modello che rappresenta lo stato del workflow avviato
    /// </summary>
    public class WorkflowStatusModel : BaseModel
    {
        #region [ Fields ]
        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Modello che rappresenta lo stato del workflow avviato
        /// </summary>
        /// <param name="instanceId">Identificativo dell'istanza</param>
        /// <param name="workflowName">Setta la proprietà Name del modello. Il valore è il nome del Workflow configurato nella DocSuite. </param>
        public WorkflowStatusModel(Guid instanceId, string workflowName)
            : base(instanceId)
        {
            Name = workflowName;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Stato del workflow <seealso cref="WorkflowStatus"/>
        /// </summary>
        public WorkflowStatus Status { get; set; }

        /// <summary>
        /// Contiene la data dell'instanza del workflow
        /// </summary>
        public DateTimeOffset Date { get; set; }
        #endregion

    }
}
