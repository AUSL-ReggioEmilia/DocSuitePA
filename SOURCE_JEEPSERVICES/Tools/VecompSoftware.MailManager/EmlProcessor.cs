using System.Globalization;
using Limilabs.Mail;
using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Limilabs.Mail.MIME;
using System.Security.Cryptography;
using System.Collections.Generic;
using VecompSoftware.Helpers.Compress;

namespace VecompSoftware.MailManager
{
    public class EmlProcessor
    {
        public static string EmlExtensions = ".eml";
        public static string CompressExtensions = ".zip|.7z|.gz|.gzip|.rar|.tar|.bz2|.bzip2";

        private readonly int _biblosMaxLength = 255;

        /// <summary>
        /// Costruttore di default (utilizza come dimensione massima di file 255 caratteri
        /// </summary>
        public EmlProcessor()
        { }

        /// <summary>
        /// Costruttore che permette di definire il numero massimo di caratteri da utilizzare
        /// </summary>
        /// <param name="biblosMaxLength"></param>
        public EmlProcessor(int biblosMaxLength)
        {
            if (biblosMaxLength <= 255)
            {
                _biblosMaxLength = biblosMaxLength;
            }
            else
            {
                throw new Exception("Il numero massimo di caratteri consentito dal sistema è 255. Impostare il parametro \"BiblosMaxLength\" ad un valore <= 255");
            }
        }

        public static string HashSHA512(Byte[] stream)
        {
            if (stream == null)
            {
                return string.Empty;
            }
            using(SHA512CryptoServiceProvider alg = new SHA512CryptoServiceProvider())
            {
                byte[] hashByte = alg.ComputeHash(stream);
                return Encoding.UTF8.GetString(hashByte);
            }
        }

        public XElement ProcessFile(string filename, string tempDir)
        {
            int id = 1;
            int idparent = 0;

            XElement root = new XElement("mail");

            XAttribute rootAttr = new XAttribute("filename", filename);
            root.Add(rootAttr);

            while (true)
            {
                string nextFile = Execute(root, tempDir, filename, ref id, ref idparent);
                if (String.IsNullOrEmpty(nextFile))
                {
                    break;
                }
                filename = nextFile;
            }
            return root;
        }


        private string Execute(XElement root, string tempDir, string filename, ref int id, ref int idparent)
        {
            int nextIdParent = -1;
            string nextFile = String.Empty;

            IMail mail = new MailBuilder().CreateFromEmlFile(filename);

            // utilizzo NonVisuals per evitare di scaricare anche le immagini comprese nell'Html
            // cfr. http://www.limilabs.com/blog/download-email-attachments-net

            //TODO: BUGS mail.NonVisuals elimina anche il postacert eml
            //      e quindi la gestione dello sbusta del contenuto.
            foreach (MimeData mime in mail.Attachments)
            {
                if (mime.HasContentType && 
                    (mime.ContentType == Limilabs.Mail.Headers.ContentType.ImageBmp  ||
                    mime.ContentType == Limilabs.Mail.Headers.ContentType.ImageGif   ||
                    mime.ContentType == Limilabs.Mail.Headers.ContentType.ImageJpeg  ||
                    mime.ContentType == Limilabs.Mail.Headers.ContentType.ImagePng   ||
                    mime.ContentType == Limilabs.Mail.Headers.ContentType.ImageTiff))
                {
                    if (mail.Visuals != null && mail.Visuals.Any(mv => HashSHA512(mv.Data).Equals(HashSHA512(mime.Data))))
                    {
                        continue;
                    }
                }
                string outpath = Utils.CreateTempName(tempDir, mime.SafeFileName, _biblosMaxLength).Trim();
                XElement child = new XElement("file", outpath);

                //parent
                XAttribute childParent = new XAttribute("idParent", idparent.ToString(CultureInfo.InvariantCulture));
                child.Add(childParent);

                if (EmlExtensions.Contains(Path.GetExtension(outpath).ToLower()))
                {
                    nextFile = outpath;
                    nextIdParent = id;
                }

                //id
                XAttribute childId = new XAttribute("id", id++.ToString(CultureInfo.InvariantCulture));
                child.Add(childId);

                //sourcename
                XAttribute childSource = new XAttribute("sourceName", Utils.SafeFileName(mime.SafeFileName, _biblosMaxLength));
                child.Add(childSource);

                //mimetype
                XAttribute childMime = new XAttribute("mimeType", mime.ContentType.MimeType.Name);
                child.Add(childMime);

                //mimetype
                XAttribute childSubMime = new XAttribute("mimeSubtype", mime.ContentType.MimeSubtype.Name);
                child.Add(childSubMime);

                root.Add(child);
                mime.Save(outpath);

                //gestione dello zip
                if (CompressExtensions.Contains(Path.GetExtension(outpath).ToLower()))
                {
                    try
                    {
                        ProcessCompressAttachment(root, tempDir, outpath, id - 1, ref id);
                    }
                    catch (Exception ex)
                    {
                        //gestione errore di file compresso che non riesce ad elaborare
                        //non lo elabora e prosegue
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            //salva la mail escluso gli allegati (body)
            var bodyPath = Utils.CreateTempName(tempDir, "body.eml", _biblosMaxLength);

            mail.RemoveAttachments(new AttachmentRemoverConfiguration
            {
                RemoveAlternatives = false,
                RemoveVisuals = false
            });

            File.WriteAllBytes(bodyPath, mail.Render());

            XElement body = new XElement("file", bodyPath);

            XAttribute bodyParent = new XAttribute("idParent", idparent.ToString(CultureInfo.InvariantCulture));
            body.Add(bodyParent);

            XAttribute bodyId = new XAttribute("id", id++.ToString(CultureInfo.InvariantCulture));
            body.Add(bodyId);

            XAttribute isBody = new XAttribute("isBody", true);
            body.Add(isBody);

            XAttribute bodyName = new XAttribute("sourceName", "busta.eml");
            body.Add(bodyName);

            root.Add(body);

            idparent = nextIdParent;
            return nextFile;
        }

        private void ProcessCompressAttachment(XElement root, string tempDir, string filepath, int idparent, ref int id)
        {
            using (FileStream stream = File.OpenRead(filepath))
            {
                bool isRar = Path.GetExtension(filepath).ToLower().EndsWith(".rar");
                ICollection<CompressItem> results = null;
                if (isRar)
                {
                    results = new RarCompress().InMemoryExtract(stream);
                }
                else
                {
                    results = new ZipCompress().InMemoryExtract(stream);
                }

                foreach (CompressItem item in results)
                {
                    string outpath = Utils.CreateTempName(tempDir, item.Filename, _biblosMaxLength);

                    XElement child = new XElement("file", outpath);

                    //parent
                    XAttribute childParent = new XAttribute("idParent", idparent.ToString(CultureInfo.InvariantCulture));
                    child.Add(childParent);

                    //id
                    XAttribute childId = new XAttribute("id", id++.ToString(CultureInfo.InvariantCulture));
                    child.Add(childId);

                    //sourcename
                    XAttribute childSource = new XAttribute("sourceName", item.Filename);
                    child.Add(childSource);

                    root.Add(child);
                    File.WriteAllBytes(outpath, item.Data);

                    //inner zip
                    string extension = Path.GetExtension(outpath);
                    if (extension != null && CompressExtensions.Contains(extension.ToLower()))
                    {
                        ProcessCompressAttachment(root, tempDir, outpath, id - 1, ref id);
                    }
                }
            }
        }
    }
}
