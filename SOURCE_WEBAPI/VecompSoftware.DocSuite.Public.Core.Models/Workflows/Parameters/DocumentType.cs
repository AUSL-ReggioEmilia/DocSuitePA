namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Tipologia di documento
    /// </summary>
    public enum DocumentType : ushort
    {
        /// <summary>
        /// Non definito
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Documento principale
        /// </summary>
        Main = 1,
        /// <summary>
        /// Documento allegato
        /// </summary>
        Attachment = 2,
        /// <summary>
        /// Documento annesso
        /// </summary>
        Annexed = Attachment * 2,
    }

}