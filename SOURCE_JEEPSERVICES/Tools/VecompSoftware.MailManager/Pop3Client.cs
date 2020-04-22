using System;
using System.IO;
using Limilabs.Client;
using Limilabs.Client.POP3;
using Limilabs.Mail;

using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.MailManager
{
    public class Pop3Client : IMailClient
    {
        private Pop3ClientParams Pars { get; set; }

        public Pop3Client(Pop3ClientParams pars)
        {
            Pars = pars;
        }


        public int GetMails(short idBox, Boolean IsProtocolBox, string boxRecipient, Func<string, string, bool> headerExistHandler, string defaultSubject)
        {
            var builder = new MailBuilder();
            var counter = 0;

            using (Pop3 mailClient = CreateMailClient())
            {
                var currentStats = mailClient.GetAccountStat();

                for (var messageCounter = (int)currentStats.MessageCount;
                  messageCounter > 0 && counter < Pars.MaxMailsForSession;
                  messageCounter--)
                {

                    if (Pars.UserCanceled())
                        return counter;

                    var uid = mailClient.GetUID(messageCounter);

                    LogAction("Get Headers Mail Uid:" + uid);
                    byte[] headers = mailClient.GetHeadersByUID(uid);
                    string headerHash = headers.ComputeSHA256Hash();

                    // Verifico se già presente, controlla header hash
                    var email = builder.CreateFromEml(headers);
                    LogAction(string.Format("Check Headers Mail Uid:{0} / Subject:{1} / Data:{2}", uid, email.Subject, email.Date));
                    if (headerExistHandler(headerHash, boxRecipient))
                        continue;

                    //controlla se ho già scaricata nel drop folder
                    if (MailInfo.CheckMailExist(Pars.DropFolder, headerHash))
                        continue;

                    string outpathEml, outpathXml;
                    Utils.GetDropFilenames(Pars.DropFolder, out outpathEml, out outpathXml);

                    //mail info
                    MailInfo mInfo = new MailInfo
                    {
                        Client = MailInfo.ClientType.Pop3,
                        EmlFilename = outpathEml,
                        IDPECMailBox = idBox,
                        IsProtocolBox = IsProtocolBox,
                        MailBoxRecipient = boxRecipient,
                        MailUID = uid
                    };

                    mInfo.Parse(email);
                    mInfo.Body = "#";
                    mInfo.Size = GetMailSize(mailClient, messageCounter);
                    mInfo.SaveAs(outpathXml);

                    //download eml
                    LogAction("Download Mail Uid:" + uid);
                    byte[] eml = mailClient.GetMessageByUID(uid);
                    File.WriteAllBytes(outpathEml, eml);

                    //Aggiorna il Body
                    //Pop3 deve forzatamente scaricare l'intero messaggio per ottenere il body della mail
                    email = builder.CreateFromEml(eml);

                    mInfo.HeaderHash = headerHash;
                    mInfo.EmlHash = eml.ComputeSHA256Hash();
                    mInfo.Body = email.GetBodyAsHtml();
                    mInfo.UpdateStatus(MailInfo.ProcessStatus.Downloaded);
                    mInfo.Save();

                    counter++;
                }

                return counter;
            }
        }

        public int GetWatcherMails(short idBox, Boolean IsProtocolBox, string boxRecipient, Func<string, string, bool> headerExistHandler, string defaultSubject, DateTime? endDate)
        {
            var builder = new MailBuilder();
            var counter = 0;

            using (var mailClient = CreateMailClient())
            {
                var currentStats = mailClient.GetAccountStat();

                for (var messageCounter = (int)currentStats.MessageCount;
                  messageCounter > 0 && counter < Pars.MaxMailsForSession;
                  messageCounter--)
                {

                    if (Pars.UserCanceled())
                        return counter;

                    var uid = mailClient.GetUID(messageCounter);

                    LogAction("Get Headers Mail Uid:" + uid);
                    byte[] headers = mailClient.GetHeadersByUID(uid);
                    string headerHash = headers.ComputeSHA256Hash();

                    // Verifico se già presente, controlla header hash
                    var email = builder.CreateFromEml(headers);
                    LogAction(String.Format("Check Headers Mail Uid:{0} / Subject:{1} / Data:{2}", uid, email.Subject, email.Date));
                    if (headerExistHandler(headerHash, boxRecipient))
                        continue;

                    //controlla se ho già scaricata nel drop folder
                    if (MailInfo.CheckMailExist(Pars.DropFolder, headerHash))
                        continue;

                    string outpathEml, outpathXml;
                    Utils.GetDropFilenames(Pars.DropFolder, out outpathEml, out outpathXml);

                    //mail info
                    MailInfo mInfo = new MailInfo
                    {
                        Client = MailInfo.ClientType.Pop3,
                        EmlFilename = outpathEml,
                        IDPECMailBox = idBox,
                        IsProtocolBox = IsProtocolBox,
                        MailBoxRecipient = boxRecipient,
                        MailUID = uid
                    };

                    mInfo.Parse(email);
                    mInfo.Body = "#";
                    mInfo.Size = GetMailSize(mailClient, messageCounter);
                    mInfo.SaveAs(outpathXml);

                    //download eml
                    LogAction("Download Mail Uid:" + uid);
                    byte[] eml = mailClient.GetMessageByUID(uid);
                    File.WriteAllBytes(outpathEml, eml);

                    //Aggiorna il Body
                    //Pop3 deve forzatamente scaricare l'intero messaggio per ottenere il body della mail
                    email = builder.CreateFromEml(eml);

                    mInfo.HeaderHash = headerHash;
                    mInfo.EmlHash = eml.ComputeSHA256Hash();
                    mInfo.Body = email.GetBodyAsHtml();
                    mInfo.UpdateStatus(MailInfo.ProcessStatus.Downloaded);
                    mInfo.Save();

                    counter++;
                }

                return counter;
            }
        }


        private Pop3 CreateMailClient()
        {
            var client = new Pop3();

            if (Pars.UseSsl)
            {
                client.SSLConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                if (Pars.Port == 0)
                    client.ConnectSSL(Pars.IncomingServer);
                else
                    client.ConnectSSL(Pars.IncomingServer, Pars.Port);
            }
            else
            {
                if (Pars.Port == 0)
                    client.Connect(Pars.IncomingServer);
                else
                    client.Connect(Pars.IncomingServer, Pars.Port);
            }

            client.Login(Pars.Username, Pars.Password);
            client.ServerCertificateValidate += OnCertificateValidate;
            return client;
        }


        private static void OnCertificateValidate(object sender, ServerCertificateValidateEventArgs e)
        {
            e.IsValid = true;
        }


        private long GetMailSize(Pop3 mailClient, int messageCounter)
        {
            try
            {
                var response = mailClient.SendCommand("LIST " + messageCounter);
                return Convert.ToInt64(response.Message.Split(' ')[1]);
            }
            catch
            {
                return -1;
            }
        }


        public void DeleteMail(long uid)
        {
            DeleteMail(uid.ToString());
        }

        public void DeleteMail(String uid)
        {
            using (Pop3 mailClient = CreateMailClient())
            {
                mailClient.DeleteMessageByUID(uid);
                mailClient.Close();
            }
        }


        private void LogAction(string str)
        {
            if (Pars.LogActionHandler != null)
                Pars.LogActionHandler(str);
        }

    }

}
