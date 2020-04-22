using System.Collections.Generic;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;

namespace VecompSoftware.DocSuite.Public.Core.Models.Customs.Workflows.Parameters
{
    public sealed class CherwellContactModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello del contatto per l'integrazione Cherwell
        /// </summary>
        public CherwellContactModel()
        {
            fields = new HashSet<CherwellMetadataModel>();
        }

        #endregion

        #region [ Properties ]

        public string busObId { get; set; }
        public string busObRecId { get; set; }
        public string busObPublicId { get; set; }
        public ICollection<CherwellMetadataModel> fields { get; private set; }
        #endregion
    }
}