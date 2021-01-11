using Limilabs.Mail;
using Limilabs.Mail.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using VecompSoftware.DocSuiteWeb.Data;

namespace VecompSoftware.MailManager
{
    /// <summary>
    /// Questa classe viene serializzata con i dati contenuti nel file eml 
    /// </summary>
    public class MailInfo
    {
        public enum ClientType
        {
            None,
            Imap,
            Pop3
        }

        public enum ProcessStatus
        {
            None,
            Downloaded,
            Processed,
            Archived
        }

        [NonSerialized]
        public string sourceFile;

        public short IDPECMailBox { get; set; }
        public Boolean IsProtocolBox { get; set; }
        public int IDPECMail { get; set; }
        public string MailBoxRecipient { get; set; }
        public ClientType Client { get; set; }
        public string MailUID { get; set; }

        public string EmlFilename { get; set; }
        public string HeaderHash { get; set; }
        public string EmlHash { get; set; }

        public string Subject { get; set; }
        public string Sender { get; set; }
        public string Recipients { get; set; }
        public string RecipientsCc { get; set; }
        public DateTime Date { get; set; }
        public string XTrasporto { get; set; }
        public string MessageID { get; set; }
        public string XRiferimentoMessageID { get; set; }
        public short Priority { get; set; }
        public long Size { get; set; }

        public string fileIndex { get; set; }
        public string fileBody { get; set; }
        public string filePostacert { get; set; }
        public string fileDaticert { get; set; }
        public string fileSMime { get; set; }
        public string fileSegnatura { get; set; }

        public CData Body { get; set; }
        public List<String> Errors { get; set; }
        public ProcessStatus Status { get; set; }

        public MailInfo()
        {
            IDPECMailBox = 0;
            IDPECMail = 0;
            MailBoxRecipient = String.Empty;
            Client = ClientType.None;
            MailUID = String.Empty;
            IsProtocolBox = false;

            EmlFilename = String.Empty;
            HeaderHash = String.Empty;
            EmlHash = String.Empty;

            Subject = String.Empty;
            Sender = String.Empty;
            Recipients = String.Empty;
            RecipientsCc = String.Empty;
            Date = DateTime.MinValue;
            XTrasporto = String.Empty;
            MessageID = String.Empty;
            XRiferimentoMessageID = String.Empty;
            Priority = 0;
            Size = 0;

            fileIndex = String.Empty;
            fileBody = String.Empty;
            filePostacert = String.Empty;
            fileDaticert = String.Empty;
            fileSMime = String.Empty;
            fileSegnatura = String.Empty;

            Body = String.Empty;
            Errors = new List<string>();
            Status = ProcessStatus.None;
        }

        public static MailInfo Load(string filename)
        {
            MailInfo res = XmlFile<MailInfo>.Load(filename);
            res.sourceFile = filename;
            return res;
        }

        //controlla tramite gli header hash nei mail info se ho già scaricato questo mail nel drop-folder
        public static bool CheckMailExist(string dropFolder, string headerHash)
        {
            string[] files = GetInfoFiles(dropFolder);
            return files.Select(Load)
                .Any(mInfo => mInfo != null && mInfo.HeaderHash == headerHash);
        }

        //ritorna elenco di file info in ordine di creazione
        public static string[] GetInfoFiles(string dropFolder)
        {
            return new DirectoryInfo(dropFolder).GetFiles("*_info.xml")
              .OrderBy(f => f.CreationTime)
              .Select(f => f.FullName)
              .ToArray();
        }

        public void Save()
        {
            XmlFile<MailInfo>.Serialize(this, sourceFile);
        }


        public void SaveAs(string filename)
        {
            sourceFile = filename;
            Save();
        }

        public void AddError(string message, Exception ex)
        {
            Errors.Add(String.Concat(message, "|", Utils.FullStacktrace(ex)));
        }

        public bool HasError()
        {
            return Errors.Count > 0;
        }

        public string LastError()
        {
            return Errors.LastOrDefault();
        }

        public void UpdateStatus(ProcessStatus status)
        {
            Errors = new List<string>();
            Status = status;
        }

