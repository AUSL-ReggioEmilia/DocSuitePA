
namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols
{
    /// <summary>
    /// Protocol type
    /// </summary>
    public enum ProtocolType : int
    {
        /// <summary>
        /// Ingresso
        /// </summary>
        Inbound = -1,
        /// <summary>
        /// Interno / Tra uffici
        /// </summary>
        Internal = 0,
        /// <summary>
        /// Uscita
        /// </summary>
        Outgoing = 1,
    }
}
