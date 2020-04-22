namespace VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters
{
    /// <summary>
    /// Tipi di segreteria/gestione di collaborazione
    /// </summary>
    public enum ManageType : ushort
    {
        /// <summary>
        /// Valore non definito
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// Utilizza gli account del dominio (oggi gestiti on-premise del Domain Controller del cliente)
        /// Per informazioni contattare Vecomp Software
        /// </summary>
        AD = 1,
        /// <summary>
        /// Utilizza i settori della DocSuite
        /// Per informazioni contattare Vecomp Software
        /// </summary>
        DSWRole = 2,
        /// <summary>
        /// Utilizza il sistema di mapping con le apposite tabelle di trascodifica
        /// Per informazioni contattare Vecomp Software
        /// </summary>
        Mapping = DSWRole * 2,
    }
}