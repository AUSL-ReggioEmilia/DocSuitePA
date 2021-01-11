namespace VecompSoftware.DocSuiteWeb.Services.WSProt.DTO
{
    public interface IProtocolStatusDto
    {
        /// <summary>
        /// Id del ProtocolStatus
        /// </summary>
        int Incremental { get; set; }
        /// <summary>
        /// Codice del ProtocolStatus
        /// </summary>
        string StatusCode { get; set; }
        /// <summary>
        /// Descrizione del ProtocolStatus
        /// </summary>
        string StatusDescription { get; set; }
    }
}
