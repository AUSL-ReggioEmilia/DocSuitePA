using System;
using VecompSoftware.DocSuite.Public.Core.Models.Securities;

namespace VecompSoftware.DocSuite.Public.Core.Models
{
    /// <summary>
    ///  Interfacci generalista dei messaggi 
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Chiave di 'correlazione'. Può essere utile per legare più messaggi allo stesso processo di business.
        /// </summary>
        Guid? CorrelationId { get; set; }

        /// <summary>
        /// Codice univo del messaggio
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Classe che specifica l'Identità dell'utente che sta eseguento l'avvio del Workflow<see cref="IdentityContext"/>
        /// </summary>
        IdentityContext IdentityContext { get; }

        /// <summary>
        /// Nome interno del messaggio
        /// </summary>
        string MessageName { get; }
        /// <summary>
        /// Guid del TenantAOO del Cliente. Contattare Dgroove per il valore
        /// </summary>
        Guid TenantAOOId { get; }
        /// <summary>
        /// Guid del Tenant del Cliente. Contattare Dgroove per il valore
        /// </summary>
        Guid TenantId { get; }

        /// <summary>
        /// Nome del Tenant del Cliente. Contattare Dgroove per il valore
        /// </summary>
        string TenantName { get; }
    }
}