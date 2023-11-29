using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits;

namespace VecompSoftware.DocSuiteWeb.DTO.SignDocuments
{
    public class DocumentFolder
    {
        public DocumentFolder()
        {
            this.Documents = new List<Document>();
        }

        public string Name { get; set; }
        public ChainType ChainType { get; set; }
        public List<Document> Documents { get; set; }
    }
}
