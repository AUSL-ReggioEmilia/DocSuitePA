namespace VecompSoftware.DocSuiteWeb.Entity.PECMails
{
    public enum PECMailMultipleType : int
    {
        ///<summary> Non è necessaria clonazione delle PEC </summary>
        NoSplit = -1,
        ///<summary> Clona per destinatario </summary>
        SplitByRecipients = 0,
        /// <summary> Clona fino al raggiungimento della massima grandezza disponibile </summary>
        SplitBySize = 1,
        /// <summary>
        /// Clona fino al raggiungimento della massima grandezza disponibile
        /// e poi per ogni destinatario
        /// Il risultato è #PEC_da_dimensione_allegati x #contatti
        /// </summary>
        SplitBySizeAndRecipients = 2
    }
}
