namespace VecompSoftware.DocSuiteWeb.Model.Entities.Collaborations
{
    public class CollaborationActionType
    {
        /// <summary>
        /// Inoltro Documento per Visione/Firma
        /// </summary>
        public const string DaVisionareFirmare = "CF";

        /// <summary>
        ///  Documento da Protocollare
        /// </summary>
        public const string DaProtocollareGestire = "CD";

        /// <summary>
        /// Cancellazione Documento/Annullamento documento
        /// </summary>
        public const string CancellazioneDocumento = "RM";
    }
}
