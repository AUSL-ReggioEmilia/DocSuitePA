namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{
    public enum DeskDocumentType
    {
        /// <summary>
        /// Il documento è di tipo "Principale". 
        /// Nei tavoli può esistere più di un documento principale
        /// </summary>
        MainDocument = 1,
        /// <summary>
        /// Il documento è di tipo "Allegato"
        /// </summary>
        Attachment = 2,
        /// <summary>
        /// Il documento è di tipo "Annesso"
        /// </summary>
        Annexed = 2 * Attachment
    }
}
