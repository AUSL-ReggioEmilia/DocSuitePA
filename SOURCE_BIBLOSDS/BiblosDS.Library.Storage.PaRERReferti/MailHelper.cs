using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net ; 

namespace BiblosDS.Library.Storage.PaRERReferti
{
    class MailHelper
    {
        public static void SendMail(string Anno, string Numero, string Archivio, string Errore, bool isError) 
        {
            MailAddress fromAddress = new MailAddress(Properties.Settings.Default.SMTP_SenderMail, "BiblosDS PaRER Referti");
            MailAddress toAddress = new MailAddress(Properties.Settings.Default.SMTP_ReceiveMail);

            string subject;
            string body;

            if (isError == true)
            {
                subject = "AVVISO: Errato versamento al PaRER del referto, Anno/Numero/Archivio " + Anno + " " + Numero + " " + Archivio;
                body = "Il versamento presso PaRER del referto  Anno/Numero/Archivio " + Anno + " " + Numero + " " + Archivio + "\n Ha riportato la seguente segnalazione:\n " + Errore +
                    "\n\n Collegarsi a BiblosDS di SMV-VEC-BIBRER e verificare.\n";
            }
            else
            {
                subject = "INFO: Versamento al PaRER del referto, Anno/Numero/Archivio " + Anno + " " + Numero + " " + Archivio;
                body = "Il versamento presso PaRER del referto  Anno/Numero/Archivio " + Anno + " " + Numero + " " + Archivio + "\n Ha riportato la seguente informazione.\n" +
                    "Non è necessaria alcuna azione, ricevete questa email come notifica." + Errore ;
            }

            var smtp = new SmtpClient
                       {
                           Host = Properties.Settings.Default.SMTP_Server,
                           Port = Properties.Settings.Default.SMTP_Port,
                           EnableSsl = Properties.Settings.Default.SMTP_EnableSSL ,
                           DeliveryMethod = SmtpDeliveryMethod.Network,
                           UseDefaultCredentials = false,
                           Credentials = new NetworkCredential(Properties.Settings.Default.SMTP_User, Properties.Settings.Default.SMTP_Password)
                       };

            using (var message = new MailMessage(fromAddress, toAddress)
                                    {
                                        Subject = subject,
                                        Body = body
                                    })
            {
                smtp.Send(message);
            }
        }
    }
}
