using System.ComponentModel;

namespace VecompSoftware.DocSuiteWeb.Entity.Desks
{
    /// <summary>
    /// Ruolo dell'utente all'interno del tavolo.
    /// </summary>
    public enum DeskPermissionType
    {
        /// <summary>
        /// Utente che ha aperto il tavolo
        /// o Può chiudere il tavolo
        /// o Può mettere il tavolo in approvazione
        /// o Può richiedere le approvazioni
        /// o Può fare nuovi inviti
        /// o Può rimuovere partecipanti
        /// o Può visualizzare i documenti
        /// o Può scrivere nella lavagna
        /// o Può fare il check out e check in dei documenti
        /// o Può approvare una versione di un documento
        /// </summary>
        [Description("Autore del tavolo")]
        Admin = 1,
        /// <summary>
        /// Utente con invito di gestione
        /// o Può chiudere il tavolo
        /// o Può mettere il tavolo in approvazione
        /// o Può richiedere le approvazioni
        /// o Può fare nuovi inviti
        /// o Può rimuovere partecipanti (tranne chi ha aperto il tavolo)
        /// o Può visualizzare i documenti
        /// o Può scrivere nella lavagna
        /// o Può fare il check out e check in dei documenti
        /// o Può approvare una versione di un documento
        /// </summary>
        [Description("Amministratore")]
        Manage = 2,
        /// <summary>
        /// Utente con invito di consultazione
        /// o Può visualizzare i documenti
        /// o Può scrivere nella lavagna
        /// </summary>
        [Description("Lettore")]
        Reader = 2 * Manage,
        /// <summary>
        /// Utente con invito di partecipazione
        /// o Può visualizzare i documenti
        /// o Può scrivere nella lavagna
        /// o Può fare il check out e check in dei documenti
        /// </summary>
        [Description("Contributore")]
        Contributor = 2 * Reader,
        /// <summary>
        /// Utente con invito di approvazione
        /// o Può visualizzare i documenti
        /// o Può scrivere nella lavagna
        /// o Può fare il check out e check in dei documenti
        /// o Può approvare una versione di un documento
        /// </summary>
        [Description("Approvatore")]
        Approval = 2 * Contributor
    }
}
