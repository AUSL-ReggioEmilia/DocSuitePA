using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.PECMails
{
    public class PECMailErrorSummaryModel
    {
        #region [Constructor]

        public PECMailErrorSummaryModel()
        {

        }

        #endregion

        #region [Properties]
        /// <summary>
        /// Service Bus CorrelatedId Identifier
        /// </summary>
        public Guid CorrelatedId { get; set; }

        /// <summary>
        /// JeepService's error messages
        /// </summary>
        public string ProcessedErrorMessages { get; set; }

        /// <summary>
        /// PEC Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// PEC Body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// PEC Sender
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        ///  PEC Recipients
        /// </summary>
        public string Recipients { get; set; }

        /// <summary>
        /// PEC received date
        /// </summary>
        public DateTime ReceivedDate { get; set; }

        /// <summary>
        /// PEC Priority
        /// </summary>
        public short Priority { get; set; }
        #endregion
    }
}