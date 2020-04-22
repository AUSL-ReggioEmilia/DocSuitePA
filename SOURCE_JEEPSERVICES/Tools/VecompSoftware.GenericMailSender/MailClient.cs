using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using VecompSoftware.GenericMailSender.Extensions;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.GenericMailSender
{
    public class MailClient
    {
        #region [ Fields ]

        private SmtpClient _smtp;
        private EwsClient _ewsClient;
        private LimilabsClient _limilabsClient;

        private readonly MailClientType _type;
        private readonly string _server;
        private readonly int _port;
        private readonly AuthenticationType _authenticationType;
        private readonly string _userName;
        private readonly string _userPassword;
        private readonly string _userDomain;

        private const string Logger = "Messages";
        #endregion

        #region [ Properties ]

        private SmtpClient SmtpClientProvider
        {
            get
            {
                if (_smtp != null) return _smtp;
                // Accetta anche i certificati non validi
                ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
                _smtp = new SmtpClient(_server, _port)
                {
                    EnableSsl = _authenticationType == AuthenticationType.Ssl,
                    UseDefaultCredentials = string.IsNullOrEmpty(_userName) && string.IsNullOrEmpty(_userPassword),
                    Timeout = 600000 // Sono 10 min
                };

                if (!_smtp.UseDefaultCredentials)
                {
                    _smtp.Credentials = string.IsNullOrEmpty(_userDomain)
                        ? new NetworkCredential(_userName, _userPassword)
                        : new NetworkCredential(_userName, _userPassword, _userDomain);
                }
                return _smtp;
            }
        }

        private EwsClient EwsClientProvider
        {
            get
            {
                if (_ewsClient != null) return _ewsClient;
                _ewsClient = new EwsClient(_server);

                if (!string.IsNullOrEmpty(_userName) || !string.IsNullOrEmpty(_userPassword))
                {
                    _ewsClient.Credentials = string.IsNullOrEmpty(_userDomain)
                        ? new NetworkCredential(_userName, _userPassword)
                        : new NetworkCredential(_userName, _userPassword, _userDomain);
                }
                return _ewsClient;
            }
        }

        private LimilabsClient LimilabsClientProvider
        {
            get
            {
                if (_limilabsClient != null) return _limilabsClient;
                _limilabsClient = new LimilabsClient(_server, _port)
                {
                    AuthenticationType = _authenticationType,
                    UseDefaultCredentials = string.IsNullOrEmpty(_userName) && string.IsNullOrEmpty(_userPassword),
                    Timeout = 600000 // Sono 10 min
                };

                if (!_limilabsClient.UseDefaultCredentials)
                {
                    _limilabsClient.Credentials = string.IsNullOrEmpty(_userDomain)
                        ? new NetworkCredential(_userName, _userPassword)
                        : new NetworkCredential(_userName, _userPassword, _userDomain);
                }
                return _limilabsClient;
            }
        }

        #endregion

        #region [ Constructor ]

        public MailClient(string type, string server, int port, AuthenticationType authenticationType, string userName, string userPassword, string userDomain)
        {
            _type = (MailClientType)Enum.Parse(typeof(MailClientType), type, true);
            _server = server;
            _port = port;
            _authenticationType = authenticationType;
            _userName = userName;
            _userPassword = userPassword;
            _userDomain = userDomain;
        }

        #endregion

        #region [ Methods ]

        public byte[] Send(MailMessage message, MailAddress notificationTo, DirectoryInfo tempDir = null, bool returnBlob = false)
        {
            return Send(message, tempDir, returnBlob, true, notificationTo);
        }
        public byte[] Send(MailMessage message, 
            DirectoryInfo tempDir = null, 
            bool returnBlob = false, 
            bool needDispositionNotification = false,
            MailAddress notificationTo = null)
        {
            var mailContent = new byte[] { };
            switch (_type)
            {
                case MailClientType.Smtp:
                    // Se è richiesto il salvataggio del blob
                    if (returnBlob)
                    {
                        if (tempDir != null)
                        {
                            // se è definita la tempo allora posso esportare
                            mailContent = message.ToArray(tempDir);
                        }
                        else
                        {
                            // se non è definita allora si interrompe tutto
                            throw new Exception("Impostare la directory TEMP");
                        }
                            
                    }
                    FileLogger.Debug(Logger, string.Format("[SendMails] - needDispositionNotification {0}", needDispositionNotification));
                    //Specifico se notificare al mittente la notifiche di lettura e ricezione
                    if (needDispositionNotification && notificationTo != null)
                    {
                        FileLogger.Debug(Logger, string.Format("[SendMails] - Sender {0}", notificationTo.ToString()));
                        message.Headers.Add("Disposition-Notification-To", notificationTo.ToString());
                        message.Headers.Add("Return-Receipt-To", notificationTo.ToString());
                    }
                    // Se sono qui significa che posso inviare il messaggio.
                    // se avessi avuto problemi sarei uscito prima
                    SmtpClientProvider.Send(message); 
                    break;
                case MailClientType.Exchange:
                    mailContent = EwsClientProvider.Send(message, returnBlob, needDispositionNotification, notificationTo);
                    break;
                case MailClientType.Limilabs:
                    mailContent = LimilabsClientProvider.Send(message, returnBlob, needDispositionNotification, notificationTo);
                    break;
            }
            return mailContent;
        }

        #endregion

        public enum MailClientType
        {
            Smtp,
            Exchange,
            Limilabs
        }

        public enum AuthenticationType
        {
            Plain,
            Ssl,
            Tls
        }
    }
}
