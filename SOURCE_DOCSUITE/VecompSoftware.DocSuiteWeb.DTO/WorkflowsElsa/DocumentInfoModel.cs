using System;

namespace VecompSoftware.DocSuiteWeb.DTO.WorkflowsElsa
{
    public class DocumentInfoModel
    {
        #region [ Properties ]
        public Guid DocumentId
        {
            get;
        }
        public string Filename
        {
            get;
        }
        public string VirtualPath
        {
            get;
        }
        public string FileExtension
        {
            get;
        }
        public int Size
        {
            get;
        }
        public string ReferenceType
        {
            get;
        }
        public bool IsOriginal
        {
            get;
        }
        #endregion

        #region [ Constructor ]
        public DocumentInfoModel(
            Guid documentId,
            string filename,
            string virtualPath,
            string fileExtension,
            int size,
            string referenceType,
            bool isOriginal)
        {
            DocumentId = documentId;
            Filename = filename;
            VirtualPath = virtualPath;
            FileExtension = fileExtension;
            Size = size;
            ReferenceType = referenceType;
            IsOriginal = isOriginal;
        }
        #endregion
    }
}
