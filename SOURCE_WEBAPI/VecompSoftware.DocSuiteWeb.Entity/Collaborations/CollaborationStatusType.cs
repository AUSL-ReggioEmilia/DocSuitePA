namespace VecompSoftware.DocSuiteWeb.Entity.Collaborations
{
    public class CollaborationStatusType
    {
        /// <summary>
        /// Bozza
        /// </summary>
        public const string Draft = "BZ";
        /// <summary>
        /// Inserimento per visione/firma
        /// </summary>
        public const string Insert = "IN";
        /// <summary>
        /// Da Protocollare
        /// </summary>
        public const string ToProtocol = "DP";
        /// <summary>
        /// Finalizzato al Protocollo
        /// </summary>
        public const string Registered = "PT";
        /// <summary>
        /// Da leggere
        /// </summary>
        public const string ToRead = "DL";
        /// <summary>
        /// Stato non previsto
        /// </summary>
        /// <remarks>Casistica di errore</remarks>
        public const string NotExpected = "NP";
        /// <summary>
        /// Non trovata
        /// </summary>
        /// <remarks>Casistica di errore</remarks>
        public const string NotFound = "NT";
        /// <summary>
        /// Collaborazione speciale
        /// </summary>
        public const string NoManage = "WM";
    }
}
