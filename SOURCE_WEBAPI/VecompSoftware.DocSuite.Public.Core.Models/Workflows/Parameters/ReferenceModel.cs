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
        /// <param name="serializedModel">Specificare im modello serializzato in formato json quando <see cref="DocSuiteEntityType"/> è stato settato su IntegrationEvent</param>
        public ReferenceModel(Guid uniqueId, DocSuiteEntityType docSuiteEntityType, string serializedModel)
        {
            UniqueId = uniqueId;
            DocSuiteEntityType = docSuiteEntityType;
            SerializedModel = serializedModel;
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

        /// <summary>
        /// Modello serializzato in formato json
        /// </summary>
        public string SerializedModel { get; private set; }
        #endregion
    }
}
