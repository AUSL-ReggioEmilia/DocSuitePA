using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.Exchange.WebServices.Data;

namespace VecompSoftware.GenericMailSender
{
    internal class EwsClient : IMailClientProvider
    {

        #region [ Fields ]
       
        private static Guid _myPropertySetId = Guid.Parse("EA76D1D6-000C-48FD-876B-B5E3BE23E2A5");

        private readonly ExchangeService _service;
        private NetworkCredential _credential;
        #endregion

        #region [ Properties ]

        public string ExchangeWebServicesAddress
        {
            get
            {
                return _service != null ? _service.Url.ToString() : "";
            }
            set
            {
                if (_service != null)
                {
                    _service.Url = new Uri(value);
                }
            }
        }

        public NetworkCredential Credentials
        {
            get { return _credential; }
            set
            {
                if (_service == null) return;
                _credential = value;
                _service.Credentials = string.IsNullOrEmpty(value.Domain)
                    ? new WebCredentials(value.UserName, value.Password)
                    : new WebCredentials(value.UserName, value.Password, value.Domain);
            }
        }

        public bool UseDefaultCredentials
        {
            get
            {
                return _service != null && _service.UseDefaultCredentials;
            }
            set
            {
                if (_service != null)
                {
                    _service.UseDefaultCredentials = value;
                }
            }
        }

        #endregion

        #region [ Constructor ]

        public EwsClient(string url)
        {
            _service = new ExchangeService(ExchangeVersion.Exchange2007_SP1) { Url = new Uri(url) };
        }

        #endregion

        #region [ Methods ]
        public byte[] Send(MailMessage message, MailAddress notificationTo, bool returnBlob = false)
        {
            return Send(message, returnBlob, true, notificationTo);
        }

        public byte[] Send(MailMessage message, bool returnBlob = false, bool needDispositionNotification = false, MailAddress notificationTo = null)
        {
            // Creo il messaggio base
            var ewsMessage = new EmailMessage(_service)
            {
                Subject = message.Subject,
                From = new EmailAddress(message.From.DisplayName, message.From.Address),
                Body = new MessageBody(BodyType.HTML, message.Body.Replace(Environment.NewLine, "<br />"))
            };

            // Carico i destinatari A
            foreach (var recipient in message.To)
                ewsMessage.ToRecipients.Add(recipient.DisplayName, recipient.Address);

            // Carico i destinatari CC
            foreach (var recipient in message.CC)
                ewsMessage.CcRecipients.Add(recipient.DisplayName, recipient.Address);

            // Carico gli allegati
            foreach (var attachment in message.Attachments)
                ewsMessage.Attachments.AddFileAttachment(attachment.Name, attachment.ContentStream);

            //Specifico se notificare al mittente la notifiche di lettura e ricezione
            if (needDispositionNotification)
            {
                ewsMessage.IsReadReceiptRequested = true;
                ewsMessage.IsDeliveryReceiptRequested = true;
            }

            // Priority
            ewsMessage.Importance = (Importance)Enum.Parse(typeof(Importance), message.Priority.ToString());
            
            // Create a custom extended property and add it to the message. 
            var myExtendedPropertyDefinition = new ExtendedPropertyDefinition(_myPropertySetId, "MyExtendedPropertyName", MapiPropertyType.String);
            ewsMessage.SetExtendedProperty(myExtendedPropertyDefinition, "MyExtendedPropertyValue");

            // Salvo nella directory della posta inviata la mail spedita
            ewsMessage.Save(new FolderId(WellKnownFolderName.SentItems, new Mailbox(message.From.Address)));
            ewsMessage.SendAndSaveCopy(new FolderId(WellKnownFolderName.SentItems, new Mailbox(message.From.Address)));

            // Send the message and save a copy. 
            // Con questo metodo lo manda "per conto di"
            //ewsMessage.SendAndSaveCopy();

            // Wait one second (while EWS sends and saves the message). 
            System.Threading.Thread.Sleep(2000);

            // Now, find the saved copy of the message by using the custom extended property. 
            var view = new ItemView(5);
            SearchFilter searchFilter = new SearchFilter.IsEqualTo(myExtendedPropertyDefinition, "MyExtendedPropertyValue");
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, myExtendedPropertyDefinition);
            var findResults = _service.FindItems(new FolderId(WellKnownFolderName.SentItems, new Mailbox(message.From.Address)), searchFilter, view);

            // Process results. 
            var sentMessage = findResults.Items.OfType<EmailMessage>().FirstOrDefault();

            var bytes = new byte[] { };
            if (returnBlob && sentMessage != null)
            {
                // Ritorno i byte
                //var sentMessage = _service.FindItems(WellKnownFolderName.SentItems, new ItemView(1)).Items.First();
                var propertySet = new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.MimeContent, EmailMessageSchema.IsRead);
                //var sentMessage = EmailMessage.Bind(_service, ewsMessage.Id, propertySet);
                var result = _service.LoadPropertiesForItems(new List<Item>() { sentMessage }, propertySet);
                if (result.First().Result == ServiceResult.Success)
                    bytes = sentMessage.MimeContent.Content;
            }
            return bytes;

            //File.WriteBytes("filename.eml", ewsMessage.MimeContent.Content);


            //ewsMessage.Load(new PropertySet(ItemSchema.MimeContent));
            //var mimeContent = ewsMessage.MimeContent;
            //byte[] bytes;
            //using (var fileStream = new MemoryStream())
            //{
            //    fileStream.Write(mimeContent.Content, 0, mimeContent.Content.Length);
            //    bytes = fileStream.ToByteArray();
            //}
            //return bytes;
        }

        #endregion
    }
}