        private const string replaceSubject = "[\x00-\x08\x0B\x0C\x0E-\x1F]";
        private const string replaceSubjectSpecialChar = "[\x3C\x3E\x35\x36\x37\x38]";


        //Torna una stringa senza i caratteri non ASCII
        public string GeneralParsing(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            string result = Regex.Replace(value.Trim(), replaceSubject, string.Empty, RegexOptions.Compiled);
            result = result.Replace(@"\", @"\\");
            result = Regex.Unescape(result);            

            return result;
        }

        public void Parse(IMail mail)
        {
            Subject = GeneralParsing(mail.Subject);

            Sender = ParseMailBoxes(mail.From);
            Recipients = ParseMailAddress(mail.To);
            RecipientsCc = ParseMailAddress(mail.Cc);
            

            // Imposto la data
            Date = mail.Date.HasValue ? mail.Date.Value : DateTime.Now;

            if (mail.Document != null &&
                mail.Document.Root != null &&
                mail.Document.Root.Headers != null)
            {
                XTrasporto = (mail.Document.Root.Headers["X-Trasporto"] ?? String.Empty).Trim();
                MessageID = (mail.Document.Root.Headers["Message-ID"] ?? String.Empty).Trim();
                XRiferimentoMessageID = (mail.Document.Root.Headers["X-Riferimento-Message-ID"] ?? String.Empty).Trim();
            }

            //priorità
            Priority = (short)System.Net.Mail.MailPriority.Normal;
            if (mail.Priority == MimePriority.NonUrgent)
            {
                Priority = (short)System.Net.Mail.MailPriority.Low;
            }
            else if (mail.Priority == MimePriority.Normal)
            {
                Priority = (short)System.Net.Mail.MailPriority.Normal;
            }
            else if (mail.Priority == MimePriority.Urgent)
            {
                Priority = (short)System.Net.Mail.MailPriority.High;
            }                        
        }


        public void ParsePostacert(IMail mail)
        {
            //sovrascrive subject, sender e recipients della mail originale

            Subject = GeneralParsing(mail.Subject);

            Sender = ParseMailBoxes(mail.From);
            Recipients = ParseMailAddress(mail.To);
            RecipientsCc = ParseMailAddress(mail.Cc);
        }


        private string ParseMailAddress(IList<MailAddress> mailBoxList)
        {
            if (mailBoxList == null || mailBoxList.Count == 0)
            {
                return String.Empty;
            }

            StringBuilder mailAddresses = new StringBuilder();
            foreach (MailAddress mb in mailBoxList)
            {
                if (mailAddresses.Length != 0)
                {
                    mailAddresses.Append("; ");
                }

                mailAddresses.Append(ParseMailBoxes(mb.GetMailboxes()));
            }
            return mailAddresses.ToString().Trim();
        }


        public string ParseMailBoxes(IList<MailBox> mailBoxList)
        {
            if (mailBoxList == null || mailBoxList.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder mailAddresses = new StringBuilder();
            foreach (MailBox mb in mailBoxList.Where(mb => !string.IsNullOrEmpty(mb.Address)))
            {
                if (mailAddresses.Length != 0)
                {
                    mailAddresses.Append("; ");
                }

                mailAddresses.Append(string.IsNullOrEmpty(mb.Name) ? mb.Address : string.Format("{0} <{1}>", mb.Name, mb.Address));
            }
            return mailAddresses.ToString().Trim();
        }


        public void Process(string dropFolder, int biblosMaxLength)
        {
            string step = string.Empty;

            try
            {
                string filePrefix = GetPrefix(EmlFilename);

                //crea una cartella temporanea con la desinenza del nome del file eml
                string tempFolder = Path.Combine(dropFolder, filePrefix);
                step = string.Format("Creazione Temp Folder: '{0}'", tempFolder);

                //se esiste svuota
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
                Directory.CreateDirectory(tempFolder);

                string indexFile = Path.Combine(dropFolder, string.Format("{0}_index.xml", filePrefix));
                fileIndex = indexFile;

                EmlProcessor processor = new EmlProcessor(biblosMaxLength);

                step = string.Format("Elaborazione Eml: '{0}'", EmlFilename);
                XElement indexRoot = processor.ProcessFile(EmlFilename, tempFolder);
                indexRoot.Save(indexFile);

                MailIndexFile index = MailIndexFile.Load(indexFile);

                //attachment particolari

                //body di primo livello
                MailAttachmentFile attachment = index.files.FirstOrDefault(p => p.isBody && p.idParent == 0);
                if (attachment != null)
                {
                    fileBody = attachment.Filename;
                }

                //postacert.eml
                int postacertId = 0;
                attachment = index.files.FirstOrDefault(p => p.Filename.ToLower().EndsWith("postacert.eml") && p.idParent == 0);
                if (attachment != null &&
                    attachment.mimeType == MimeType.Message.Name &&
                    attachment.mimeSubtype == MimeSubtype.Rfc822.Name)
                {
                    step = string.Format("Elaborazione '{0}'", attachment.Filename);

                    IMail email = new MailBuilder().CreateFromEmlFile(attachment.Filename);
                    ParsePostacert(email);
                    Body = email.GetBodyAsHtml();
                    filePostacert = attachment.Filename;
                    postacertId = attachment.id;
                }

                //daticert.xml
                attachment = index.files.FirstOrDefault(p => p.Filename.ToLower().EndsWith("daticert.xml") && p.idParent == 0);
                if (attachment != null &&
                    attachment.mimeType == MimeType.Application.Name &&
                    attachment.mimeSubtype == MimeSubtype.Xml.Name)
                {
                    step = string.Format("Elaborazione '{0}'", attachment.Filename);

                    fileDaticert = attachment.Filename;
                }

                //smime.p7s
                attachment = index.files.FirstOrDefault(p => p.Filename.ToLower().EndsWith("smime.p7s") && p.idParent == 0);
                if (attachment != null &&
                    attachment.mimeType == MimeType.Application.Name &&
                    attachment.mimeSubtype == MimeSubtype.XPkcs7Signature.Name)
                {
                    step = string.Format("Elaborazione '{0}'", attachment.Filename);

                    fileSMime = attachment.Filename;
                }

                //segnatura.xml
                //cerca tra i figli di postacert.eml
                if (postacertId > 0)
                {
                    attachment = index.files.FirstOrDefault(p => p.Filename.ToLower().EndsWith("segnatura.xml") && p.idParent == postacertId);
                    if (attachment != null)
                    {
                        step = string.Format("Elaborazione '{0}'", attachment.Filename);

                        fileSegnatura = attachment.Filename;
                    }
                }

                //salva informazioni
                step = string.Format("Salva MailInfo in '{0}'", sourceFile);
                UpdateStatus(ProcessStatus.Processed);
                Save();
            }
            catch (Exception ex)
            {
                string message = string.Format("Errore in ProcessFile:'{0}' step:{1}, Exception:{2}", sourceFile, step, ex.Message);
                AddError(message, ex);
                Save();
                throw new ApplicationException(LastError(), ex);
            }
        }


        public void RemoveFiles(string dropFolder)
        {
            string[] files;
            string filePrefix = GetPrefix(EmlFilename);

            //cartella temporanea
            string tempDir = Path.Combine(dropFolder, filePrefix);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            //rimuove i file
            files = Directory.GetFiles(dropFolder, String.Concat(filePrefix, "*"));
            foreach (string filename in files)
            {
                File.Delete(filename);
            }
        }


        public void MoveFiles(string dropFolder, string destFolder)
        {
            string filePrefix = GetPrefix(EmlFilename);

            //spost la cartella temporanea
            string tempDir = Path.Combine(dropFolder, filePrefix);
            if (Directory.Exists(tempDir))
            {
                Utils.CopyDirectory(tempDir, Path.Combine(destFolder, filePrefix));
                Directory.Delete(tempDir, true);
            }

            //sposta i file
            string[] files = Directory.GetFiles(dropFolder, filePrefix + "*");
            foreach (string filename in files)
            {
                File.Copy(filename, Path.Combine(destFolder, Path.GetFileName(filename)), true);
                File.Delete(filename);
            }
        }

        private string GetPrefix(string filename)
        {
            filename = Path.GetFileName(filename);
            if (filename == null || filename.Length < Utils.PrefixLen)
            { 
                return String.Empty;
            }

            return filename.Substring(0, Utils.PrefixLen);
        }


    }
}
