using System.Net.Mail;

namespace VecompSoftware.GenericMailSender
{
    internal interface IMailClientProvider
    {
        /// <summary>
        /// Invia un messaggio "MailMessage" standard tramite il provider scelto
        /// </summary>
        /// <param name="message">mailMessage in formato standard da inviare</param>
        /// <param name="returnBlob">definisce se deve essere calcolato lo stream generato</param>
        /// <returns>byte array rappresentante lo stream inviato</returns>
        byte[] Send(MailMessage message, bool returnBlob = false, bool needDispositionNotification = false, MailAddress notificationTo = null);
    }
}
