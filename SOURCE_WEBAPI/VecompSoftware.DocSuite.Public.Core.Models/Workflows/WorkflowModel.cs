using System;
using System.Collections.Generic;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows
{
    /// <summary>
    /// Modello che permette l'avvio del Workflow. Viene utilizzato nel content type del comando <see cref="Commands.Workflows.StartWorkflowCommand"/>
    /// </summary>
    public sealed class WorkflowModel : BaseModel
    {
        #region [ Fields ]
        #endregion

        #region [ Contructors ]
        /// <summary>
        /// Modello che permette l'avvio del Workflow. Viene utilizzato nel content type del comando <see cref="Commands.Workflows.StartWorkflowCommand"/>
        /// </summary>
        /// <param name="workflowName">Setta la proprietà Name del modello. Il valore è il nome del Workflow configurato nella DocSuite. 
        /// Contattare Dgroove per conoscere i valori del cliente</param>
        /// <param name="activityTitlePrefix">Permette di personalizzare le attività che vengono generate nella scrivania dell'utente in DocSuite.
        /// Questo sistema è utile quanto le attività del workflow non possono essere completamente 
        /// automatizzate e quindi necessitano dell'intervento dell'operatore per essere completate. 
        /// La lunghezza del parametro viene 'troncata' se supera i 50 caratteri.</param>
        public WorkflowModel(string workflowName, string activityTitlePrefix = "")
            : base(Guid.NewGuid())
        {
            Name = workflowName;
            ActivityTitlePrefix = activityTitlePrefix?.Length > 50 ? activityTitlePrefix?.Substring(0, 49) : activityTitlePrefix;
            WorkflowParameters = new HashSet<WorkflowParameterModel>();
        }
        #endregion

        #region [ Properties ]
        /// <summary>
        /// Permette di personalizzare le attività che vengono generate nella scrivania dell'utente in DocSuite.
        /// Questo sistema è utile quanto le attività del workflow non possono essere completamente 
        /// automatizzate e quindi necessitano dell'intervento dell'operatore per essere completate. 
        /// La lunghezza del parametro viene 'troncata' se supera i 50 caratteri.
        /// </summary>
        public string ActivityTitlePrefix { get; private set; }

        /// <summary>
        /// Collezione contenente i parametri del workflow
        /// </summary>
        public ICollection<WorkflowParameterModel> WorkflowParameters { get; private set; }

        #endregion

    }
}
