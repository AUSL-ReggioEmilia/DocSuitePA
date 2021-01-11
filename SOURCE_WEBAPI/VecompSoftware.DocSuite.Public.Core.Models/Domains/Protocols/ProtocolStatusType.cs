
namespace VecompSoftware.DocSuite.Public.Core.Models.Domains.Protocols
{
    public enum ProtocolStatusType : int
    {
        /// <summary>
        /// Incompleto
        /// </summary> 
        Incomplete = -10,
        /// <summary>
        /// Rigettato
        /// </summary>
        Rejected = -20,
        /// <summary>
        /// Temporaneo
        /// </summary>
        Temporary = -9,
        /// <summary>
        /// Errato
        /// </summary>
        Wrong = -5,
        /// <summary>
        /// Prenotato
        /// </summary>
        Booked = -4,
        /// <summary>
        /// Sospeso
        /// </summary>
        Suspended = -3,
        /// <summary>
        /// Annullato
        /// </summary>
        Annuled = -2,
        /// <summary>
        /// ErratoE
        /// </summary>
        Invalid = -1,
        /// <summary>
        /// Attivo
        /// </summary>
        Active = 0,
        /// <summary>
        /// Contenitore
        /// </summary>
        Container = 1,
        /// <summary>
        /// Mittenti
        /// </summary>
        Senders = 2,
        /// <summary>
        /// Destinatari
        /// </summary>
        Recipients = 3,
        /// <summary>
        /// Recuperato
        /// </summary>
        Recovered = 4,
        /// <summary>
        /// Utilizzato
        /// </summary>
        Used = 5,
        /// <summary>
        /// Fattura PA inviata
        /// </summary>
        PAInvoiceSent = 6,
        /// <summary>
        /// Fattura PA notificata
        /// </summary>
        PAInvoiceNotified = 7,
        /// <summary>
        /// Fattura PA accettata
        /// Fattura PA Notificata da SDI, non ancora da Destinatario
        /// </summary>
        PAInvoiceAccepted = 8,
        /// <summary>
        /// Fattura PA rifiutata da SDI
        /// </summary>
        PAInvoiceSdiRefused = 9,
        /// <summary>
        /// Fattura rifiutata
        /// </summary>
        PAInvoiceRefused = 10
    }
}
