using System;

namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    ///  Modello del Metadato dinamico.
    /// </summary>
    public sealed class MetadataModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello del Metadato dinamico.
        /// I singoli valori vanno concordati con Dgroove in fase di attivazione dell'integrazione con DocSuite 
        /// </summary>
        /// <param name="keyName">Nome del metadato.</param>
        /// <param name="value">Valore del metadato in formato stringa.</param>
        /// <param name="metadataId">Identificativo interno del metadato.</param>
        /// <param name="labelName">Etichetta di visualizzazione del metadato.</param>
        /// <param name="archiveSection">Se l'unità documentaria è un archivio, è necessario specificare la sezione del metadato.</param>
        public MetadataModel(string keyName, string value, Guid? metadataId = null, string labelName = "", string archiveSection = "")
        {
            KeyName = keyName;
            Value = value;
            MetadataId = metadataId;
            LabelName = labelName;
            ArchiveSection = archiveSection;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Nome del metadato
        /// </summary>
        public string KeyName { get; private set; }

        /// <summary>
        /// Valore del metadato
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Identificativo interno del metadato.
        /// </summary>
        public Guid? MetadataId { get; private set; }

        /// <summary>
        /// Etichetta di visualizzazione del metadato.
        /// </summary>
        public string LabelName { get; private set; }

        /// <summary>
        /// Se l'unità documentaria è un archivio, è necessario specificare la sezione del metadato.
        /// </summary>
        public string ArchiveSection { get; private set; }
        #endregion
    }

}
