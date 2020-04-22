namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    ///  Modello del Documento
    /// </summary>
    public sealed class DocumentModel : IWorkflowModel
    {
        #region [ Fields ]

        #endregion

        #region [ Contructors ]

        /// <summary>
        /// Modello del Documento
        /// </summary>
        /// <param name="documentName">Filename del documento</param>
        /// <param name="stream">Stream dati</param>
        /// <param name="documentType">Tipo del documento <see cref="DocumentType"/></param>
        /// <param name="archiveSection">Se l'unità documentaria è un archivio, è necessario specificare anche l'identificativo o Label del documento</param>
        public DocumentModel(string documentName, byte[] stream, DocumentType documentType,
            string archiveSection = "")
        {
            DocumentName = documentName;
            Stream = stream;
            DocumentType = documentType;
            ArchiveSection = archiveSection;
        }
        #endregion

        #region [ Properties ]

        /// <summary>
        /// Filename del documento
        /// </summary>
        public string DocumentName { get; private set; }

        /// <summary>
        /// Stream dati
        /// </summary>
        public byte[] Stream { get; private set; }

        /// <summary>
        /// Tipo del documento <see cref="DocumentType"/>
        /// </summary>
        public DocumentType DocumentType { get; private set; }

        /// <summary>
        /// Se l'unità documentaria è un archivio, è necessario specificare anche l'identificativo o Label del documento
        /// </summary>
        public string ArchiveSection { get; private set; }

        #endregion
    }

}
