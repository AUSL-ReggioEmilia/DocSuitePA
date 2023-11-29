using System;
using System.Collections.Generic;
using VecompSoftware.Commons.Interfaces.CQRS.Commands;
using VecompSoftware.Commons.Interfaces.CQRS.Events;

namespace VecompSoftware.DocSuiteWeb.Model.ExternalModels
{
    public class DocSuiteEvent : ICloneable, IWorkflowContentBase
    {
        #region [ Constructor ]

        public DocSuiteEvent()
        {
            UniqueId = Guid.NewGuid();
            WorkflowActions = new List<IWorkflowAction>();
        }

        #endregion

        #region [ Properties ]
        /// <summary>
        /// Identificativo univoco dell'evento
        /// es: F1345CE3-71B0-4D79-9E9A-57ABD0EF4978
        /// </summary>
        public Guid UniqueId { get; }
        /// <summary>
        /// Questa proprietà corrisponde all'instanza del workflow che ha generato l'evento.
        /// In workflow di tipo "Public" corrisponde al valore specificato nella proprietà "Id" 
        /// del modello StartWorkflowCommand.
        /// es: D11EB32F-5386-4DC5-A0D1-CA0A55D834A7
        /// </summary>
        public Guid? WorkflowReferenceId { get; set; }
        /// <summary>
        /// Modello contenente le informazioni del risultato del workflow.
        /// es: i riferimenti del protocollo creato.
        /// /// </summary>
        public DocSuiteModel EventModel { get; set; }
        /// <summary>
        /// Modello contenente il riferimento dell'entità che ha generato l'attività.
        /// Ad esempio la gestione di una protocollazione del flusso di collaborazione conterrà 
        /// nella proprietà EventModel i riferimento del protocollo appena creato, ma in ReferenceModel
        /// ci saranno gli estremi che identificano la collaborazione.
        /// Analogalmente per i flusso di invio PEC di protocollo, i ReferenceModel conterrà di dati del protocollo
        /// e nell'EventModel ci saranno i dati della PEC.
        /// /// </summary>
        public DocSuiteModel ReferenceModel { get; set; }
        /// <summary>
        /// Date afferente all'attività comunicata.
        /// es: se EventModel è di tipologia PEC sarà la data di invio della PEC
        ///     se EventModel è di tipologia PROTOCOLLO sarà la data di creazione del Protocollo
        ///     se EventModel è di tipologia COLLABORAZIONE sarà la data di creazione della Collaborazione
        /// </summary>
        public DateTimeOffset EventDate { get; set; }
        #endregion

        #region [ Workflow Properties ]
        public bool WorkflowAutoComplete { get; set; }
        public string WorkflowName { get; set; }
        public Guid? IdWorkflowActivity { get; set; }
        public ICollection<IWorkflowAction> WorkflowActions { get; set; }
        public string RegistrationUser { get; set; }
        Guid IContentBase.UniqueId { get; set; }
        #endregion

        #region [ Methods ]
        public object Clone()
        {
            _ = new DocSuiteEvent
            {
                EventDate = new DateTimeOffset(EventDate.UtcDateTime, EventDate.Offset),
                EventModel = EventModel,
                ReferenceModel = ReferenceModel,
                WorkflowReferenceId = WorkflowReferenceId
            };
            return MemberwiseClone();
        }
        #endregion
    }
}
