using System;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.PECMails
{
    public class PECMailErrorStreamModel
    {
        #region [Constructor]

        public PECMailErrorStreamModel()
        {

        }

        #endregion

        #region [Properties]
        /// <summary>
        /// Service Bus CorrelatedId Identifier
        /// </summary>
        public Guid CorrelatedId { get; set; }

        /// <summary>
        /// EML/MSG content file
        /// </summary>
        public byte[] Stream { get; set; }

        #endregion
    }
}