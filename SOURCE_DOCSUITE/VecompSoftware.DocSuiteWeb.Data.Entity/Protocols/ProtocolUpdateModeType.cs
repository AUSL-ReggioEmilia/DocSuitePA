namespace VecompSoftware.DocSuiteWeb.Data.Entity.Protocols
{
    public enum ProtocolUpdateModeType
    {
        /// <summary>
        /// Ogni volta che viene modificato il protocollo, si aggiorna data e utente che ha eseguito la modifica.
        /// </summary>
        UpdateAuditable = 0,
        /// <summary>
        /// solo gli utenti con diritti espliciti sul contenitore del protocollo,
        /// aggiornano effettivamente data e utente dell'ultima modifica (si ignorano le modifiche dei settori autorizzati)
        /// </summary>
        UpdateAuditableIfOnlyContainer = 1,
    }
}
