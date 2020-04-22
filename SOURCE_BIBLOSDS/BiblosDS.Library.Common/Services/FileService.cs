using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BiblosDS.Library.Common.Objects;

namespace BiblosDS.Library.Common.Services
{
    public class FileService: ServiceBase
    {
        public static void SaveFileToTransitoLocalPath(Document Document, Byte[] content)
        {          
            if (string.IsNullOrEmpty(Document.Archive.PathTransito))
                Document.Archive = ArchiveService.GetArchive(Document.Archive.IdArchive);
            if (!Directory.Exists(Document.Archive.PathTransito))
                Directory.CreateDirectory(Document.Archive.PathTransito);
            File.WriteAllBytes(Path.Combine(Document.Archive.PathTransito, Document.IdDocument + Path.GetExtension(Document.Name)), content);
        }

        public static void SaveFileToTransitoLocalPath(Document document, string path, Byte[] content)
        {           
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllBytes(Path.Combine(path, document.IdDocument + Path.GetExtension(document.Name)), content);
        }

        public static void SaveFileToTransitoAttachLocalPath(DocumentAttach attach, Byte[] content)
        {
            if (attach == null)
                throw new Exceptions.DocumentNotFound_Exception("SaveFileToTransitoAttachLocalPath : document is null");

            if (attach.Document == null)
                throw new Exceptions.DocumentNotFound_Exception("SaveFileToTransitoAttachLocalPath : referrer document is null");

            if (attach.Document.Archive == null || attach.Document.Archive.IdArchive == Guid.Empty)
                throw new Exceptions.Archive_Exception("SaveFileToTransitoAttachLocalPath : document archive is null");

            if (string.IsNullOrWhiteSpace(attach.Document.Archive.PathTransito))
            {
                attach.Document.Archive = ArchiveService.GetArchive(attach.Document.Archive.IdArchive);
                if (attach.Document.Archive == null || string.IsNullOrWhiteSpace(attach.Document.Archive.PathTransito))
                    throw new Exceptions.Archive_Exception("SaveFileToTransitoAttachLocalPath : document archive transit's path is not valid");
            }

            if (!Directory.Exists(attach.Document.Archive.PathTransito))
                Directory.CreateDirectory(attach.Document.Archive.PathTransito);

            File.WriteAllBytes(Path.Combine(attach.Document.Archive.PathTransito, attach.Document.IdDocument + "_" + attach.IdDocumentAttach + Path.GetExtension(attach.Name)), content);
        }

        public static void DeleteFileToTransitoLocalPath(DocumentTransito Transito)
        {
            try
            {
                File.Delete(Path.Combine(Transito.LocalPath, Transito.Document.IdDocument.ToString() + Path.GetExtension(Transito.Document.Name)));
            }
            catch (Exception ex)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                    "DeleteFileToTransitoLocalPath",
                    ex.ToString(),
                     BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                      BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);

            }
        }

        public static void DeleteFileToTransitoAttachLocalPath(DocumentAttachTransito transito)
        {
            try
            {
                var att = transito.Attach;
                var doc = att.Document;
                File.Delete(Path.Combine(transito.LocalPath, string.Format("{0}_{1}{2}", doc.IdDocument, att.IdDocumentAttach, Path.GetExtension(att.Name))));
            }
            catch (Exception ex)
            {
                Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
                    "DeleteFileToTransitoAttachLocalPath",
                    ex.ToString(),
                     BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
                      BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);

            }
        }

        //public static void DeleteFileToTransitoLocalPath(string Name)
        //{
        //    try
        //    {
        //        File.Delete(Path.Combine(TransitoLocalPath, Name));
        //    }
        //    catch (Exception ex)
        //    {

        //        Logging.WriteLogEvent(BiblosDS.Library.Common.Enums.LoggingSource.BiblosDS_General,
        //            "DeleteFileToTransitoLocalPath",
        //            ex.ToString(),
        //             BiblosDS.Library.Common.Enums.LoggingOperationType.BiblosDS_General,
        //              BiblosDS.Library.Common.Enums.LoggingLevel.BiblosDS_Errors);

        //    }            
        //}

    }
}
