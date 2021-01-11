using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using VecompSoftware.StampaConforme.Interfaces.Common.Services;

namespace VecompSoftware.StampaConforme.Services.Common.Cache
{
    public class CacheService : ICacheService
    {
        #region [ Fields ]

        #endregion

        #region [ Properties ]
        public string CacheDir => ConfigurationManager.AppSettings["CacheDir"];
        #endregion

        #region [ Constructor ]
        public CacheService()
        {

        }
        #endregion

        #region [ Methods ]
        public void CreateDocument(byte[] documentContent, byte[] referenceContent, string fileName)
        {
            if (string.IsNullOrEmpty(CacheDir))
            {
                return;
            }

            string fileExtension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(fileExtension))
            {
                fileExtension = fileName;
            }
            string documentId = CreateDocumentId(referenceContent);
            string currentFileName = Path.Combine(CacheDir, $"{documentId}.cache.{fileExtension}");
            File.WriteAllBytes(currentFileName, documentContent);
        }

        public byte[] FindDocument(byte[] documentContentToFind, string fileName)
        {
            if (string.IsNullOrEmpty(CacheDir))
            {
                return null;
            }

            string fileExtension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(fileExtension))
            {
                fileExtension = fileName;
            }
            string documentId = CreateDocumentId(documentContentToFind);
            string currentFileName = Path.Combine(CacheDir, $"{documentId}.cache.{fileExtension}");

            if (!File.Exists(currentFileName))
            {
                return null;
            }
            return File.ReadAllBytes(currentFileName);
        }

        private string CreateDocumentId(byte[] documentContent)
        {
            using (SHA256 sha256Service = SHA256.Create())
            {
                byte[] hashValue = sha256Service.ComputeHash(documentContent);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte hashByte in hashValue)
                {
                    stringBuilder.AppendFormat("{0:X2}", hashByte);
                }
                return stringBuilder.ToString();
            }
        }
        #endregion        
    }
}
