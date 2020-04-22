using BiblosDS.LegalExtension.AdminPortal.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BiblosDS.LegalExtension.AdminPortal.Helpers
{
    public class UploadHelper
    {
        #region [ Fields ]
        private readonly ILog _logger = LogManager.GetLogger(typeof(UploadHelper));
        #endregion

        #region [ Properties ]

        #endregion

        #region [ Constructor ]
        public UploadHelper()
        {

        }
        #endregion

        #region [ Methods ]
        public void UploadSave(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(ConfigurationHelper.GetAppDataPath(), fileName);
                    file.SaveAs(physicalPath);
                }
            }
        }

        public FileResult ChunkUploadSave(IEnumerable<HttpPostedFileBase> files, string metaData, string uniqueId)
        {
            ChunkMetaData chunkMetaData = JsonConvert.DeserializeObject<ChunkMetaData>(metaData);
            string extension = Path.GetExtension(chunkMetaData.FileName);
            string fullPath = Path.Combine(ConfigurationHelper.GetAppDataPath(), string.Concat(uniqueId, extension));
            if (files != null)
            {
                foreach (HttpPostedFileBase file in files)
                {                    
                    AppendToFile(fullPath, file.InputStream);
                }
            }

            return new FileResult()
            {
                Uploaded = chunkMetaData.IsUploaded,
                FileUid = chunkMetaData.UploadUid,
                FileName = fullPath
            };
        }

        public void RemoveUploadedFile(string fileName, string uniqueId)
        {
            string extension = Path.GetExtension(fileName);
            string path = Path.Combine(ConfigurationHelper.GetAppDataPath(), string.Concat(uniqueId, extension));
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private void AppendToFile(string fullPath, Stream content)
        {
            try
            {
                using (FileStream stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (content)
                    {
                        content.CopyTo(stream);
                    }
                }
            }
            catch (IOException ex)
            {
                _logger.Error(ex.Message, ex);
                throw ex;
            }
        }
        #endregion
    }
}