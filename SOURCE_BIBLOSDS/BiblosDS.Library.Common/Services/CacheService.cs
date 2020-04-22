using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BiblosDS.Library.Common.Objects;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using BiblosDS.Library.Common.Utility;

namespace BiblosDS.Library.Common.Services
{
    public class CacheService : ServiceBase
    {
        static DateTime? lastVerifyDate;
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CacheService));

        public static void DeleteCache(DocumentCache cache, Document document)
        {

            try
            {
                logger.DebugFormat("DeleteCache {0} {1}", document.IdDocument, cache.FileName);
                DbProvider.DeleteCache(cache, document.IdDocument);
                if (File.Exists(cache.FileName))
                    File.Delete(cache.FileName);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public static string AddCache(Document document, DocumentContent content, string fileName, string signature)
        {
            try
            {
                logger.DebugFormat("AddCache IdDocument:{0}, IdBiblos:{1}, fileName:{2}, signature:{3}", document.IdDocument, document.IdBiblos, fileName, signature);
                if (content != null && ConfigurationManager.AppSettings["PathCache"] != null)
                {
                    if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["PathCache"].ToString()))
                    {
                        logger.DebugFormat("AddCache DISABLED IdDocument:{0}, IdBiblos:{1}, fileName:{2}, signature:{3}", document.IdDocument, document.IdBiblos, fileName, signature);
                        return string.Empty;
                    }
                    string fullPath = Path.Combine(ConfigurationManager.AppSettings["PathCache"], document.Archive.Name);
                    if (!Directory.Exists(fullPath))
                        Directory.CreateDirectory(fullPath);
                    //
                    var signatureHash = "O";
                    if (!string.IsNullOrEmpty(signature))
                    {
                        SHA1 sha = new SHA1CryptoServiceProvider();
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        signatureHash = UtilityService.GetStringFromBob(sha.ComputeHash(encoding.GetBytes(signature.ToString())));
                        Path.GetInvalidPathChars().ToList().ForEach(x =>
                        {
                            signatureHash = signatureHash.Replace(x.ToString(), "Ox30");
                        });
                        signatureHash = signatureHash.Replace("/", "Ox29");
                    }
                    fileName = Path.Combine(fullPath, string.Format("{0}_{1}{2}", document.IdDocument, signatureHash, string.IsNullOrEmpty(fileName) ? Path.GetExtension(document.Name) : fileName.IndexOf('.') > 0 ? Path.GetExtension(fileName) : "." + fileName));
                    //
                    DbProvider.AddCache(new DocumentCache { FileName = fileName, Signature = signature, ServerName = System.Environment.MachineName }, document.IdDocument, content.Blob.Length);                    
                    File.WriteAllBytes(fileName, content.Blob);
                    //Manage Cache
                    if ((DateTime.Now - lastVerifyDate.GetValueOrDefault()).TotalMinutes > 0)
                    {
                        try
                        {
                            lastVerifyDate = DateTime.Now;
                            int workerThread = 0;
                            int completionThread = 0;
                            ThreadPool.GetAvailableThreads(out workerThread, out completionThread);
                            if (completionThread > 0)
                                ThreadPool.QueueUserWorkItem(new WaitCallback(VerifyCacheLimit), document.Archive.IdArchive);
                        }
                        catch (Exception err)
                        {
                            logger.Error(err);
                        }
                    }
                    return fileName;
                }                
            }
            catch (Exception ex)
            {
                logger.Error(ex);                
            }
            return string.Empty;
        }

        public static byte[] GetFromChache(Document document, string FileFormat, string XmlLabel)
        {
            try
            {
                logger.DebugFormat("GetFromChache {0}, {1}, {2}", document.IdDocument, FileFormat, XmlLabel);
                if (document.Cache == null)
                {
                    //Load chache
                    document.Cache = DbProvider.GetDocumentCache(document.IdDocument, System.Environment.MachineName);
                }
                DocumentCache cache = null;
                if (document.Cache != null && (cache = document.Cache.Where(x => x.FileName.EndsWith((FileFormat.IndexOf('.') > 0 ? Path.GetExtension(FileFormat) : FileFormat)) && ((string.IsNullOrEmpty(XmlLabel) && string.IsNullOrEmpty(x.Signature)) || x.Signature == XmlLabel)).FirstOrDefault()) != null)
                {
                    logger.DebugFormat("GetFromChache Get Document {0} {1}", document.IdDocument, cache.FileName);
                    if (File.Exists(cache.FileName))
                    {
                        return File.ReadAllBytes(cache.FileName);
                    }
                    else
                        DeleteCache(cache, document);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);                
            }            
            return null;//No document in chache
        }

        static void VerifyCacheLimit(object idArchive)
        {
            try
            {
                logger.InfoFormat("VerifyCacheLimit idArchive: {0}", idArchive);
                var cache = DbProvider.GetCahceSize(System.Environment.MachineName);
                DocumentArchive archive = DbProvider.GetArchive((Guid)idArchive);
                if (cache.GetValueOrDefault() > archive.MaxCache.GetValueOrDefault())
                {
                    var toDel = DbProvider.GetDocumentCache(System.Environment.MachineName, 20, 0);
                    foreach (var item in toDel)
                    {
                        logger.InfoFormat("VerifyCacheLimit idArchive: {0} DeleteCache: {1} IdDocument: {2}", idArchive, item.FileName, item.IdDocument);
                        DeleteCache(item, new Document(item.IdDocument));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);                
            }           
        }
    }
}
