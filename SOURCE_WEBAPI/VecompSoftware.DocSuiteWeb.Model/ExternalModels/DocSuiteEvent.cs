using System;

namespace VecompSoftware.DocSuiteWeb.Model.ExternalModels
{
    public class DocSuiteEvent : ICloneable
    {
        #region [ Constructor ]

        public DocSuiteEvent()
        {
            UniqueId = Guid.NewGuid();
        }

        #endregion

        #region [ Properties ]
        /// <summary>
        /// Identificativo univoco dell'evento
        /// </summary>
        public Guid UniqueId { get; }
        /// <summary>
        /// Identificativo univoco del workflow che ha generato l'attività
        /// </summary>
        public Guid? WorkflowReferenceId { get; set; }
        /// <summary>
        /// Modello contenente i valori da comunicare
        /// /// </summary>
        public DocSuiteModel EventModel { get; set; }
        /// <summary>
        /// Modello contenente il riferimento dell'entità che ha generato l'attività.
        /// /// </summary>
        public DocSuiteModel ReferenceModel { get; set; }
        /// <summary>
        /// Date afferente all'attività comunicata.
        /// </summary>
        public DateTimeOffset EventDate { get; set; }
        #endregion

        #region [ Methods ]
        public object Clone()
        {
            DocSuiteEvent docSuiteEvent = new DocSuiteEvent
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
