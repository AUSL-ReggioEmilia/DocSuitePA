using System;
using System.Collections.Generic;
using System.Linq;

namespace VecompSoftware.DocSuiteWeb.DTO.SignDocuments
{
    public class DocumentRootFolder
    {
        public DocumentRootFolder()
        {
            this.DocumentFolders = new List<DocumentFolder>();
        }
        
        public int Id { get; set; }
        public Guid UniqueId { get;set; } 
        public string Name { get; set; }
        public DocumentSignBehaviour SignBehaviour { get; set; }
        public List<DocumentFolder> DocumentFolders { get; set; }
        public string PageUrl { get; set; }
        public bool CommentVisibile { get; set; }
        public bool CommentChecked { get; set; }
        public string Comment { get; set; }
        public bool MainDocumentIsPadesCompliant
        {
            get
            {
                if (DocumentFolders == null || DocumentFolders.Count == 0)
                {
                    return false;
                }
                DocumentFolder mainDocumentFolder = DocumentFolders.FirstOrDefault(x => x.ChainType == Model.Entities.DocumentUnits.ChainType.MainChain);

                if(mainDocumentFolder == null || mainDocumentFolder.Documents == null || mainDocumentFolder.Documents.Count == 0)
                {
                    return false;
                }
                return mainDocumentFolder.Documents.FirstOrDefault().PadesCompliant;
            }
        }
    }
}
