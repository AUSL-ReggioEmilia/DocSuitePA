using System.Globalization;
using Limilabs.Client;
using Limilabs.Client.IMAP;
using Limilabs.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VecompSoftware.Helpers.ExtensionMethods;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.MailManager
{
    public class ImapClient : IMailClient 
    {
        private ImapClientParams Pars { get; set; }

        public ImapClient(ImapClientParams pars)
        {
            Pars = pars;
        }


        public int GetMails(short idBox, Boolean IsProtocolBox, string boxRecipient, Func<string, string, bool> headerExistHandler, string defaultSubject)
        {
            MailBuilder builder = new MailBuilder();
            var counter = 0;

            using (Imap mailClient = CreateMailClient())
            {
                List<long> mails = GetMailsUids(mailClient, Pars.ImapSearchFlag, Pars.ImapStartDate, Pars.ImapEndDate);

                for (int messageCounter = mails.Count - 1;
                     messageCounter >= 0 && counter < Pars.MaxMailsForSession;
                     messageCounter--)
                {

                    if (Pars.UserCanceled())
                    {
                        return counter;
                    }

                    long uid = mails[messageCounter];

                    LogAction(string.Format("Get Headers Mail Uid:{0}", uid));
                    byte[] headers = mailClient.GetHeadersByUID(uid);
                    string headerHash = headers.ComputeSHA256Hash();

                    // Verifico se già presente, controlla header hash
                    MessageInfo info = mailClient.GetMessageInfoByUID(uid);
                    LogAction(String.Format("Check Header Checksum Mail Uid:{0} / Subject:{1} / Data:{2}", uid, info.Envelope.Subject, info.Envelope.Date));
                    if (headerExistHandler(headerHash, boxRecipient))
                    {
                        LogAction(String.Format("Mail Uid:{0} - Skipped", uid));
                        //TODO: inserire logica di 'eliminazione o identificare la pec come già presente
                        continue;
                    }

                    //controlla se ho già scaricata nel drop folder
                    if (MailInfo.CheckMailExist(Pars.DropFolder, headerHash))
                    {
                        //TODO: inserire logica di 'emilinazione o identificare la pec come già presente
                        continue;
                    }

                    string outpathEml, outpathXml;
                    Utils.GetDropFilenames(Pars.DropFolder, out outpathEml, out outpathXml);

                    //mail info
                    MailInfo mInfo = new MailInfo
                    {
                        Client = MailInfo.ClientType.Imap,
                        EmlFilename = outpathEml,
                        IDPECMailBox = idBox,
                        IsProtocolBox = IsProtocolBox,
                        MailBoxRecipient = boxRecipient,
                        MailUID = uid.ToString(CultureInfo.InvariantCulture)
                    };

                    IMail email = builder.CreateFromEml(headers);
                    mInfo.Parse(email);
                    mInfo.Body = DownloadBody(mailClient, info);
                    mInfo.Size = GetMailSize(info);
                    mInfo.SaveAs(outpathXml);

                    //download eml
                    LogAction("Download Mail Uid:" + uid);
                    byte[] eml = mailClient.GetMessageByUID(uid);
                    File.WriteAllBytes(outpathEml, eml);

                    //salva eml hash del messaggio
                    mInfo.HeaderHash = headerHash;
                    mInfo.EmlHash = eml.ComputeSHA256Hash();
                    mInfo.UpdateStatus(MailInfo.ProcessStatus.Downloaded);
                    mInfo.Save();
                    counter++;
                }

                mailClient.Close();
                return counter;
            }
        }

        public int GetWatcherMails(short idBox, Boolean IsProtocolBox, string boxRecipient, Func<string, string, bool> headerExistHandler, string defaultSubject, DateTime? endDate)
        {
            MailBuilder builder = new MailBuilder();
            var counter = 0;

            using (Imap mailClient = CreateMailClient())
            {
                List<long> mails = GetMailsUids(mailClient, Pars.ImapSearchFlag, Pars.ImapStartDate, endDate);

                for (int messageCounter = mails.Count - 1;
                     messageCounter >= 0 && counter < Pars.MaxMailsForSession;
                     messageCounter--)
                {
                    if (Pars.UserCanceled())
                    {
                        return counter;
                    }

                    long uid = mails[messageCounter];

                    LogAction(string.Format("Get Headers Mail Uid:{0}", uid));
                    byte[] headers = mailClient.GetHeadersByUID(uid);
                    string headerHash = headers.ComputeSHA256Hash();

                    // Verifico se già presente, controlla header hash
                    MessageInfo info = mailClient.GetMessageInfoByUID(uid);
                    LogAction(String.Format("Check Header Checksum Mail Uid:{0} / Subject:{1} / Data:{2}", uid, info.Envelope.Subject, info.Envelope.Date));
                    //if (headerExistHandler(headerHash, boxRecipient))
                    //{
                    //    LogAction(String.Format("Mail Uid:{0} - Skipped", uid));
                    //    //TODO: inserire logica di 'eliminazione o identificare la pec come già presente
                    //    continue;
                    //}
                    ////controlla se ho già scaricata nel drop folder
                    //if (MailInfo.CheckMailExist(Pars.DropFolder, headerHash))
                    //{
                    //    //TODO: inserire logica di 'emilinazione o identificare la pec come già presente
                    //    continue;
                    //}

                    string outpathEml, outpathXml;
                    Utils.GetDropFilenames(Pars.DropFolder, out outpathEml, out outpathXml);

                    //mail info
                    MailInfo mInfo = new MailInfo
                    {
                        Client = MailInfo.ClientType.Imap,
                        EmlFilename = outpathEml,
                        IDPECMailBox = idBox,
                        IsProtocolBox = IsProtocolBox,
                        MailBoxRecipient = boxRecipient,
                        MailUID = uid.ToString(CultureInfo.InvariantCulture)
                    };

                    IMail email = builder.CreateFromEml(headers);
                    mInfo.Parse(email);
                    mInfo.Body = DownloadBody(mailClient, info);
                    mInfo.Size = GetMailSize(info);
                    mInfo.SaveAs(outpathXml);

                    //download eml
                    LogAction("Download Mail Uid:" + uid);
                    byte[] eml = mailClient.GetMessageByUID(uid);
                    File.WriteAllBytes(outpathEml, eml);

                    //salva eml hash del messaggio
                    mInfo.HeaderHash = headerHash;
                    mInfo.EmlHash = eml.ComputeSHA256Hash();
                    mInfo.UpdateStatus(MailInfo.ProcessStatus.Downloaded);
                    mInfo.Save();
                    counter++;
                }

                mailClient.Close();
                return counter;
            }
        }


        public void DeleteMail(String uid)
        {
            DeleteMail(long.Parse(uid));
        }

        public void DeleteMail(long uid)
        {
            using (Imap mailClient = CreateMailClient())
            {
                mailClient.DeleteMessageByUID(uid);
                mailClient.Close();
            }
        }


        public void MarkMessageSeen(long uid)
        {
            using (Imap mailClient = CreateMailClient())
            {
                mailClient.MarkMessageSeenByUID(uid);
            }
        }


        public void MoveByUid(long uid, string destFolder)
        {
            using (Imap mailClient = CreateMailClient())
            {
                mailClient.MoveByUID(uid, GetOrCreateFolder(mailClient, destFolder));
            }
        }

        private static string GetOrCreateFolder(Imap connection, string folderCaption)
        {
            if (string.IsNullOrEmpty(folderCaption))
            {
                return String.Empty;
            }

            // Cerco il folder
            var folderElement = connection.GetFolders()
                .Find(folder => folder.ShortName.Eq(folderCaption) || folder.Name.Eq(folderCaption));

            // Il folder esiste ed è selezionabile, quindi ne restituisco la dicitura corretta
            if (folderElement != null && folderElement.CanSelect)
            {
                return folderElement.Name;
            }

            // Il folder non esiste allora lo genero a partire dalla Inbox
            var folderName = String.Format("{0}.{1}", connection.SelectInbox().Name, folderCaption);
            connection.CreateFolder(folderName);
            return folderName;
        }


        public List<string> GetFolders()
        {
            using (var mailClient = CreateMailClient())
            {
                String str = "Cartella trovata sul server: {0} [CanSelect: {1}].";
                return mailClient.GetFolders()
                    .Select(folderInfo => string.Format(str, folderInfo.Name, folderInfo.CanSelect))
                  .ToList();
            }
        }

        private void openConnection(Action<string> action, Action<string, Int32> actionWithPort, ImapClientParams pars)
        {
            if (pars.Port == 0)
            {
                action(Pars.IncomingServer);
            }
            else
            {
                actionWithPort(Pars.IncomingServer, Pars.Port);
            }
        }

        private Imap CreateMailClient()
        {
            var client = new Imap { ReceiveTimeout = new TimeSpan(0, 0, 10, 0), SendTimeout = new TimeSpan(0, 0, 10, 0) };

            if (Pars.UseSsl)
            {
                client.SSLConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                openConnection(client.ConnectSSL, client.ConnectSSL, Pars);
                
            }
            else
            {
                openConnection(client.Connect, client.Connect, Pars);                    
            }

            client.UseBestLogin(Pars.Username, Pars.Password);
            client.ServerCertificateValidate += OnCertificateValidate;

            if (!string.IsNullOrEmpty(Pars.ImapFolder))
            {
                client.Select(Pars.ImapFolder);
            }    
            else
            {
                client.SelectInbox();
            }

            return client;
        }

        private List<long> GetMailsUids(Imap imap, ImapFlag imapFlag, DateTime? startDate, DateTime? endDate)
        {
            ICriterion expression = Expression.HasFlag(imapFlag.Flag());

            if (startDate.HasValue && !startDate.Equals(default(DateTime)))
            {
                expression = Expression.And(Expression.Since(startDate.Value), expression);
            }

            if (endDate.HasValue && !endDate.Equals(default(DateTime)))
            {
                endDate = endDate.Value.AddHours(-endDate.Value.Hour).AddMinutes(-endDate.Value.Minute).AddSeconds(-endDate.Value.Second).AddDays(1);
                expression = Expression.And(Expression.Before(endDate.Value),expression);
            }

            return imap.Search().Where(expression).GetList();
        }

        private static void OnCertificateValidate(object sender, ServerCertificateValidateEventArgs e)
        {
            e.IsValid = true;
        }

        private long GetMailSize(MessageInfo info)
        {
            return (info.Envelope.Size ?? 0) + info.BodyStructure.Attachments.Sum(item => item.TransferSize);
        }

        private string DownloadBody(Imap mailClient, MessageInfo info)
        {
            if (info.BodyStructure.Html != null)
            {
                return mailClient.GetTextByUID(info.BodyStructure.Html);
            }
                
            if (info.BodyStructure.Text != null)
            {
                return mailClient.GetTextByUID(info.BodyStructure.Text);
            }
                

            return "#";
        }

        private void LogAction(string str)
        {
            if (Pars.LogActionHandler != null)
            {
                Pars.LogActionHandler(str);
            }
        }

  
             
    }
    
}
