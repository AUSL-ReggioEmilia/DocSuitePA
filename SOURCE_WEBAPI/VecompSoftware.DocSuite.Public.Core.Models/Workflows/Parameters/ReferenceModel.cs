using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Modello che rappresenta una referenza a un'entità della DocSuite
    /// </summary>
    public sealed class ReferenceModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello che rappresenta una referenza a un'entità della DocSuite
        /// </summary>
        /// <param name="uniqueId">Chiave univoca dell'entità della DocSuite</param>
        /// <param name="docSuiteEntityType">Tipologie di entità della DocSuite <see cref="DocSuiteEntityType"/></param>
        public ReferenceModel(Guid uniqueId, DocSuiteEntityType docSuiteEntityType)
        {
            UniqueId = uniqueId;
            DocSuiteEntityType = docSuiteEntityType;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Chiave univoca dell'entità della DocSuite
        /// </summary>
        public Guid UniqueId { get; private set; }

        /// <summary>
        /// Tipologie di entità della DocSuite
        /// </summary>
        public DocSuiteEntityType DocSuiteEntityType { get; private set; }

        #endregion
    }
}
