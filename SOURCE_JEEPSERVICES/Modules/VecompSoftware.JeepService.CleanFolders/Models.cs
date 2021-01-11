using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.Helpers.ExtensionMethods;

namespace VecompSoftware.JeepService.CleanFolders
{
    public class EmlItem
    {
        #region [ Properties ]
        public string Name { get; set; }
        public long Size { get; set; }
        public string MailUid { get; set; }
        public string HeaderHash { get; set; }
        public string Recipient { get; set; }
        public short RecipientBoxId { get; set; }
        #endregion

        #region [ Constructor ]

        public EmlItem(FileInfo fi)
        {
            Name = fi.Name;
            Size = fi.Length;
            MailUid = GetUid(fi.Name);
            HeaderHash = CreateHeaderHash(fi.FullName);
            RecipientBoxId = GetBoxId(fi.Name);
            Recipient = GetOriginalRecipients(fi.FullName);
        }
        #endregion

        #region [ Methods ]
        public static string EmlHash(string path)
        {
            byte[] blobPecBytes;
            SHA1 sha = new SHA1CryptoServiceProvider();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                blobPecBytes = new byte[fs.Length];
                fs.Read(blobPecBytes, 0, (int)fs.Length);
            }
            var byteHash = sha.ComputeHash(blobPecBytes);
            var hash = Convert.ToBase64String(byteHash);
            return hash;
        }

        private string GetUid(string fileName)
        {
            var splitted = fileName.Split(new[] { '_' });
            return splitted.Count() > 1 ? splitted[1] : string.Empty;
        }

        private short GetBoxId(string fileName)
        {
            short returnValue;
            var splitted = fileName.Substring(0, 6);
            short.TryParse(splitted, out returnValue);
            return returnValue;
        }

        private string CreateHeaderHash(string filePath)
        {
            var body = File.ReadAllText(filePath);
            var header = body.Substring(0, body.IndexOf("\r\n\r\n", StringComparison.Ordinal) + 4);
            var headerhash = header.ComputeSHA256Hash();
            return headerhash;
        }

        private string GetOriginalRecipients(string filePath)
        {
            var facadeFactory = new FacadeFactory();
            var id = GetBoxId(Path.GetFileName(filePath));
            try
            {
                return facadeFactory.PECMailboxFacade.GetById(id).MailBoxName;
            }
            catch
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
