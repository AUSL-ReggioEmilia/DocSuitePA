using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using BiblosDS.Library.Common.Objects;
using BiblosDS.Library.IStorage;

namespace BiblosDS.Library.Storage.Ftp
{
    public class FtpStorage : StorageBase
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(FtpStorage));       

        private new string GetFileName(Document Document)
        {
            return string.Format("{0}{1}", Document.IdDocument, Path.GetExtension(Document.Name));
        }

        /// <summary>
        /// salvataggio del documento nella directory del definitivo
        /// </summary>
        /// <param name="LocalFilePath"></param>
        /// <param name="Storage"></param>
        /// <param name="StorageArea"></param>
        /// <param name="Document"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        /// <remarks>piuttosto di trovarsi in situazioni, che non dovrebbero succedere, di documenti 
        /// nel transito aventi lo stesso nome di documenti nel definitivo e l'impossibilità di sovrascriverli,
        /// viene permesso la sovrascrittura
        /// </remarks>
        protected override long SaveDocument(string LocalFilePath, DocumentStorage Storage, DocumentStorageArea StorageArea, Document Document, System.ComponentModel.BindingList<DocumentAttributeValue> attributeValue)
        {
            //string saveFileName = GetStorageDir(Document.Storage, Document.StorageArea) + GetFileName(Document);
            string[] pathAndPort = Storage.MainPath.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            string path = pathAndPort.FirstOrDefault();
            string port = pathAndPort.LastOrDefault();
            long bytes = -1;
            if (!string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(path))
            {
                logger.Debug(path + " (port" + port + ")");
                logger.Debug("filepath: "  + LocalFilePath + " , StorageAreaPath: " + StorageArea.Path);

                FTPFactory fact = new FTPFactory();
                fact.setRemoteUser(Storage.AuthenticationKey);
                fact.setRemotePass(Storage.AuthenticationPassword);
                fact.setRemoteHost(path);
                fact.setRemotePath(StorageArea.Path);
                fact.setRemotePort(int.Parse(port));

                fact.login();
                fact.upload(LocalFilePath);

                //long bytes = FtpWrite(LocalFilePath, saveFileName, Storage.AuthenticationKey, Storage.AuthenticationPassword);
                bytes = new FileInfo(LocalFilePath).Length;
                logger.Debug(bytes + " writed...");                
            }
            return bytes;
        }

        protected override byte[] LoadDocument(Document Document)
        {
            try
            {
                logger.InfoFormat("LoadDocument " + Document.IdDocument);
                string[] pathAndPort = Document.Storage.MainPath.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string path = pathAndPort.FirstOrDefault();
                string port = pathAndPort.LastOrDefault();
                
                logger.Debug(path + " (port" + port + ")");

                FTPFactory fact = new FTPFactory();
                fact.setRemoteUser(Document.Storage.AuthenticationKey);
                fact.setRemotePass(Document.Storage.AuthenticationPassword);
                fact.setRemoteHost(path);
                fact.setRemotePath(Document.StorageArea.Path);
                fact.setRemotePort(int.Parse(port));

                fact.login();
                string LocalFilePath = Path.GetTempFileName();
                fact.download(GetFileName(Document), LocalFilePath);
                
                return File.ReadAllBytes(LocalFilePath);     
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
                                   
          
        }

        protected override void RemoveDocument(Document Document)
        {
            //string saveFileName = GetFileName(Document);            
            //try
            //{
            //    FtpWebRequest reqFTP;
            //    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("saveFileName"));

            //    if (!string.IsNullOrEmpty(Document.Storage.AuthenticationKey))
            //        reqFTP.Credentials = new NetworkCredential(Document.Storage.AuthenticationKey, Document.Storage.AuthenticationPassword);
            //    reqFTP.KeepAlive = false;
            //    reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

            //    string result = String.Empty;
            //    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
            //    long size = response.ContentLength;
            //    Stream datastream = response.GetResponseStream();
            //    StreamReader sr = new StreamReader(datastream);
            //    result = sr.ReadToEnd();
            //    sr.Close();
            //    datastream.Close();
            //    response.Close();
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //    throw;
            //}
        }

        protected override void SaveAttributes(Document Document)
        {
            //Write the attribute
            try
            {
                if (Document.AttributeValues == null)
                    return;                
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(Document.AttributeValues.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlDocument doc = new XmlDocument();
                    x.Serialize(stream, Document.AttributeValues);
                    stream.Position = 0;
                    doc.Load(stream);
                    doc.DocumentElement.Attributes.RemoveAll();
                    string tmpFileName = Path.Combine(Path.GetTempPath(),
                                                      Document.IdDocument + Path.GetExtension(Document.Name) + ".xml");
                    doc.Save(tmpFileName);                  
                    try
                    {
                        string[] pathAndPort = Document.Storage.MainPath.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        string path = pathAndPort.FirstOrDefault();
                        string port = pathAndPort.LastOrDefault();
                        long bytes = -1;
                        if (!string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(path))
                        {
                            logger.Debug(path + " (port" + port + ")");
                            logger.Debug("filepath: " + tmpFileName + " , StorageAreaPath: " + Document.StorageArea.Path);

                            FTPFactory fact = new FTPFactory();
                            fact.setRemoteUser(Document.Storage.AuthenticationKey);
                            fact.setRemotePass(Document.Storage.AuthenticationPassword);
                            fact.setRemoteHost(path);
                            fact.setRemotePath(Document.StorageArea.Path);
                            fact.setRemotePort(int.Parse(port));

                            fact.login();
                            fact.upload(tmpFileName);

                            //long bytes = FtpWrite(LocalFilePath, saveFileName, Storage.AuthenticationKey, Storage.AuthenticationPassword);
                            bytes = new FileInfo(tmpFileName).Length;
                            logger.Debug(bytes + " writed...");                                                      
                        }
                        //FtpWrite(tmpFileName, storage + GetFileName(Document), Document.Storage.AuthenticationKey, Document.Storage.AuthenticationPassword);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        throw;
                    }
                    try
                    {
                        File.Delete(tmpFileName);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        protected override BindingList<DocumentAttributeValue> LoadAttributes(Document Document)
        {
            byte[] metadata = null;

            try
            {
                logger.InfoFormat("LoadAttributes " + Document.IdDocument);
                string[] pathAndPort = Document.Storage.MainPath.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                string path = pathAndPort.FirstOrDefault();
                string port = pathAndPort.LastOrDefault();                

                logger.Debug(path + " (port" + port + ")");

                FTPFactory fact = new FTPFactory();
                fact.setRemoteUser(Document.Storage.AuthenticationKey);
                fact.setRemotePass(Document.Storage.AuthenticationPassword);
                fact.setRemoteHost(path);
                fact.setRemotePath(Document.StorageArea.Path);
                fact.setRemotePort(int.Parse(port));

                fact.login();
                string LocalFilePath = Path.GetTempFileName();
                fact.download(string.Format("{0}{2}.{1}", Document.IdDocument, "xml", Path.GetExtension(Document.Name)), LocalFilePath);

                metadata = File.ReadAllBytes(LocalFilePath);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            try
            {                            
                XmlTextReader reader = new XmlTextReader(new MemoryStream(metadata)); 

                BindingList<DocumentAttributeValue> saveAttributes = new BindingList<DocumentAttributeValue>();
                //Gianni: Use linq to Xml because with the object type of value th deserialize fail
                var attr = from c in XElement.Load(reader).Elements("DocumentAttributeValue")
                           select c;
                DocumentAttributeValue attributeItem;
                foreach (var item in attr)
                {
                    attributeItem = new DocumentAttributeValue();
                    attributeItem.Value = item.Element("Value").Value.TryConvert(Type.GetType(item.Element("Attribute").Element("AttributeType").Value));
                    attributeItem.Attribute = new DocumentAttribute
                    {
                        IdAttribute = new Guid(item.Element("Attribute").Element("IdAttribute").Value),
                        Name = item.Element("Attribute").Element("Name").Value
                    };
                    saveAttributes.Add(attributeItem);
                }                
                return saveAttributes;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return Document.AttributeValues;
        }
        
        protected override long WriteFile(string saveFileName, DocumentStorage storage, DocumentStorageArea storageArea, Document document, DocumentContent content)
        {
            throw new NotImplementedException();
        }

        protected override byte[] LoadAttach(Document Document, string name)
        {
            throw new NotImplementedException();
        }
    }
}
