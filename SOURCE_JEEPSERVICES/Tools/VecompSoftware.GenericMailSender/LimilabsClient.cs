using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Limilabs.Client;
using Limilabs.Client.SMTP;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.MIME;
using StandardMailAddress = System.Net.Mail.MailAddress;

namespace VecompSoftware.GenericMailSender
{
    internal class LimilabsClient : IMailClientProvider
    {
        #region [ Fields ]

        private readonly string _server;
        private readonly int _port;

        #endregion

        #region [ Properties ]

        public MailClient.AuthenticationType AuthenticationType { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public int Timeout { get; set; }
        public NetworkCredential Credentials { get; set; }

        #endregion

        #region [ Constructor ]

        public LimilabsClient(string server, int port)
        {
            _server = server;
            _port = port;
        }

        #endregion

        public byte[] Send(MailMessage message, StandardMailAddress notificationTo, bool returnBlob = false)
        {
            return Send(message, returnBlob, true, notificationTo);
        }

        public byte[] Send(MailMessage message, bool returnBlob = false, bool needDispositionNotification = false, StandardMailAddress notificationTo = null)
        {
            // Gestore dei messaggi
            MailBuilder builder = new MailBuilder { MessageID = Guid.NewGuid().ToString(), Subject = message.Subject };

            // Creo il messaggio base
            builder.From.Add(new MailBox(message.From.Address, message.From.DisplayName));
            builder.Text = message.Body;
            builder.Html = message.Body.Replace(Environment.NewLine, "<br />");

            // Carico i destinatari A
            foreach (var recipient in message.To)
                builder.To.Add(new MailBox(recipient.Address, recipient.DisplayName));

            // Carico i destinatari CC
            foreach (var recipient in message.CC)
                builder.Cc.Add(new MailBox(recipient.Address, recipient.DisplayName));
            
            //Specifico se notificare al mittente la notifiche di lettura e ricezione
            if (needDispositionNotification && notificationTo != null)
            {
                MailBox originalSender = new MailBox(notificationTo.Address, notificationTo.DisplayName);
                builder.RequestReadReceipt();
                builder.NotificationTo.Clear();
                builder.ReplyTo.Clear();
                builder.ReturnReceiptTo.Clear();
                builder.NotificationTo.Add(originalSender);
                builder.ReturnReceiptTo.Add(originalSender);
            }
            
            // Carico gli allegati
            foreach (var attachment in message.Attachments)
            {
                var mime = new MimeFactory().CreateMimeData();
                using (var ms = new MemoryStream())
                {
                    attachment.ContentStream.CopyTo(ms);
                    mime.Data = ms.ToArray();
                }

                mime.ContentType = ContentType.Parse(attachment.Name.GetMimeType());
                mime.FileName = attachment.Name;
                builder.AddAttachment(mime);
            }

            // Priorità
            switch (message.Priority)
            {
                case MailPriority.Low:
                    builder.Priority = MimePriority.NonUrgent;
                    break;
                case MailPriority.Normal:
                    builder.Priority = MimePriority.Normal;
                    break;
                case MailPriority.High:
                    builder.Priority = MimePriority.Urgent;
                    break;
            }

            // Genero la mail da spedire
            var email = builder.Create();

            var sent = false;
            var i = 0;
            Exception lastException = null;
            while (!sent && i < 5)
            {
                using (var smtp = new Smtp())
                {
                    // Accetto anche i certificati non validi
                    smtp.ServerCertificateValidate += ServerCertificateValidateCallBack;

                    // Connessione al server
                    smtp.Connect(_server, _port, AuthenticationType == MailClient.AuthenticationType.Ssl);

                    // Attivazione Tls, se richiesta
                    if (AuthenticationType == MailClient.AuthenticationType.Tls)
                        smtp.StartTLS();

                    // Login, se necessario
                    if (!UseDefaultCredentials)
                    {
                        smtp.UseBestLogin(Credentials.UserName, Credentials.Password);
                    }

                    // Imposto il timeout per la richiesta
                    smtp.SendTimeout = new TimeSpan(0, 0, 0, 0, Timeout);

                    // Invio e calcolo il risultato
                    var messageResult = smtp.SendMessage(email);
                    sent = (messageResult.Status == SendMessageStatus.Success);
                    try
                    {
                        smtp.Close(true);
                    }
                    catch (Exception ex)
                    {
                        lastException = ex;
                    }
                }

                if (!sent)
                {
                    Thread.Sleep(1000 * 30);
                    continue;
                }
                i++;
            }

            if (returnBlob && email != null)
            {
                byte[] eml = email.Render();
                return eml;
            }

            if (!sent && lastException != null)
                throw new Exception("Impossibile inviare il messaggio dopo 5 tentativi.", lastException);

            return null;
        }

        public static void ServerCertificateValidateCallBack(object sender, ServerCertificateValidateEventArgs e)
        {
            e.IsValid = true;
        }
    }
}
