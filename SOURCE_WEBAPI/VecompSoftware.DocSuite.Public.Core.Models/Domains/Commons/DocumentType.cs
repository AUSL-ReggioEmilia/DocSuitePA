
namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Commons
{
    /// <summary>
    /// Tipologia di documento
    /// </summary>
    public enum DocumentType : short
    {
        /// <summary>
        /// Miscellanea
        /// </summary>
        Miscellanea = -1,
        /// <summary>
        /// Documento principale
        /// </summary>
        Main = 0,
        /// <summary>
        /// Documento allegato
        /// </summary>
        Attachment = 1,
        /// <summary>
        /// Documento annesso
        /// </summary>
        Annexed = Attachment * 2,
        /// <summary>
        /// Documento attestazione di conformità
        /// </summary>
        Dematerialisation = Annexed * 2

    }

}